# GDD breve — The Cyber-Conbini

## Concepto y objetivo

Juego educativo 3D situado en la caja de un conbini nocturno. El jugador atiende al primer cliente resolviendo instrucciones de sintaxis Python didáctica. La meta de la demo es completar los seis retos del Módulo 1, «Primer turno».

## Pilares de la demo

- **Punto de vista fijo:** el jugador permanece detrás del mostrador. `E` acerca la cámara a la terminal y `Escape` vuelve a la caja; el borrador escrito se conserva.
- **Aprendizaje contextual:** cada instrucción atiende una necesidad concreta de la tienda. La dificultad avanza de imprimir un literal a guardar e imprimir variables de texto y número.
- **Feedback claro:** la terminal indica el resultado, ofrece pista y reinicio; al acertar, el CRT, la luz de escáner y el cliente reaccionan visualmente.
- **Atmósfera nocturna:** cliente, productos y estantería visibles, iluminación de tienda y lluvia exterior. El audio ASMR es una intención de diseño, no una función presente en esta build.

## Bucle jugable

1. Pulsar **COMENZAR TURNO** en el inicio y ver entrar al cliente por la puerta automática: elige un onigiri, lo lleva a caja y lo deja en el mostrador. La terminal permite resolver los retos cuando está listo para ser atendido.
2. Leer el enunciado y el texto objetivo en la terminal.
3. Escribir una instrucción y pulsar **EJECUTAR**.
4. Leer la salida y el feedback. Si falla, consultar **PISTA**, corregir o usar **REINICIAR**.
5. Si acierta, pulsar **SIGUIENTE RETO** cuando exista otro reto en el catálogo. El cliente espera sin límite de tiempo.
6. En R6, la salida `450` registra la compra. La cámara vuelve suavemente a caja; el cliente recoge su onigiri y sale. Solo entonces la terminal muestra «Módulo completado». No hay R7 ni un nuevo cliente automático. El recibo de turno completado permite **JUGAR DE NUEVO** o **VOLVER AL INICIO**.

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

`CustomerVisitController` coordina una única visita mediante puntos de recorrido, puerta deslizante y animaciones humanoides de reposo, caminar y giro del paquete Human Basic Motions ya importado. La recogida y entrega usan IK de mano sobre un onigiri procedural propio; no hay inventario ni simulación física del producto. Resolver todos los retos emite un evento una sola vez; finalizar el aprendizaje y terminar la salida son estados distintos. Durante la despedida se bloquea temporalmente el zoom de entrada para mantener visible la salida, y después se presenta el recibo de turno completado.

## Escena y presentación

`Assets/Scenes/Conbini_Main.unity` es la escena inicial de la build. La UI está integrada en la pantalla física de la caja mediante un Canvas en espacio de mundo, con paleta inspirada en Tokyo Night y brillo reducido. Conserva el contador «Reto X de 6», enunciado, objetivo, campo de código, botones y consola. E encuadra el monitor de frente y enfoca el campo de código; Escape vuelve a caja conservando el borrador. El zoom cambia la cámara, no la progresión del reto. El éxito activa el brillo CRT temporal mediante `MaterialPropertyBlock`; no altera el material compartido.

## Navegación de la visita

El Canvas independiente UI_VisitWindows incorpora inicio, guía, pausa, pista y recibo final. **MENÚ**, visible en caja, abre la pausa; reiniciar turno o volver al inicio requiere confirmación. Escape mantiene su función de volver desde el zoom y cierra las ventanas secundarias cuando están abiertas. La pista usa el reto activo y conserva el borrador. Los reinicios recargan toda la visita. Detalles en [UI_VISIT_WINDOWS.md](UI_VISIT_WINDOWS.md).

## Fuera del alcance actual

No hay movimiento libre, inventario, guardado, economía, operaciones aritméticas, condicionales, bucles ni nuevos módulos. Los sonidos ASMR aún deben seleccionarse o producirse; esta demo se presenta en silencio. La experiencia sonora y las mecánicas futuras requieren aprobación y pruebas propias.
