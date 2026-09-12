# Alcance del MVP (Vertical Slice)

## 1. Definición del MVP
El Minimum Viable Product (MVP) consiste en un corte vertical funcional y enfocado que demuestra la viabilidad de la mecánica central de aprendizaje y la atmósfera inmersiva sin sobrecargar el desarrollo con mecánicas accesorias.

---

## 2. Elementos Incluidos en el MVP
1. **Entorno 3D Minimalista:**
   - Mostrador de caja básico con materiales placeholder en URP.
   - Ventanal con lluvia de fondo sugerida (iluminación nocturna y reflejos tenues).
   - Cámara en primera persona estática orientada hacia la caja y el mostrador de clientes.
2. **Terminal de la Caja Registradora:**
   - Monitor 3D en el mostrador con lienzo (World Space Canvas o Screen Space interactivo) usando TextMesh Pro.
   - Panel de consigna del reto con texto descriptivo claro.
   - InputField para que el jugador escriba o complete su respuesta.
   - Botón interactivo "Ejecutar / Validar".
3. **Reto Inicial (Nivel 1):**
   - Reto de salida: Imprimir mensaje en el display (ejemplo: `print("Bienvenido")` o equivalente definido para el nivel).
4. **Sistema de Validación Determinística en C# (Simulador Didáctico de Python):**
   - Módulo validador modular programado en C# que analiza el input en sintaxis Python del jugador contra patrones y respuestas válidas aceptadas (eliminando espacios redundantes y contemplando variantes entre comillas simples/dobles, etc.).
   - **Cero código arbitrario:** No se ejecuta código dinámico ni intérpretes externos por razones de seguridad, estabilidad y simplicidad arquitectónica.
5. **Ciclo de Feedback:**
   - Feedback de éxito: Mensaje positivo en terminal + luz de terminal en verde + sonido de caja registradora o avance del cliente placeholder.
   - Feedback de error: Mensaje explicativo del fallo + botón/pista contextual accesible.

---

## 3. Elementos Explícitamente Excluidos del MVP
- Movimiento libre del jugador o navegación de primera/tercera persona.
- Física compleja, colisiones dinámicas avanzadas o ragdolls.
- Sistema de inventario persistente o economía profunda.
- IA compleja de peatones o tráfico exterior.
- Modos multijugador o servicios backend online.
- Intérpretes dinámicos o ejecución de código arbitrario no controlado en runtime.
