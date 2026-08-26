# Certificados del Alumno (generación on-demand)

Esta guía explica cómo **generar certificados en Word** para un alumno y cómo
**administrar** las plantillas, los datos del establecimiento y los firmantes.

---

## ¿Qué certificados hay?

El sistema trae configurados 5 certificados:

| Código | Certificado |
|--------|-------------|
| `ALUMNO_REGULAR`    | Certificado de Alumno Regular |
| `MATRICULA`         | Certificado de Matrícula |
| `TRASLADO`          | Certificado de Traslado |
| `CAMBIO_APODERADO`  | Constancia Cambio de Apoderado |
| `TOMA_CONOCIMIENTO` | Toma de Conocimiento |

Cada uno se genera **bajo demanda** por alumno, en formato **Word (.docx)
editable**, reemplazando los *tags* de la plantilla con los datos reales.

---

## Generar un certificado

1. Abre la **ficha del alumno**.
2. Presiona el botón **Certificados** (arriba a la derecha).
3. Elige el **tipo de certificado**.
4. (Opcional) Cambia **quién firma** — por defecto se usa el firmante
   configurado para esa plantilla.
5. Presiona **Generar y Descargar**: se descarga el `.docx` listo.

---

## Administración (solo Administrador)

Menú **Administración ? Certificados**. Tiene tres pestañas:

### 1) Establecimiento
Datos del **membrete** que se inyectan en los certificados: nombre, RBD,
dirección, ciudad/región, teléfono, correo, sitio web y **logo** (PNG/JPG).
El logo reemplaza automáticamente la imagen del encabezado de las plantillas.

### 2) Plantillas
Por cada certificado puedes:
- **Subir/Reemplazar** el archivo Word (`.docx`) con los tags.
- **Descargar** el archivo Word actual (si ya hay uno cargado).
- Definir el **firmante por defecto**.
- **Activar/Desactivar** la plantilla.

### 3) Firmantes
Lista de personas que pueden firmar (Directora, Coordinador/a, etc.) con su
**cargo**. Se pueden crear, editar y desactivar.

---

## Tags disponibles para las plantillas Word

Escribe estos marcadores dentro del `.docx`. Se reemplazan al generar:

**Membrete / establecimiento**
- `{{COLEGIO_NOMBRE}}`, `{{COLEGIO_DIRECCION}}`, `{{COLEGIO_CIUDAD}}`,
  `{{COLEGIO_COMUNA}}`, `{{COLEGIO_REGION}}`,
  `{{COLEGIO_TELEFONO}}`, `{{COLEGIO_RBD}}`, `{{COLEGIO_EMAIL}}`, `{{COLEGIO_WEB}}`

**Alumno**
- `{{ALUMNO_NOMBRE}}`, `{{ALUMNO_RUT}}`, `{{ALUMNO_CURSO}}`, `{{ALUMNO_ANIO}}`

**Apoderado titular**
- `{{APODERADO_NOMBRE}}`, `{{APODERADO_RUT}}`

**Fecha / firmante**
- `{{FECHA}}` (fecha larga), `{{ANIO_ACTUAL}}`,
  `{{FIRMANTE_NOMBRE}}`, `{{FIRMANTE_CARGO}}`

> El **logo** no necesita tag: se reemplaza la imagen del encabezado por la
> cargada en el Mantenedor.

---

## Notas técnicas

- Las plantillas y el logo se guardan en la **base de datos**
  (`tbl_PlantillaCertificado`, `tbl_Establecimiento`).
- La generación usa `System.IO.Packaging` (incluido en .NET Framework), por lo
  que **no requiere Office/Word instalado en el servidor**.
- Script de base de datos: `Scripts/SQL/2026_Certificados.sql`.

---

## Guías relacionadas

- *Documentos del alumno*
- *Ficha del alumno*
