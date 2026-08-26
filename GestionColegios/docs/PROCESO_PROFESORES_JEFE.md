# Profesores Jefe

Esta guía explica cómo gestionar **profesores jefe** y asignarlos a los cursos
del establecimiento.

---

## Datos del profesor

Cada profesor tiene:

- **RUT** (único en el sistema).
- **Nombre** y **Apellido**.
- **Email** y **Teléfono**.
- **Estado** (Activo / Inactivo / etc.).
- **Vigente** (sí/no).

---

## Operaciones

### Crear profesor
1. Menú **Administración ? Profesores Jefe**.
2. Botón **+ Nuevo Profesor**.
3. Completa los datos y guarda.

> El RUT se normaliza (sin puntos, mayúsculas) y se valida que no esté
> duplicado.

### Editar profesor
Botón **Editar** en la fila del profesor.

### Inactivar profesor
Cambia su estado o desmárcalo como vigente. Los cursos donde aparecía
asignado conservan la referencia histórica, pero el profesor **deja de aparecer
en los selectores** de cursos nuevos.

---

## Asignar profesor a un curso

Se hace al **crear o editar un curso** (ver *Año Escolar y Cursos*), eligiendo
el profesor en el campo **Profesor Jefe**.

- Un profesor puede ser jefe de **varios cursos** a la vez.
- No es obligatorio asignar profesor; un curso puede quedar **"Sin asignar"**.

---

## Visualización

### Listado de Profesores Jefe
Muestra RUT, nombre, email, teléfono, estado y la **cantidad de cursos
asignados**.

### Detalle del Año Escolar
En cada curso se muestra el profesor jefe asignado o **"Sin asignar"**.

### Ficha del alumno
Si el alumno tiene matrícula vigente, se muestra el **Profesor Jefe** del
curso y su **email** en la cabecera.

---

## Guías relacionadas

- *Año Escolar y Cursos*
- *Ficha del alumno*