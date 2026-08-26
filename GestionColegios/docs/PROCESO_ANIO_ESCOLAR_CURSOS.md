# Año Escolar y Cursos

Esta guía explica cómo configurar los **años escolares** y los **cursos** del
establecimiento.

---

## Año Escolar

### Crear un año escolar
1. Menú **Administración ? Años Escolares**.
2. Botón **+ Nuevo Año Escolar**.
3. Completa:
   - **Año** (ej. 2026).
   - **Establecimiento**.
   - **Fecha de Inicio** y **Fecha de Término** (opcionales).
4. Guarda.

> No se permite **duplicar** el mismo año para el mismo establecimiento.
> Si existe un año desactivado con esos mismos datos, el sistema lo **reactiva**
> en lugar de crear uno nuevo.

### Marcar el año Activo
El año marcado como **Activo** es el que el sistema usa por defecto en el
dashboard y en las nuevas matrículas.

- Botón **Marcar como Activo** en el listado.
- Solo puede haber **un año activo** a la vez.
- No se puede activar un año que esté **Cerrado**.

### Cerrar / Reabrir el año
- Botón **Cerrar año** cuando finaliza el periodo (impide crear/editar cursos y
  matrículas asociadas).
- Botón **Reabrir año** para volver a habilitarlo.

> Para el **Cierre / Promoción** de los alumnos del año, ver la guía
> *Cierre de año (promoción)*.

---

## Cursos del año

### Crear un curso
1. Abre el **detalle del año escolar**.
2. Botón **+ Agregar Curso**.
3. Completa:
   - **Grado** (catálogo, p. ej. 1° Básico).
   - **Letra** (A, B, C…).
   - **Capacidad** (opcional pero recomendada para control de cupos).
   - **Profesor Jefe** (opcional).
4. Guarda.

> No se permite duplicar el mismo (Grado + Letra) en el mismo año.
> La letra se normaliza a mayúsculas.

### Editar un curso
Botón **Editar** en la fila del curso.

### Eliminar un curso
Botón **Eliminar** en la fila del curso. **No se puede eliminar** un curso que
tenga matrículas activas; primero hay que reasignar o anular esas matrículas.

---

## Sobre la **Capacidad** del curso

| Valor | Significado |
|-------|-------------|
| Número (ej. 30) | Activa el control de cupos y la lista de espera. |
| Vacío | Curso **sin límite**: nunca habrá lista de espera. |

Cuando un curso alcanza su capacidad, las nuevas matrículas quedan
automáticamente en **? Lista de Espera**. Ver *Lista de espera (cupos)*.

---

## Vista del Detalle del Año

Muestra:

- Cabecera con año, establecimiento, fechas y estado (Activo / Cerrado / Abierto).
- Tarjetas con totales: cursos, alumnos matriculados, fechas.
- Tabla de cursos agrupada por **Nivel de Enseñanza**, con:
  - Grado, Letra, **Capacidad**, **Matriculados** (con badge rojo si está
    lleno), **Profesor Jefe** y acciones.
- Botones de **Editar año**, **Cierre / Promoción** y **Cerrar / Reabrir año**.

---

## Diagrama

```
   [ Crear año escolar ]
            ?
            ?
   [ Crear cursos del año ]
            ?
            ?
   [ Matrícula de alumnos ]  (ver guía de matrícula)
            ?
            ?
   [ Cierre / Promoción ]    (ver guía de cierre)
            ?
            ?
   [ Cerrar año ]
```

---

## Guías relacionadas

- *Proceso general de matrícula*
- *Lista de espera (cupos)*
- *Cierre de año (promoción)*
- *Profesores Jefe*