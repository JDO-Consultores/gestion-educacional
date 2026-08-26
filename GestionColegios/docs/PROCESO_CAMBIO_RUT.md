# Cambio de RUT (Traspaso)

Esta guía explica el proceso para **alumnos extranjeros** que obtienen su
**RUT chileno definitivo** y cómo se traspasa toda su información al nuevo
registro.

---

## ¿Cuándo se usa?

Un alumno fue inscrito originalmente con un identificador provisorio (por
ejemplo, RUT extranjero) y ahora obtuvo su **RUT definitivo**. Para mantener
el historial completo y separar el periodo previo del posterior, se crea un
**nuevo registro** y el **original** queda **archivado** (solo lectura).

---

## Paso a paso

1. Abre la **ficha del alumno** (con su RUT provisorio).
2. Presiona el botón **Cambio RUT**.
   - Está deshabilitado si el alumno ya tuvo un cambio de RUT previo.
3. Ingresa:
   - **RUT Nuevo** (RUT chileno definitivo, debe no existir en el sistema).
   - **Motivo** (opcional).
4. Confirma.

---

## ¿Qué hace el sistema?

Crea un **nuevo alumno** con el RUT definitivo y le **copia automáticamente**:

- Datos personales, contacto, salud y foto.
- **Matrículas activas** (con el mismo número, año, curso, estado y fecha).
- **Apoderados** vinculados.
- **Alergias** y **discapacidades** (con sus certificados).
- **Documentos** cargados.

Al mismo tiempo:

- El **registro original queda archivado** (solo lectura) y con estado
  **Traspasado por cambio de RUT**.
- Se guarda la relación entre ambos registros para poder navegar entre ellos.

---

## ¿Cómo se ve cada ficha después del cambio?

### Ficha nueva (con RUT definitivo)
Banner amarillo con el mensaje:
> *"Este registro fue creado por un cambio de RUT. RUT anterior: …"*
> con enlace **Ver ficha original**.

### Ficha original (archivada)
Banner rojo con el mensaje:
> *"Ficha archivada — Registro de solo lectura"*
> con enlace **Ver ficha con RUT definitivo**.

La ficha original no es editable; queda como histórico.

---

## Reglas importantes

- El RUT nuevo **no puede estar ya registrado** en el sistema.
- **No se permite** un segundo cambio de RUT sobre un alumno que ya tiene RUT
  anterior asignado.
- La **etiqueta Alumno Nuevo / Antiguo** se **conserva** del registro original
  para cada matrícula copiada.

---

## Diagrama

```
   Ficha original (RUT provisorio)
           ?
           ?  Botón "Cambio RUT"
           ?
           ?  Modal: RUT nuevo + Motivo
           ?
           ?
   ?? Se crea ficha NUEVA con RUT definitivo
   ?   (matrículas, apoderados, alergias,
   ?    discapacidades y documentos copiados)
   ?
   ?? La ficha ORIGINAL queda archivada
       (solo lectura, con enlace a la nueva)
```

---

## Guías relacionadas

- *Ficha del alumno*
- *Alumno Nuevo / Alumno Antiguo*