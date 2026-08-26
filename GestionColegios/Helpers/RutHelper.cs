namespace GestionColegios.Helpers
{
    /// <summary>
    /// Utilidades para formateo y limpieza de RUT chileno.
    /// </summary>
    public static class RutHelper
    {
        /// <summary>
        /// Formatea un RUT sin puntos a la notación con puntos y guión.
        /// Ejemplos:
        ///   "182602108"  ? "18.260.210-8"
        ///   "18260210-8" ? "18.260.210-8"
        ///   "9202938-7"  ? "9.202.938-7"
        /// Devuelve el valor original si no se puede formatear.
        /// </summary>
        public static string Formatear(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut))
                return rut;

            // Normalizar: quitar puntos, espacios; separar cuerpo y dígito verificador
            var limpio = rut.Replace(".", "").Replace(" ", "").ToUpperInvariant();
            string cuerpo, dv;

            if (limpio.Contains("-"))
            {
                var partes = limpio.Split('-');
                if (partes.Length != 2) return rut;
                cuerpo = partes[0];
                dv     = partes[1];
            }
            else if (limpio.Length >= 2)
            {
                cuerpo = limpio.Substring(0, limpio.Length - 1);
                dv     = limpio.Substring(limpio.Length - 1);
            }
            else
            {
                return rut;
            }

            // Formatear el cuerpo con puntos de millar
            if (!long.TryParse(cuerpo, out long numero))
                return rut;

            return string.Format("{0:N0}-{1}", numero, dv)
                         .Replace(",", ".");      // separador de miles local ? punto
        }

        /// <summary>
        /// Limpia el RUT para almacenamiento: sin puntos, con guión y dígito verificador en mayúscula.
        /// Ejemplo: "18.260.210-8" ? "18260210-8"
        /// </summary>
        public static string Limpiar(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut))
                return rut;

            return rut.Replace(".", "").Replace(" ", "").Trim().ToUpperInvariant();
        }
    }
}
