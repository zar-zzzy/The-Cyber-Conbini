# Acuerdos de trabajo — The Cyber-Conbini

Este archivo orienta a cualquier chat que trabaje en este repositorio. La solicitud actual del usuario define el alcance de cada tarea; no conviertas ideas futuras en trabajo autorizado.

## Visión compartida

- Es un juego educativo 3D situado en la caja de un conbini nocturno. La tienda, el cliente, la lluvia y la terminal deben sentirse como partes de una misma experiencia; la ambientación no debe dificultar la lectura ni el uso de la terminal.
- Conserva el bucle jugable existente del Módulo 1 y sus seis retos, salvo que el usuario pida expresamente cambiarlo. No se ejecuta Python real: el validador reconoce patrones permitidos en C#.
- La dirección sonora buscada es ASMR, tranquila y creíble para la tienda. La demo actual no contiene audio; no reintroduzcas sonidos provisionales por iniciativa propia.
- Consulta `Assets/Documentation/GAME_OVERVIEW.md` para la visión y el estado jugable, y `Assets/Documentation/MVP_SCOPE.md` cuando una tarea afecte el alcance. Evita duplicar esos documentos aquí.

## Responsabilidades y coordinación

- Programación: scripts, arquitectura, pruebas e integración técnica. No rediseñes la tienda ni el audio fuera del encargo concreto.
- Diseño visual: composición del conbini, iluminación, utilería y búsqueda de assets. No alteres mecánicas, textos de retos o scripts sin coordinarlo con el usuario.
- Sonido: investigar, proponer o crear sonidos ASMR y su integración cuando se autorice. No añadas audio solo para llenar silencios.
- Estas son áreas de trabajo, no permisos automáticos. El usuario decide los cambios de fase y realiza los commits y el push manualmente; no ejecutes operaciones Git mutativas salvo petición explícita.
- Antes de editar, revisa `git status --short` y preserva los cambios locales ajenos. Cada chat tiene su propio historial y puede usar un checkout o worktree distinto: confirma el entorno y no presupongas que ve los cambios no confirmados de otro chat.
- `Assets/Scenes/Conbini_Main.unity` es la escena principal y un archivo compartido especialmente sensible. Si otra tarea está editándola, coordina con el usuario antes de tocarla; no sobrescribas ni mezcles a ciegas cambios de escena. Mantén los archivos `.meta` asociados a cualquier asset nuevo.

## Assets y calidad

- Antes de incorporar un recurso externo, verifica origen, autor, licencia y permiso de uso; registra la incorporación en `Assets/Documentation/ASSET_LICENSES.md` y la atribución correspondiente. No uses assets de pago ni de procedencia dudosa. Investigar opciones no equivale a importarlas.
- Para cambios de código, ejecuta las pruebas pertinentes y comprueba compilación cuando sea posible. Para cambios visuales o sonoros, verifica la escena en Play Mode y describe lo que todavía requiere revisión humana. En todos los casos, revisa el diff y ejecuta `git diff --check` antes de informar.
- Comunica archivos cambiados, pruebas realizadas, limitaciones y decisiones pendientes. Mantén los cambios de cada tarea acotados y reversibles; no modifiques paquetes, configuración global, materiales o contenido de otros roles sin necesidad justificada por la solicitud.
