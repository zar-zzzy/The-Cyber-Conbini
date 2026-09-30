# Ventanas de visita — integración funcional

## Estado

Las ventanas de `UI_VisitWindows` están conectadas mediante `VisitWindowController`.
La escena conserva los paneles ocultos en edición; al entrar en Play Mode el controlador muestra INICIO.
No hay que activar hijos manualmente ni añadir listeners en el Inspector: se registran al arrancar y se retiran al destruir el controlador.

## Flujo conectado

| Acción | Resultado |
| --- | --- |
| COMENZAR TURNO | Oculta inicio y libera la entrada del cliente, retenida hasta ese momento. |
| CÓMO JUGAR / VOLVER | Abre la guía y regresa al inicio sin iniciar la visita. |
| E | Acerca la cámara a la terminal durante el turno. |
| Escape desde terminal | Vuelve a caja conservando el borrador. |
| MENÚ desde caja | Abre pausa y congela el tiempo de la visita. Se oculta durante el zoom y los modales. |
| CONTINUAR | Cierra pausa y restaura la velocidad previa. |
| REINICIAR TURNO | Abre confirmación; confirmar recarga la escena y comienza una visita nueva. |
| VOLVER AL INICIO desde pausa | Abre confirmación; confirmar recarga la escena mostrando inicio. |
| CANCELAR | Regresa a pausa sin tocar progreso ni borrador. Recibe el foco inicial en ambas confirmaciones. |
| PISTA | Muestra `CurrentChallenge.Hint` en un panel compacto; no copia ni modifica datos de retos. |
| X / VOLVER A LA TERMINAL | Cierra la pista conservando el borrador y devuelve el foco al campo si se está en el zoom. |
| TURNO COMPLETADO | Se abre automáticamente solo cuando `CustomerVisitController.IsModuleClosed` es verdadero, tras recoger el producto, salir y cerrar la puerta. |
| JUGAR DE NUEVO | Recarga la visita y la inicia desde cero. |
| VOLVER AL INICIO desde el recibo | Recarga la escena mostrando inicio. |
| SALIR | Cierra la aplicación en escritorio; en el Editor detiene Play Mode. Oculto en otros destinos. |

Escape cierra PISTA, vuelve de la guía al inicio, cancela una confirmación o continúa desde pausa.
**Escape nunca abre pausa desde caja.** Los modales consumen Escape para que no cambie también la cámara.
REINICIAR de la terminal sigue repitiendo el reto actual; REINICIAR TURNO descarta toda la visita.

## Responsabilidades técnicas

- `VisitWindowController`: navegación, foco, bloqueo modal, pausa y recarga. Referencias serializadas en el Canvas independiente.
- `CustomerVisitController.waitForBegin`: activado en Conbini_Main; `BeginVisit()` libera la entrada una sola vez. Otras escenas conservan el inicio automático por defecto.
- `TerminalUIController.HintRequested`: envía la pista activa al coordinador. Sin suscriptor conserva su presentación anterior.
- `TerminalUIController.SetModalInputBlocked`: evita ejecutar, reiniciar o avanzar retos detrás de una ventana.
- `TerminalExperienceController.SetUIInputBlocked`: bloquea E/Escape y peticiones de zoom mientras haya un modal, independientemente del bloqueo de entrada durante la despedida.
- Un CanvasGroup añadido al Canvas físico bloquea navegación y raycasts de la terminal detrás de los modales. No se reemplaza su Canvas ni se altera el diseño del monitor.
- Pausa, confirmaciones y pista guardan/restauran `Time.timeScale`. El inicio retiene al cliente sin congelar la lluvia. El cambio de deltaTime se aplica al siguiente fotograma de Unity.
- La recarga es asíncrona y bloquea clics repetidos. La intención de comenzar se consume una sola vez en la nueva instancia y se limpia al iniciar una sesión.
- No se añaden eventos persistentes de Inspector: cero listeners persistentes es normal; las conexiones son de ejecución.

## Presentación actual — YORU 24H

Referencia 1024 × 768, Canvas Overlay orden 100, tipografía Liberation Sans SDF del proyecto.
Paneles laterales verde profundo con marcos biselados Kenney, rótulos crema y acentos ámbar. Botones con relieve, pequeñas fijaciones y contraste de estados; confirmación destructiva en terracota.
La pista queda abajo a la derecha dejando visible el código. Los estados hover, selección y pulsación usan Color Tint.
El rediseño visual reutiliza los PNG Kenney aportados por el usuario, registrados en ASSET_LICENSES.md. No modifica materiales, retos, validador, paquetes ni configuración global.

## Verificación

Las pruebas `VisitWindowsPlayModeTests` cubren inicio/guía, bloqueo previo, pausa/restauración de tiempo,
cancelaciones, pista dinámica/borrador/foco, reinicio completo, vuelta al inicio y cierre tras la salida con repetición.
Las pruebas existentes de cámara y terminal comienzan ahora el turno en su preparación.
La validación de salida real de una build de escritorio requiere ejecutar esa build; SALIR en Editor detiene Play Mode.

## Capturas

En `Assets/Screenshots`, carpeta local ignorada por Git:
`ConnectedUI_Start.png`, `ConnectedUI_HowToPlay.png`, `ConnectedUI_Pause.png`,
`ConnectedUI_ConfirmRestart.png`, `ConnectedUI_ConfirmReturn.png`,
`ConnectedUI_Hint.png` y `ConnectedUI_Completed.png`.

## Resultado de la comprobación (2026-09-30)

- Unity Test Runner vía MCP: **23/23 Play Mode** y **109/109 Edit Mode**, sin fallos.
- Pruebas nuevas ejecutadas primero en rojo: 6/6 fallaban porque aún no existían conexiones; después pasan con la integración.
- Revisión adicional con raycasts sobre botones y eventos de teclado en Game View: E enfoca la terminal; Escape cierra primero la pista, conserva el borrador y después vuelve a caja; Escape también continúa desde pausa.
- La verificación automatizada de Game View necesitó temporalmente Application.runInBackground mientras Unity no tenía foco; el valor se restaura al finalizar y no se modifica ProjectSettings.
- R1–R6 resueltos también en la revisión de capturas, respetando la despedida antes del recibo.

- [Inicio](../Screenshots/ConnectedUI_Start.png)
- [Cómo jugar](../Screenshots/ConnectedUI_HowToPlay.png)
- [Pausa](../Screenshots/ConnectedUI_Pause.png)
- [Confirmar reinicio](../Screenshots/ConnectedUI_ConfirmRestart.png)
- [Confirmar vuelta al inicio](../Screenshots/ConnectedUI_ConfirmReturn.png)
- [Pista dinámica](../Screenshots/ConnectedUI_Hint.png)
- [Turno completado](../Screenshots/ConnectedUI_Completed.png)

SALIR comprobado en Editor: detiene Play Mode. Escape también comprobado desde la guía.

## Archivos de esta integración

- Assets/Scenes/Conbini_Main.unity
- Assets/Scripts/UI/VisitWindowController.cs y .meta (nuevo)
- Assets/Scripts/UI/TerminalUIController.cs
- Assets/Scripts/UI/TerminalExperienceController.cs
- Assets/Scripts/Gameplay/CustomerVisitController.cs
- Assets/Tests/PlayMode/VisitWindowsPlayModeTests.cs y .meta (nuevo)
- Assets/Tests/PlayMode/CustomerVisitPlayModeTests.cs
- Assets/Tests/PlayMode/PhysicalTerminalPlayModeTests.cs
- Assets/Tests/PlayMode/TerminalExperiencePlayModeTests.cs
- Assets/Documentation/GAME_OVERVIEW.md
- Assets/Documentation/MVP_SCOPE.md
- Assets/Documentation/UI_VISIT_WINDOWS.md (continuación de la entrega visual; conserva su .meta)


## Rediseño visual YORU 24H (2026-09-30)

La paleta sigue los rótulos, la fachada y la luz cálida del conbini: verde profundo #183C34, crema #EFE2BF, salvia #B5C1AB, ámbar #D6A462 y terracota #D79564. El monitor físico conserva su presentación anterior.

Se añaden una placa 24H, franjas de identidad, teclas con volumen y un recibo de papel con código de barras decorativo. Los nombres y referencias de los botones permanecen iguales, por lo que el flujo ya conectado se conserva. No se cambian scripts en esta fase.

Cinco sprites usados de Kenney_SciFi/PNG/Extra/Default:
- panel_glass_notches.png: marcos de ventana y tarjetas interiores.
- button_rectangle_depth.png: botones principales y acceso al menú.
- button_rectangle.png: botones secundarios y cabeceras.
- button_square_depth.png: teclas y placa 24H.
- panel_rectangle_screws.png: recibo.

Importación Sprite Single, nueve secciones con bordes, sin mipmaps ni compresión. Los PNG originales permanecen intactos; se generan los .meta del paquete proporcionado. Se preparó también un botón de Grey/Default durante la exploración, sin uso final en la escena.

### Revisión de esta fase

- Play Mode: navegación de inicio/guía, pausa, ambas confirmaciones y cancelación mediante raycast de botones.
- Entrada a terminal con E, apertura y cierre de pista, y R1–R6 completados.
- Seis pistas sin desbordamiento; altura máxima 82,21 con 90 disponibles.
- Capturas a 1024 × 768, lectura del recibo después de la salida.
- Las 132 pruebas indicadas arriba pertenecen a la integración funcional anterior; esta fase visual se revisa en Play Mode sin volver a ejecutar la suite.

### Comparación y capturas del estilo final

- [Antes](../Screenshots/ConnectedUI_Start.png) · [Inicio YORU](../Screenshots/YoruUI_Start.png)
- [Guía](../Screenshots/YoruUI_HowToPlay.png)
- [Pausa](../Screenshots/YoruUI_Pause.png)
- [Confirmar reinicio](../Screenshots/YoruUI_ConfirmRestart.png)
- [Confirmar inicio](../Screenshots/YoruUI_ConfirmReturn.png)
- [Pista](../Screenshots/YoruUI_Hint.png)
- [Recibo final](../Screenshots/YoruUI_Completed.png)

### Alcance comprobado de la fase visual

Comparación con la copia de escena previa al rediseño: 393 documentos existentes modificados, todos dentro de UI_VisitWindows; 304 documentos añadidos y ninguno eliminado. Ningún componente ajeno a las ventanas cambió.

Archivos tocados en esta fase: Conbini_Main.unity, UI_VISIT_WINDOWS.md, ASSET_LICENSES.md y .meta de importación de Kenney_SciFi. Las capturas y la copia previa están en Assets/Screenshots, ignorado por Git. Los scripts, pruebas y demás archivos pendientes del turno anterior se preservan.
