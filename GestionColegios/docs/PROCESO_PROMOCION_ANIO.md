# Cierre de Año (Promoción)

Esta guía explica cómo **cerrar el año escolar** y marcar a cada alumno como
**Promovido** o **No Promovido**, y cómo eso afecta la matrícula del **año
siguiente**.

---

## ¿Qué resultados puede tener un alumno al cierre?

| Resultado | Significado | ¿Pasa al año siguiente? |
|-----------|-------------|--------------------------|
| ?? **Promovido** | Aprueba y avanza de grado. | ? Sí |
| ?? **No Promovido** | No avanza (repite). Requiere **Motivo + Decreto + Glosa**. | ? No |

### Condición especial sobre "Promovido"
Un alumno **Promovido** puede quedar además con **?? Matrícula Cancelada**, que
lo **bloquea** para matricularse el año siguiente. Solo se puede levantar con
**autorización de un supervisor** (clave).

---

## ¿Dónde se hace?

1. Menú **Administración ? Años Escolares**.
2. Abrir el **detalle** del año a cerrar.
3. Presionar **Cierre / Promoción**.

Verás una grilla con todos los alumnos matriculados del año, con su resultado
(vacío = pendiente).

---

## Registrar el resultado de un alumno

Por cada alumno, presiona **Registrar resultado** y elige:

### Caso A — Promovido
- Opcionalmente marcas **Matrícula Cancelada** si quieres bloquearlo del
  próximo año (por ejemplo, por comportamiento o convivencia escolar).

### Caso B — No Promovido
Es **obligatorio** completar los tres campos:

- **Motivo** — por ejemplo: *Inasistencia no justificada, Rendimiento, etc.*
- **Decreto** — número del decreto que ampara la decisión.
- **Glosa** — descripción o fundamento detallado.

> Si intentas guardar "No Promovido" sin alguno de esos tres campos, el sistema
> te lo impedirá y mostrará el aviso.

---

## Promoción masiva (todo un curso de una vez)

Si la mayoría del curso aprueba, hay un atajo:

1. En la vista de **Cierre / Promoción**, presiona **Promover curso completo**.
2. Elige el **curso** (solo aparecen los que tienen alumnos pendientes).
3. Confirma: todos los alumnos del curso sin resultado se marcan como
   **Promovido** en una sola operación.

Luego puedes ajustar manualmente los casos de "No Promovido".

---

## Autorizar a un alumno con Matrícula Cancelada

Cuando un alumno aparece con ?? **Matrícula Cancelada**:

1. Aparece un botón **Autorizar** (solo administradores). Lo encuentras:
   - En la **ficha del alumno**.
   - En la vista **Cierre / Promoción**.
2. Al presionarlo, se solicita:
   - **Usuario supervisor** (debe ser Administrador del sistema).
   - **Clave de supervisor**.
   - **Observación** (opcional).
3. Si las credenciales son correctas, se levanta el bloqueo y el alumno ya
   puede matricularse el año siguiente. Queda registro de quién lo autorizó.

---

## ¿Qué pasa al matricular el año siguiente?

| Situación del año anterior | Resultado al matricular el nuevo año |
|----------------------------|---------------------------------------|
| ?? **Promovido** | Se matricula como **Pre-Matriculado** (esperando documentos). |
| ?? **Promovido + Matrícula Cancelada** | **No matricular** (bloqueado). Requiere autorización de supervisor ? luego Pre-Matriculado. |
| ?? **Retirado** | Se le crea una **matrícula nueva** con N° nuevo (ver *Alumno retirado que reingresa*). |
| Sin cupo en el curso | Queda en **Lista de Espera**. |

### Ejemplo (paso del Año 2026 al 2027)

```
AÑO 2026                                  AÑO 2027
A) Promovido                       ?      ?? Pre-Matriculado
B) Promovido + Matrícula Cancelada ?      ?? No matricular (*)
C) Retirado                        ?      ?? Matrícula nueva
D) Curso lleno                     ?      ? Lista de Espera

(*) Se puede pasar a Pre-Matriculado con autorización de supervisor.
```

---

## Estados del próximo año (al matricular)

- ?? **Pre-Matriculado** — alumno promovido en espera de documentos reglamentarios.
- ?? **Matriculado** — todos los documentos reglamentarios subidos.
- ? **Lista de Espera** — sala llena; avanza al liberarse un cupo y pasa a
  Pre-Matriculado.

---

## Guías relacionadas

- *Proceso general de matrícula*
- *Alumno retirado que reingresa*
- *Lista de espera (cupos)*
- *Documentos del alumno*