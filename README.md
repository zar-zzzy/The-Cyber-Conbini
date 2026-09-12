# The Cyber-Conbini 🏪🌧️

> **Videojuego educativo 3D de simulación y automatización en un Conbini de Tokio.**

---

## 📖 Descripción Breve
**The Cyber-Conbini** es una experiencia inmersiva en primera persona donde el jugador asume el rol del encargado del turno nocturno en una tienda de conveniencia japonesa (*Conbini*) durante una noche lluviosa en Tokio. 

A diferencia de los simuladores tradicionales, el jugador no recorre libremente el espacio ni participa en combates. Su posición es fija detrás del mostrador y su herramienta de trabajo principal es una **terminal interactiva integrada en la caja registradora**. A través de ella, resuelve desafíos lógicos y de programación para atender clientes, automatizar procesos y mantener operativa la tienda dentro de una atmósfera relajante de estética *lo-fi* y sonido ambiente estilo *ASMR*.

---

## 🎯 Objetivo Educativo
Enseñar y reforzar los **fundamentos de la lógica de programación** de manera gradual y contextualizada. El estudiante aprende a través del ciclo de retroalimentación inmediata:
1. **Comprensión del problema** en el contexto real del negocio (ej. cobros, inventario, validación de acceso).
2. **Escritura y estructuración de código** estructurado o comandos de programación.
3. **Validación visual y sonora inmediata** del resultado en el entorno 3D (reacciones de la caja, del cliente o de la tienda).

**Habilidades abordadas en la progresión curricular:**
- Salida por terminal (`print()`, mensajes formateados).
- Variables y tipos de datos (nombres, precios, stock, flags booleanas).
- Operaciones aritméticas (cálculo de totales, impuestos, descuentos).
- Estructuras condicionales (`if / else`, control de flujo según cliente o stock).
- *Futuras expansiones:* Bucles (`for`, `while`), funciones y automatización de inventarios.

---

## 🚀 Estado del Proyecto
- **Fase Actual:** **Preproducción / MVP en desarrollo (Vertical Slice)**.
- **Versión de Motor:** Unity 6 (`6000.5.10f1`) - Universal Render Pipeline (URP).

---

## 🛠️ Stack Tecnológico
- **Motor de Videojuego:** Unity 3D (Unity 6 / URP 17.5.0)
- **Lenguaje de Desarrollo del Juego:** C# (.NET Standard / Unity Engine API)
- **Lenguaje Educativo del Jugador (Terminal de Caja):** **Python** (sintaxis didáctica: `print()`, variables, operaciones, `if/elif/else`)
- **Interfaz de Usuario:** Unity UI / TextMesh Pro
- **Herramientas de Asistencia y Desarrollo:** Google Antigravity + Unity MCP
- **Control de Versiones:** Git / GitHub

---

## 📦 Alcance del MVP (Vertical Slice)
El objetivo del MVP es validar el bucle jugable nuclear sin complejidades innecesarias:
1. **Entorno 3D mínimo:** Vista fija en primera persona detrás del mostrador del Conbini.
2. **Caja Registradora Interactiva:** Mostrador y caja con terminal de pantalla integrada.
3. **Reto Inicial:** Desafío didáctico de salida por pantalla (`print()`).
4. **Entrada de Usuario:** Campo de texto interactivo con botón "Ejecutar".
5. **Validación Determinística:** Validador en C# que analiza la solución esperada contra patrones permitidos (sin ejecución de código arbitrario).
6. **Sistema de Retroalimentación:** Mensajes de éxito, error y botón de pista contextual.
7. **Reacción del Entorno:** Feedback audiovisual mínimo (indicador lumínico de estado, sonido placeholder y/o reacción del cliente).

---

## 📜 Política de Assets y Registro de Licencias
- **Política de Costo Cero:** Se prohíbe estrictamente la descarga, uso o importación de assets de pago o de procedencia dudosa.
- **Fuentes Permitidas:**
  - Recursos y prototipos de modelado/audio propios.
  - Formas y materiales geométricos *placeholders* creados en Unity / ProBuilder.
  - Assets de dominio público o con licencias comerciales permisivas (CC0, MIT, Apache 2.0, Unity Standard Assets gratuitos autorizados).
- **Control y Atribución:** Todo recurso externo incorporado debe estar explícitamente documentado en [`Assets/Documentation/ASSET_LICENSES.md`](Assets/Documentation/ASSET_LICENSES.md) indicando autor, fuente original, fecha de integración y tipo de licencia.

---

## 📂 Documentación del Proyecto
- [Visión General del Juego (GAME_OVERVIEW.md)](Assets/Documentation/GAME_OVERVIEW.md)
- [Especificación y Alcance del MVP (MVP_SCOPE.md)](Assets/Documentation/MVP_SCOPE.md)
- [Registro de Licencias de Assets (ASSET_LICENSES.md)](Assets/Documentation/ASSET_LICENSES.md)
