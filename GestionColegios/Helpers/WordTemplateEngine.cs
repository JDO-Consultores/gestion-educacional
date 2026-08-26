using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace GestionColegios.Helpers
{
    /// <summary>
    /// Motor de generación de documentos Word (.docx) por reemplazo de tags.
    /// Usa System.IO.Packaging (WindowsBase) — no requiere Office ni paquetes externos.
    ///
    /// - Reemplaza marcadores {{TAG}} aunque Word los divida en varios &lt;w:r&gt; (runs).
    /// - Reemplaza la(s) imagen(es) del encabezado por el logo del establecimiento.
    /// </summary>
    public static class WordTemplateEngine
    {
        private const string WordprocessingMlNs =
            "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private const string RelationshipNs =
            "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";

        /// <summary>
        /// Genera un .docx a partir de la plantilla, reemplazando los tags de texto
        /// y, opcionalmente, el logo del encabezado.
        /// </summary>
        /// <param name="plantilla">Bytes del .docx plantilla.</param>
        /// <param name="reemplazos">Diccionario tag (sin llaves) ? valor.</param>
        /// <param name="logo">Bytes del nuevo logo (opcional).</param>
        public static byte[] Generar(
            byte[] plantilla,
            IDictionary<string, string> reemplazos,
            byte[] logo = null)
        {
            if (plantilla == null || plantilla.Length == 0)
                throw new ArgumentException("La plantilla está vacía.", nameof(plantilla));

            // Trabajar sobre una copia en memoria para no alterar la plantilla original.
            using (var ms = new MemoryStream())
            {
                ms.Write(plantilla, 0, plantilla.Length);
                ms.Position = 0;

                using (var package = Package.Open(ms, FileMode.Open, FileAccess.ReadWrite))
                {
                    // 1) Reemplazo de tags de texto en cuerpo, encabezados y pies.
                    foreach (var part in GetWordTextParts(package))
                        ReemplazarTagsEnParte(part, reemplazos);

                    // 2) Reemplazo del logo (todas las imágenes de los encabezados).
                    if (logo != null && logo.Length > 0)
                        ReemplazarLogo(package, logo);

                    package.Flush();
                }

                return ms.ToArray();
            }
        }

        /// <summary>Partes del documento que contienen texto (document, headers, footers).</summary>
        private static IEnumerable<PackagePart> GetWordTextParts(Package package)
        {
            return package.GetParts().Where(p =>
                p.ContentType.IndexOf("wordprocessingml.document.main", StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.ContentType.IndexOf("wordprocessingml.header", StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.ContentType.IndexOf("wordprocessingml.footer", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void ReemplazarTagsEnParte(PackagePart part, IDictionary<string, string> reemplazos)
        {
            var doc = new XmlDocument();
            using (var stream = part.GetStream(FileMode.Open, FileAccess.Read))
                doc.Load(stream);

            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("w", WordprocessingMlNs);

            // Procesar párrafo por párrafo. Word suele fragmentar un tag en varios runs,
            // por lo que se consolida el texto del párrafo, se reemplaza y se reescribe.
            var parrafos = doc.SelectNodes("//w:p", nsm);
            if (parrafos == null) return;

            foreach (XmlNode parrafo in parrafos)
                ReemplazarTagsEnParrafo(parrafo, nsm, reemplazos);

            using (var stream = part.GetStream(FileMode.Create, FileAccess.Write))
                doc.Save(stream);
        }

        private static void ReemplazarTagsEnParrafo(
            XmlNode parrafo, XmlNamespaceManager nsm, IDictionary<string, string> reemplazos)
        {
            // Solo los <w:t> que pertenecen DIRECTAMENTE a este parrafo. Un parrafo
            // puede contener cuadros de texto anidados (w:txbxContent dentro de
            // mc:AlternateContent / VML), que a su vez tienen sus propios <w:p>.
            // Si usaramos el eje descendiente (.//w:t) sobre el parrafo ancla,
            // tomariamos el texto de esas copias anidadas (Choice + Fallback) y se
            // duplicaria / corromperia. Cada <w:p> anidado se procesa por separado.
            var nodosTexto = GetTextNodesDirectos(parrafo, nsm);
            if (nodosTexto.Count == 0) return;

            // Texto completo del parrafo (todos los <w:t> directos concatenados).
            var textoCompleto = string.Concat(nodosTexto.Select(n => n.InnerText));
            if (textoCompleto.IndexOf("{{", StringComparison.Ordinal) < 0)
                return; // No hay tags en este parrafo.

            var textoReemplazado = AplicarReemplazos(textoCompleto, reemplazos);
            if (textoReemplazado == textoCompleto)
                return;

            // Volcar todo el texto resultante en el primer <w:t> y vaciar los demas,
            // preservando el formato del primer run.
            var primero = nodosTexto[0];
            SetPreserveSpace(primero);
            primero.InnerText = textoReemplazado;

            for (int i = 1; i < nodosTexto.Count; i++)
                nodosTexto[i].InnerText = string.Empty;
        }

        /// <summary>
        /// Devuelve los nodos <w:t> cuyo <w:p> ancestro mas cercano es exactamente
        /// el parrafo dado. Excluye los de parrafos anidados (cuadros de texto),
        /// evitando duplicar o corromper el contenido del encabezado.
        /// </summary>
        private static List<XmlNode> GetTextNodesDirectos(XmlNode parrafo, XmlNamespaceManager nsm)
        {
            var resultado = new List<XmlNode>();
            var todos = parrafo.SelectNodes(".//w:t", nsm);
            if (todos == null) return resultado;

            foreach (XmlNode t in todos)
            {
                var ancestroP = t.ParentNode;
                while (ancestroP != null &&
                       !(ancestroP.LocalName == "p" && ancestroP.NamespaceURI == WordprocessingMlNs))
                    ancestroP = ancestroP.ParentNode;

                if (ReferenceEquals(ancestroP, parrafo))
                    resultado.Add(t);
            }

            return resultado;
        }

        private static void SetPreserveSpace(XmlNode nodoTexto)
        {
            if (nodoTexto.Attributes == null) return;
            const string xmlNs = "http://www.w3.org/XML/1998/namespace";
            var attr = nodoTexto.Attributes["xml:space"];
            if (attr == null)
            {
                attr = nodoTexto.OwnerDocument.CreateAttribute("xml", "space", xmlNs);
                nodoTexto.Attributes.Append(attr);
            }
            attr.Value = "preserve";
        }

        private static string AplicarReemplazos(string texto, IDictionary<string, string> reemplazos)
        {
            // Reemplazo explícito de {{TAG}} por su valor (case-insensitive en el nombre).
            foreach (var kv in reemplazos)
            {
                var patron = "{{" + Regex.Escape(kv.Key) + "}}";
                texto = Regex.Replace(texto, patron, kv.Value ?? string.Empty,
                                      RegexOptions.IgnoreCase);
            }
            return texto;
        }

        /// <summary>
        /// Sustituye el contenido de las imágenes referenciadas por los encabezados
        /// por el nuevo logo. Reemplaza todas las imágenes de header (membrete).
        /// </summary>
        private static void ReemplazarLogo(Package package, byte[] logo)
        {
            var headerParts = package.GetParts().Where(p =>
                p.ContentType.IndexOf("wordprocessingml.header", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            var imagenesReemplazadas = new HashSet<string>();

            foreach (var header in headerParts)
            {
                foreach (var rel in header.GetRelationshipsByType(RelationshipNs))
                {
                    var targetUri = PackUriHelper.ResolvePartUri(header.Uri, rel.TargetUri);
                    if (!package.PartExists(targetUri)) continue;
                    if (!imagenesReemplazadas.Add(targetUri.ToString())) continue;

                    var imgPart = package.GetPart(targetUri);
                    using (var stream = imgPart.GetStream(FileMode.Create, FileAccess.Write))
                        stream.Write(logo, 0, logo.Length);
                }
            }
        }
    }
}
