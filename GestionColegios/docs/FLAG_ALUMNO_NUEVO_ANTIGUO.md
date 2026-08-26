# Alumno Nuevo / Alumno Antiguo

Esta guía explica qué significan las etiquetas **Alumno Nuevo** y **Alumno
Antiguo** que aparecen en la matrícula y en la ficha.

---

## ¿Qué significa cada etiqueta?

| Etiqueta | Significado |
|----------|-------------|
| ?? **Alumno Nuevo** | Es la **primera matrícula** del alumno en el establecimiento. |
| ?? **Alumno Antiguo** | El alumno **ya tuvo al menos una matrícula** anterior (en cualquier año). |

---

## ¿Cómo se decide?

La regla es simple: **¿el alumno tiene alguna matrícula anterior?**

- **No** ? ?? Alumno Nuevo.
- **Sí** ? ?? Alumno Antiguo.

> El estado queda **fijo** en cada matrícula al momento de crearla. No cambia
> después, aunque el alumno se retire y reingrese.

---

## Casos especiales

| Caso | Resultado |
|------|-----------|
| Es la primera vez que matriculas a este alumno | ?? Alumno Nuevo |
| El alumno ya tuvo matrículas en años anteriores | ?? Alumno Antiguo |
| El alumno se retiró y ahora vuelve (reingreso) | ?? Alumno Antiguo (porque ya había tenido matrícula) |
| Cambio de RUT (extranjero con RUT chileno) | Se **conserva** la etiqueta que tenía cada matrícula original |

---

## ¿Dónde aparece esta etiqueta?

### En el formulario de matrícula
Una insignia te indica antes de guardar si el alumno será Nuevo o Antiguo.

### En el listado de Matrículas
- Una columna **Tipo** con la etiqueta.
- Puedes **filtrar** por Nuevo o Antiguo.
- En la parte superior verás dos tarjetas-resumen: **Alumnos Nuevos** y
  **Alumnos Antiguos** del año.

### En la ficha del alumno
Aparece como insignia en la cabecera.

---

## Resumen visual

```
   ¿Es la primera matrícula del alumno?
            ?
       SÍ ? ?? Alumno Nuevo
       NO ? ?? Alumno Antiguo
```

---

## Guías relacionadas

- *Proceso general de matrícula*
- *Alumno retirado que reingresa*
- *Cambio de RUT (traspaso)*