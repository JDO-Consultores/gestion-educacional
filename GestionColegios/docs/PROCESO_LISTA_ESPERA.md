# Lista de Espera (Cupos)

Esta guía explica cómo se controla la **capacidad de cada curso** y cómo
funciona la **lista de espera** cuando un curso ya no tiene cupos.

---

## ¿Cuándo una matrícula entra a Lista de Espera?

Al matricular a un alumno, el sistema revisa los cupos del curso:

- Si hay cupos ? la matrícula sigue su flujo normal (Pre-Matriculado o
  Matriculado, según los documentos).
- Si **no hay cupos** ? la matrícula queda en **? Lista de Espera**.

En el formulario de matrícula te aparecerá un aviso:

> *"Sin cupos disponibles · Quedará en LISTA DE ESPERA"*

---

## ¿Cómo se cuentan los cupos?

- Cada curso tiene una **capacidad** (la define el administrador).
- Si la capacidad está **vacía**, el curso es **sin límite** y nunca habrá lista
  de espera.
- Las matrículas **Anuladas** y las que ya están en **Lista de Espera** **no
  cuentan** como ocupadas.

**Ejemplo:** un curso con capacidad 30 y 30 alumnos en Matriculado +
Pre-Matriculado está **lleno**. Si llega un alumno nuevo, queda en Lista de
Espera.

---

## ¿Cómo "corre" la lista?

Cuando se **libera un cupo** (por ejemplo, al **anular** una matrícula del
curso), el sistema:

1. Toma al **primero en lista de espera** (el que entró antes).
2. Lo pasa a **?? Pre-Matriculado**.
3. Si ya tiene todos los documentos reglamentarios completos, lo promueve
   automáticamente a **?? Matriculado**.
4. Si todavía hay cupos disponibles, repite con el siguiente de la lista.

> El orden de la lista es por **fecha de matrícula**: primero quien se anotó
> antes.

---

## ¿Cómo libero un cupo?

Desde **Matrículas** (listado o historial), presiona **Anular** en la matrícula
que quieres liberar. Eso:

- Marca la matrícula como **?? Anulada**.
- Libera el cupo.
- Si hay alguien en lista de espera, **avanza automáticamente**.

---

## Diagrama

```
   Curso lleno                       Se anula una matrícula
        ?                                        ?
        ?                                        ?
  Nueva matrícula                Se libera un cupo
  ? ? Lista de Espera                    ?
                                         ?
                       El primero en la lista pasa a
                       ? ?? Pre-Matriculado
                       Si tiene documentos completos:
                       ? ?? Matriculado
                       (y sigue con el siguiente si hay más cupos)
```

---

## Guías relacionadas

- *Proceso general de matrícula*
- *Documentos del alumno*
- *Año Escolar y Cursos* (cómo se define la capacidad)