# Registro de Licencias y Atribuciones de Assets

Este documento contiene el historial y registro obligatorio de todos los recursos (modelos 3D, texturas, audios, fuentes) utilizados en **The Cyber-Conbini**.

---

## 1. Política de Uso de Recursos
- **Cero Costo:** Solo se integran recursos gratuitos cuya licencia permita su uso en el juego, respetando las condiciones particulares de cada fuente. "Gratis" no significa dominio público.
- **Sin Assets de Pago:** No está permitido integrar ningún paquete comercial o de procedencia dudosa.
- **Placeholders Prioritarios:** Durante las etapas de preproducción y MVP se priorizan primitivas 3D de Unity, materiales nativos con colores planos y efectos procedurales o generados en motor.

---

## 2. Inventario de Recursos

| Tipo de Recurso | Nombre / Archivo | Autor / Creador | Fuente / Enlace | Licencia | Estado en Proyecto |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Modelos 3D** | Primitivas y cubos de mostrador | Equipo de Desarrollo | Generado en Unity | Propia / N/A | Prototipo MVP |
| **Fuentes** | Fuentes por defecto TMP (Liberation Sans) | Unity Technologies | Integrada en TextMesh Pro | SIL Open Font License | Base UGUI |
| **Audio** | Sin efectos de sonido en la demo actual | N/A | N/A | N/A | Los sonidos provisionales sintetizados se retiraron; selección ASMR pendiente |
| **Audio Música**| Lluvia y ambiente ASMR | Por definir | Por definir | Por verificar | Pendiente; no hay pista incluida en la demo |
| **Personajes 3D** | `Assets/CharacterPack Lowpoly (FREE)` | EMD Assets / elvismd | [Character Pack - Lowpoly FREE](https://assetstore.unity.com/packages/3d/characters/humanoids/character-pack-lowpoly-free-221766) | Standard Unity Asset Store EULA | Cliente integrado en `Conbini_Main` con reacción visual y animación de reposo |
| **Comida 3D** | `Assets/ithappy/Food_Free` | ithappy | [Food FREE - Low Poly 3D Models Pack](https://assetstore.unity.com/packages/3d/props/food/food-free-low-poly-3d-models-pack-260726) | Standard Unity Asset Store EULA | Tres productos integrados en `Conbini_Main` |
| **Animaciones** | `Assets/Kevin Iglesias/Human Animations` | Kevin Iglesias | [Human Basic Motions FREE](https://assetstore.unity.com/packages/3d/animations/human-basic-motions-free-154271) | Standard Unity Asset Store EULA | Animación de reposo del cliente integrada en `Conbini_Main` |
| **Estanterías 3D** | `Assets/LowPolyMetalRack` | DigitalHakka | [Low Poly Metal Rack](https://assetstore.unity.com/packages/3d/props/furniture/low-poly-metal-rack-213045) | Standard Unity Asset Store EULA | Rack integrado en `Conbini_Main` |
| **Muebles 3D** | `Assets/Low Poly Furniture` | Gobormu | [Low Poly Simple Furniture FREE](https://assetstore.unity.com/packages/3d/props/furniture/low-poly-simple-furniture-free-240197) | Standard Unity Asset Store EULA | Importado; no integrado en la escena principal |
| **Partículas** | `Assets/Rain Particles` | Game Seed Assets | [Rain Particles](https://assetstore.unity.com/packages/vfx/particles/rain-particles-351846) | Standard Unity Asset Store EULA | Lluvia exterior integrada en `Conbini_Main` |
| **Caja registradora 3D y textura** | `Assets/Art/Models/Props/CashRegister_Wayneer`; prefab y material propios en `Assets/Prefabs/CashRegister_Wayneer.prefab` y `Assets/Art/Materials/Mat_CashRegister_Wayneer.mat` | Wayneer | [Low Poly Cash Register](https://sketchfab.com/3d-models/low-poly-cash-register-42f7f90246e440f59438ad22b866f84e) | Creative Commons Attribution (CC BY; versión no especificada en la ficha) | Prefab integrado en `Conbini_Main` |

**Atribución obligatoria de la caja:** "Low Poly Cash Register" por Wayneer, [Sketchfab](https://sketchfab.com/3d-models/low-poly-cash-register-42f7f90246e440f59438ad22b866f84e), licencia CC Attribution. En este proyecto se creó un prefab con escala adaptada y un material URP que reutiliza la textura original; el FBX y la textura no se modificaron.

Los seis paquetes de Unity Asset Store se registran bajo la [EULA estándar de Unity](https://unity.com/legal/as-terms), no bajo CC0 ni una licencia de código abierto. La EULA distingue incorporar assets en un juego de redistribuirlos como recursos independientes. Las fichas públicas de Character Pack, Human Basic Motions y Low Poly Metal Rack muestran además «Extension Asset» como tipo de licencia; antes de compartir los archivos fuente, confirmar los requisitos de acceso o asiento aplicables a cada colaborador. El modelo de Wayneer figura públicamente como «CC Attribution», sin versión concreta indicada en la ficha consultada.

---

## 3. Protocolo para Nuevas Incorporaciones
Antes de agregar cualquier asset al proyecto:
1. Validar que la licencia permita la integración y distribución del juego; distinguir este permiso de la redistribución de los archivos fuente del asset.
2. Añadir la fila correspondiente en la tabla anterior especificando origen y autor.
3. Conservar el archivo de licencia de origen si existe y registrar un enlace verificable en esta tabla.
