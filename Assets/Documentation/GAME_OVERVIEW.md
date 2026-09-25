# GDD breve — The Cyber-Conbini

## Concepto y objetivo

Juego educativo 3D situado en la caja de un conbini nocturno. El jugador atiende al primer cliente resolviendo instrucciones de sintaxis Python didáctica. La meta de la demo es completar los seis retos del Módulo 1, «Primer turno».

## Pilares de la demo

- **Punto de vista fijo:** el jugador permanece detrás del mostrador. `E` acerca la cámara a la terminal y `Escape` vuelve a la caja; el borrador escrito se conserva.
- **Aprendizaje contextual:** cada instrucción atiende una necesidad concreta de la tienda. La dificultad avanza de imprimir un literal a guardar e imprimir variables de texto y número.
- **Feedback claro:** la terminal indica el resultado, ofrece pista y reinicio; al acertar, el CRT, la luz de escáner y el cliente reaccionan visualmente.
- **Atmósfera nocturna:** cliente, productos y estantería visibles, iluminación de tienda y lluvia exterior. El audio ASMR es una intención de diseño, no una función presente en esta build.

## Bucle jugable

1. Leer el enunciado y el texto objetivo en la terminal.
2. Escribir una instrucción y pulsar **EJECUTAR**.
3. Leer la salida y el feedback. Si falla, consultar **PISTA**, corregir o usar **REINICIAR**.
4. Si acierta, pulsar **SIGUIENTE RETO** cuando exista otro reto en el catálogo.
5. En R6, la salida `450` y el feedback final cierran el Módulo 1. No hay R7.

## Progresión implementada

| Reto | Nombre | Concepto | Ejemplo válido |
| :--- | :--- | :--- | :--- |
| M1_R1 | Saludo inicial | `print()` de texto | `print("Bienvenido al Cyber-Conbini")` |
| M1_R2 | Inicio de turno | `print()` de texto | `print("Turno nocturno iniciado")` |
| M1_R3 | Producto en caja | `print()` de texto | `print("Onigiri de salmón")` |
| M1_R4 | Guardar cliente | Variable de texto | `cliente = "Aiko"` |
| M1_R5 | Mostrar cliente | Asignar e imprimir texto | `cliente = "Aiko"` y, en la siguiente línea, `print(cliente)` |
| M1_R6 | Precio del onigiri | Asignar e imprimir número | `precio_onigiri = 450` y, en la siguiente línea, `print(precio_onigiri)` |

Los datos viven en `Assets/Data/Challenges/Module1ChallengeCatalog.asset`. `ChallengeFlowController` gestiona el reto actual y `ChallengeValidator` valida solo los patrones permitidos en C#. No se ejecuta Python, no hay evaluación dinámica y la entrada está limitada antes de analizarla.

## Escena y presentación

`Assets/Scenes/Conbini_Main.unity` es la escena inicial de la build. La UI conserva el contador «Reto X de 6», enunciado, objetivo, campo de código, botones y consola. El zoom cambia la cámara, no la progresión del reto. El éxito activa el brillo CRT temporal mediante `MaterialPropertyBlock`; no altera el material compartido.

## Fuera del alcance actual

No hay movimiento libre, inventario, guardado, economía, operaciones aritméticas, condicionales, bucles ni nuevos módulos. Los sonidos ASMR aún deben seleccionarse o producirse; esta demo se presenta en silencio. La experiencia sonora y las mecánicas futuras requieren aprobación y pruebas propias.
