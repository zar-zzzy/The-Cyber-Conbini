# Visión General: The Cyber-Conbini

## 1. Concepto y Pilares de Diseño
**The Cyber-Conbini** es una experiencia inmersiva en primera persona que fusiona la estética nostálgica y relajante de una tienda de conveniencia japonesa (*Conbini*) bajo una lluvia nocturna en Tokio, con la resolución de retos de lógica de programación y automatización.

### Pilares Fundamentales:
1. **Posición Fija e Inmersión Contenida:** El jugador no camina ni combate. Se ubica de forma fija tras el mostrador de atención. Toda la interacción del mundo ocurre a través de su punto de vista y su consola.
2. **Pedagogía Visual y Contextual:** Cada concepto de programación resuelve una necesidad tangible del negocio (atender a un cliente, fijar el precio de un bento, discriminar entre un cliente y una mascota).
3. **Atmósfera Acogedora (ASMR & Lo-Fi):** Lluvia constante en los ventanales, luces de neón tenues reflejadas en el asfalto y el suelo, pitidos sutiles de la caja registradora y el tecleo de la terminal.
4. **Retroalimentación Determinística Inmediata:** Respuestas claras de éxito o error con pistas contextuales que guían al aprendiz sin frustrarlo.

---

## 2. Bucle de Juego (Core Loop)
```text
[Cliente / Evento Llega] 
        ↓
[Presentación del Reto en Pantalla de la Terminal]
        ↓
[El Jugador Escribe / Selecciona la Solución]
        ↓
[Presionar "Ejecutar" / Validación Interna C#]
        ↓
[Éxito: Reacción 3D (Luz verde, Sonido, Avance) / Error: Pista Didáctica]
        ↓
[Transición al Siguiente Reto o Cliente]
```

---

## 3. Plan de Progresión Curricular
1. **Nivel 1: Salida de Datos (`print / Console.WriteLine`)**
   - Reto: Mostrar mensaje de bienvenida al cliente en el display ("Irasshaimase / Bienvenido").
2. **Nivel 2: Variables y Tipos Básicos**
   - Reto: Asignar nombre del cajero, nombre del producto (`string`) y precio (`int`/`float`).
3. **Nivel 3: Operaciones Aritméticas**
   - Reto: Sumar artículos, calcular el vuelto/cambio y multiplicar por cantidad.
4. **Nivel 4: Estructuras Condicionales (`if / else`)**
   - Reto: Verificar si el cliente es mayor de edad para comprar tabaco, o no abrir la puerta automática si se detecta un gato callejero bajo la lluvia.
5. **Niveles Futuros (Post-MVP):**
   - Bucles para calcular el inventario de estanterías, funciones de caja y automatización de reabastecimiento.
