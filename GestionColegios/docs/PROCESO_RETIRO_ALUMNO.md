# Retiro del Alumno

Esta guía explica cómo **retirar un alumno** del establecimiento, qué datos
debes ingresar y qué efectos tiene.

---

## ¿Cuándo retiro a un alumno?

Cuando un alumno deja el establecimiento de forma definitiva o por traslado.

> El retiro **no anula** automáticamente su matrícula del año en curso. Si
> quieres **liberar el cupo** del curso, además debes **anular la matrícula**
> desde el listado de Matrículas.

---

## Paso a paso

1. Abre la **ficha del alumno**.
2. Presiona el botón **Retirar** (solo está habilitado si el alumno está
   Vigente).
3. Completa:
   - **Causal de Retiro** (obligatorio).
   - **Fecha de Retiro** (obligatoria; exigida por MINEDUC).
   - **Observación** (opcional).
4. Confirma.

El sistema:

- Cambia el estado del alumno a **?? Retirado**.
- Guarda el retiro (causal, fecha, observación) para el historial.
- Registra la acción en el log.

---

## ¿Cómo se ve después de retirar?

En la cabecera de la ficha aparece una insignia con la fecha y la causal:

> ?? *Retirado el 12/07/2025 — Traslado de colegio*

---

## Si el alumno vuelve más adelante

Cuando un alumno retirado **regresa**, simplemente abres su ficha y presionas
**Nueva Matrícula**. El sistema lo reconoce como **reingreso** y le asigna un
**nuevo número de matrícula** conservando el anterior como referencia
histórica.

Ver la guía *Alumno retirado que reingresa*.

---

## Diagrama

```
   Ficha del alumno (Vigente)
           ?
           ?
    Botón "Retirar"
           ?
           ?  Modal: Causal + Fecha + Observación
           ?
           ?
    El alumno queda RETIRADO
    (y aparece la fecha/causal en la ficha)
```

---

## Guías relacionadas

- *Alumno retirado que reingresa*
- *Lista de espera (cupos)*
- *Ficha del alumno*