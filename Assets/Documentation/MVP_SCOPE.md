# Alcance de la demo del Módulo 1

Esta demo permite completar el primer turno desde una posición fija en la caja del conbini. La escena oficial es `Assets/Scenes/Conbini_Main.unity`, primera en Build Settings.

## Incluido

- Entorno 3D nocturno con caja registradora, producto, cliente, estantería, ventana y lluvia exterior.
- Cámara de caja y zoom reversible a la terminal (`E` para acercar, `Escape` para volver). El borrador del campo de código se conserva al volver.
- Catálogo de seis retos M1_R1–M1_R6 en orden, con pista, salida, feedback, reinicio y avance solo cuando existe el siguiente reto.
- Validador determinístico en C# para `print()` de un literal, asignación de una variable de texto y asignación seguida de `print()` de texto o número.
- Feedback visual: emisión CRT temporal, luz breve de escáner y movimiento sencillo del cliente al acertar.
- Una visita completa: entrada por puerta deslizante, elección de un onigiri, espera en caja durante los seis retos y salida con la compra tras resolver R6. Recorrido guiado con animaciones del paquete existente e IK; sin nuevos paquetes ni audio.
- Pruebas Edit Mode y Play Mode, y build Windows local.

## No incluido

- Audio provisional ni ASMR final. La búsqueda o creación de sonidos queda pendiente de aprobación.
- Movimiento libre, inventario, economía, guardado, multijugador o módulos 2 y 3.
- Ejecución real de Python, evaluación dinámica o procesos externos para validar respuestas.
- Aritmética, condicionales, bucles o retos posteriores a M1_R6.

## Criterio de demo

El jugador ve llegar al cliente, puede abrir la terminal, resolver los seis retos, recibir feedback visual y volver con `Escape` sin perder lo escrito. Resolver R6 inicia la despedida y devuelve la cámara a caja; el cierre del módulo aparece después de que el cliente sale con su compra, sin botón para un R7 inexistente. La visita no se repite automáticamente. El resultado debe compilar y ejecutarse en Windows sin errores de proyecto.
