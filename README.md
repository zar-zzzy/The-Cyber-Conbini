# The Cyber-Conbini 🏪🌧️

> **Videojuego educativo 3D de simulación y automatización en un Conbini de Tokio.**

---

## 📖 Descripción Breve
**The Cyber-Conbini** es una experiencia educativa en primera persona: el jugador atiende un turno nocturno desde la caja de una tienda de conveniencia japonesa.

El jugador permanece detrás del mostrador y resuelve seis retos de sintaxis Python didáctica en la **terminal de caja**. La escena combina cliente, productos, lluvia exterior y respuestas visuales. La dirección sonora busca ASMR, pero esta versión se entrega sin audio mientras se seleccionan sonidos adecuados.

---

## 🎯 Objetivo Educativo
Enseñar y reforzar los **fundamentos de la lógica de programación** de manera gradual y contextualizada. El estudiante aprende a través del ciclo de retroalimentación inmediata:
1. **Comprensión del problema** en el contexto real del negocio (ej. cobros, inventario, validación de acceso).
2. **Escritura y estructuración de código** estructurado o comandos de programación.
3. **Validación inmediata** con texto y reacciones visuales de la caja y del cliente.

**En el Módulo 1:** `print()` con literales, asignación de texto a una variable, impresión de esa variable y asignación/impresión de un número. Aritmética, condicionales, bucles e inventario quedan para módulos futuros.

---

## 🚀 Estado del Proyecto
- **Fase actual:** demo del Módulo 1 con seis retos jugables y build Windows.
- **Versión de Motor:** Unity 6 (`6000.5.10f1`) - Universal Render Pipeline (URP).

---

## 🛠️ Stack Tecnológico
- **Motor de Videojuego:** Unity 3D (Unity 6 / URP 17.5.0)
- **Lenguaje de Desarrollo del Juego:** C# (.NET Standard / Unity Engine API)
- **Lenguaje Educativo del Jugador (Terminal de Caja):** subconjunto didáctico de sintaxis Python (`print()` y variables)
- **Interfaz de Usuario:** Unity UI / TextMesh Pro
- **Automatización de pruebas y editor:** Unity Test Framework y Unity MCP
- **Control de Versiones:** Git / GitHub

---

## 📦 Alcance del MVP (Vertical Slice)
El objetivo del MVP es validar el bucle jugable nuclear sin complejidades innecesarias:
1. **Escena principal:** `Assets/Scenes/Conbini_Main.unity`, índice 0 en Build Settings.
2. **Interacción:** `E` acerca la cámara a la terminal; `Escape` vuelve a la caja sin borrar el borrador.
3. **Seis retos ordenados:** mensajes con `print()`, variable `cliente` y precio numérico `precio_onigiri`.
4. **Validación determinística:** C# reconoce patrones permitidos; no ejecuta Python ni código arbitrario.
5. **Feedback:** consola, pista, reinicio, avance, brillo CRT, luz breve de escáner y reacción simple del cliente.
6. **Audio:** sin sonidos provisionales. La selección ASMR se hará en una fase posterior.

---

## 📜 Política de Assets y Registro de Licencias
- **Política de Costo Cero:** Se prohíbe estrictamente la descarga, uso o importación de assets de pago o de procedencia dudosa.
- **Fuentes permitidas:** recursos propios y assets gratuitos con permisos compatibles con el uso previsto; cada licencia y atribución debe verificarse individualmente.
- **Control y Atribución:** Todo recurso externo incorporado debe estar explícitamente documentado en [`Assets/Documentation/ASSET_LICENSES.md`](Assets/Documentation/ASSET_LICENSES.md) indicando autor, fuente original, fecha de integración y tipo de licencia.

---

## 📂 Documentación del Proyecto
- [Visión General del Juego (GAME_OVERVIEW.md)](Assets/Documentation/GAME_OVERVIEW.md)
- [Especificación y Alcance del MVP (MVP_SCOPE.md)](Assets/Documentation/MVP_SCOPE.md)
- [Registro de Licencias de Assets (ASSET_LICENSES.md)](Assets/Documentation/ASSET_LICENSES.md)
- [Créditos de la demo (CREDITS.md)](Assets/Documentation/CREDITS.md)
- [Capturas de la demo](Assets/Documentation/Screenshots/)

La build local se genera en `Builds/Windows/TheCyberConbini.exe` y no se incluye en Git.
