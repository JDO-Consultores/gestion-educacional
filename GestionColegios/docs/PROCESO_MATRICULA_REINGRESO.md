# Alumno Retirado que Reingresa

Esta guía explica qué pasa cuando un alumno que se había **retirado** vuelve a
**matricularse** en el establecimiento.

---

## En resumen

1. El alumno fue **retirado** en algún momento (se registró su retiro con
   causal y fecha).
2. Tiempo después, **vuelve** y se le crea una **nueva matrícula**.
3. El sistema le **asigna un número nuevo de matrícula** y conserva el anterior
   como **referencia histórica**.

---

## ¿Cómo retiro al alumno?

Desde la **ficha del alumno** ? botón **Retirar**.

Debes indicar:

- **Causal del retiro** (obligatorio).
- **Fecha del retiro** (obligatoria; es exigida por MINEDUC).
- **Observación** (opcional).

El alumno queda con estado **Retirado** y aparece la fecha y la causal en la
cabecera de su ficha.

> Retirar al alumno **no anula** su matrícula del año en curso. Si quieres
> liberar el cupo, debes **anular la matrícula** desde el listado de
> Matrículas. Ver *Lista de espera (cupos)*.

---

## ¿Cómo lo vuelvo a matricular?

Desde la **ficha del alumno** ? botón **Nueva Matrícula**. Se completa como
una matrícula normal (ver *Proceso general de matrícula*).

Como el alumno estaba **Retirado**, el sistema detecta el reingreso y:

- Le asigna un **nuevo número de matrícula**.
- Guarda el número anterior como **referencia histórica**.
- Cambia su estado a **Vigente**.

---

## ¿Cómo se ve el número de matrícula al reingresar?

**Ejemplo:**

```
Año 2025 — N° 2025-00042    (el alumno se retira)
Año 2026 — N° 2026-00067    (Ant: 2025-00042)
```

En el listado de matrículas y en el historial de la ficha verás el número
actual y, justo debajo, una etiqueta pequeña que dice **Ant: …** indicando el
número anterior. Eso te permite relacionar ambas matrículas.

---

## Casos especiales

### El alumno tiene "Matrícula Cancelada"
Si en el cierre del año anterior el alumno quedó marcado con **Matrícula
Cancelada**, el sistema **bloquea** el intento de matricularlo y muestra:

> ?? *"El alumno tiene la matrícula cancelada y está bloqueado para
> matricularse. Requiere autorización de un supervisor."*

Para habilitarlo, un **supervisor** debe **autorizarlo** ingresando sus
credenciales. Ver *Cierre de año (promoción)*.

### El curso no tiene cupos
Si el curso ya está lleno, la nueva matrícula queda en **Lista de Espera** y
avanzará cuando se libere un cupo. Ver *Lista de espera (cupos)*.

---

## Resumen visual

```
   El alumno fue RETIRADO
            ?
            ? Vuelve a matricularse
   ¿Quedó con Matrícula Cancelada?
       SÍ  ?  ?? Requiere autorización de supervisor
       NO  ?
   ¿Hay cupo en el curso?
       NO  ?  ? Lista de Espera
       SÍ  ?
   ¿Documentos completos?
       NO  ?  ?? Pre-Matriculado
       SÍ  ?  ?? Matriculado
```

---

## Guías relacionadas

- *Proceso general de matrícula*
- *Retiro del alumno*
- *Cierre de año (promoción)*
- *Lista de espera (cupos)*