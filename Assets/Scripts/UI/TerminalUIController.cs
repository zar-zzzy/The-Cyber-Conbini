using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CyberConbini.Gameplay;

namespace CyberConbini.UI
{
    /// <summary>
    /// Controlador principal para la interfaz de la Terminal de Caja de The Cyber-Conbini.
    /// Conecta los elementos de la interfaz de usuario con la lógica de validación del reto.
    /// </summary>
    public class TerminalUIController : MonoBehaviour
    {
        private static readonly int EmissionColorPropertyId = Shader.PropertyToID("_EmissionColor");

        [Header("Referencias de UI")]
        [Tooltip("Campo de texto donde el jugador introduce su código Python")]
        [SerializeField] private TMP_InputField inputField;

        [Tooltip("Texto donde se proyecta la consola o salida del programa")]
        [SerializeField] private TextMeshProUGUI consoleOutputText;

        [Tooltip("Título visible del módulo actual")]
        [SerializeField] private TextMeshProUGUI moduleText;

        [Tooltip("Contador visible del reto actual")]
        [SerializeField] private TextMeshProUGUI progressText;

        [Tooltip("Descripción visible del reto actual")]
        [SerializeField] private TextMeshProUGUI promptIntroText;

        [Tooltip("Texto objetivo visible del reto actual")]
        [SerializeField] private TextMeshProUGUI promptTargetText;

        [Tooltip("Contenedor del mensaje de feedback")]
        [SerializeField] private GameObject feedbackPanel;

        [Tooltip("Texto explicativo del feedback")]
        [SerializeField] private TextMeshProUGUI feedbackMessageText;

        [Tooltip("Fondo visual del panel de feedback para tintar el estado")]
        [SerializeField] private Image feedbackBackground;

        [Header("Botones de Control")]
        [SerializeField] private Button buttonExecute;
        [SerializeField] private Button buttonHint;
        [SerializeField] private Button buttonReset;
        [SerializeField] private Button buttonNextChallenge;

        [Header("Lógica")]
        [SerializeField] private ChallengeFlowController flowController;

        [Header("Efectos Visuales en Escena (CRT Feedback)")]
        [Tooltip("Renderer de la pantalla CRT para alterar emisión")]
        [SerializeField] private Renderer crtScreenRenderer;

        [Tooltip("Luz puntual de brillo de la pantalla CRT")]
        [SerializeField] private Light crtGlowLight;

        [Header("Paleta de Color de Feedback")]
        [SerializeField] private Color colorSuccess = new Color(0.20f, 0.85f, 0.55f, 0.95f); // Verde menta
        [SerializeField] private Color colorError = new Color(0.95f, 0.75f, 0.25f, 0.95f);   // Amarillo suave / ámbar
        [SerializeField] private Color colorHint = new Color(0.35f, 0.80f, 0.95f, 0.95f);    // Cian didáctico

        // Colores y estados base de la pantalla CRT
        private Color normalCrtEmission = new Color(0.04f, 0.30f, 0.10f);
        private Color successCrtEmission = new Color(0.20f, 1.0f, 0.65f);
        private Color normalGlowColor = new Color(0.1f, 0.9f, 0.3f);
        private Color successGlowColor = new Color(0.25f, 1.0f, 0.70f);

        private MaterialPropertyBlock crtPropertyBlock;

        private void Awake()
        {
            if (flowController == null)
            {
                flowController = GetComponent<ChallengeFlowController>();
            }

            CacheNormalCrtEmission();
        }

        private void Start()
        {
            if (flowController != null && !flowController.IsInitialized)
            {
                flowController.Initialize();
            }

            ApplyCurrentChallengePresentation();

            // Configurar estado inicial
            ResetTerminalState();

            // Asignar listeners de botones
            if (buttonExecute != null) buttonExecute.onClick.AddListener(OnClickExecute);
            if (buttonHint != null) buttonHint.onClick.AddListener(OnClickHint);
            if (buttonReset != null) buttonReset.onClick.AddListener(OnClickReset);
            if (buttonNextChallenge != null) buttonNextChallenge.onClick.AddListener(OnClickNextChallenge);
        }

        private void OnDestroy()
        {
            // Limpiar listeners al destruirse
            if (buttonExecute != null) buttonExecute.onClick.RemoveListener(OnClickExecute);
            if (buttonHint != null) buttonHint.onClick.RemoveListener(OnClickHint);
            if (buttonReset != null) buttonReset.onClick.RemoveListener(OnClickReset);
            if (buttonNextChallenge != null) buttonNextChallenge.onClick.RemoveListener(OnClickNextChallenge);
        }

        /// <summary>
        /// Evalúa la solución escrita por el usuario al presionar "EJECUTAR".
        /// </summary>
        public void OnClickExecute()
        {
            if (inputField == null || flowController == null || !flowController.IsInitialized) return;

            string code = inputField.text;
            ValidationResult result = flowController.ValidateCurrent(code);

            // Actualizar consola de salida
            if (consoleOutputText != null)
            {
                consoleOutputText.text = result.ConsoleOutput;
            }

            // Mostrar feedback
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(true);
            }

            if (feedbackMessageText != null)
            {
                feedbackMessageText.text = result.FeedbackMessage;
            }

            if (result.IsSuccess)
            {
                // Éxito: Bloquear edición y activar brillo verde menta
                inputField.interactable = false;

                if (feedbackBackground != null)
                {
                    feedbackBackground.color = colorSuccess;
                }

                SetCrtScreenVisuals(successCrtEmission, successGlowColor, 0.8f);
                SetNextChallengeAvailability(flowController.CanAdvance);
            }
            else
            {
                // Error: Mantener input editable y pintar ámbar
                inputField.interactable = true;

                if (feedbackBackground != null)
                {
                    feedbackBackground.color = colorError;
                }

                SetCrtScreenVisuals(normalCrtEmission, normalGlowColor, 0.4f);
                SetNextChallengeAvailability(false);
            }
        }

        /// <summary>
        /// Muestra la pista didáctica al presionar "PISTA".
        /// </summary>
        public void OnClickHint()
        {
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(true);
            }

            if (feedbackMessageText != null)
            {
                ChallengeDefinition challenge = flowController != null ? flowController.CurrentChallenge : null;
                feedbackMessageText.text = challenge != null ? challenge.Hint : string.Empty;
            }

            if (feedbackBackground != null)
            {
                feedbackBackground.color = colorHint;
            }
        }

        /// <summary>
        /// Restablece la terminal a su estado original al presionar "REINICIAR".
        /// </summary>
        public void OnClickReset()
        {
            ResetTerminalState();
        }

        /// <summary>
        /// Avanza al siguiente reto disponible después de resolver el actual.
        /// </summary>
        public void OnClickNextChallenge()
        {
            if (flowController == null || !flowController.TryAdvance())
            {
                return;
            }

            ApplyCurrentChallengePresentation();
            ResetTerminalState();
        }

        private void ResetTerminalState()
        {
            if (inputField != null)
            {
                inputField.text = string.Empty;
                inputField.interactable = true;
            }

            if (consoleOutputText != null)
            {
                consoleOutputText.text = "> Esperando instrucciones...";
            }

            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(false);
            }

            SetNextChallengeAvailability(false);
            SetCrtScreenVisuals(normalCrtEmission, normalGlowColor, 0.4f);
        }

        private void SetNextChallengeAvailability(bool canAdvance)
        {
            if (buttonNextChallenge == null)
            {
                return;
            }

            buttonNextChallenge.interactable = canAdvance;
            buttonNextChallenge.gameObject.SetActive(canAdvance);
        }

        private void ApplyCurrentChallengePresentation()
        {
            if (flowController == null || !flowController.IsInitialized)
            {
                return;
            }

            ChallengeDefinition challenge = flowController.CurrentChallenge;
            if (challenge == null)
            {
                return;
            }

            if (moduleText != null)
            {
                moduleText.text = flowController.ModuleTitle;
            }

            if (progressText != null)
            {
                progressText.text = $"Reto {flowController.CurrentIndex + 1} de {flowController.PlannedChallengeCount}";
            }

            if (promptIntroText != null)
            {
                promptIntroText.text = challenge.Prompt;
            }

            if (promptTargetText != null)
            {
                promptTargetText.text = $"> \"{challenge.TargetDisplayText}\"";
            }
        }

        private void SetCrtScreenVisuals(Color emissionColor, Color lightColor, float lightIntensity)
        {
            if (crtScreenRenderer != null)
            {
                Material sharedMaterial = crtScreenRenderer.sharedMaterial;
                if (sharedMaterial != null && sharedMaterial.HasProperty(EmissionColorPropertyId))
                {
                    crtPropertyBlock ??= new MaterialPropertyBlock();
                    crtScreenRenderer.GetPropertyBlock(crtPropertyBlock);
                    crtPropertyBlock.SetColor(EmissionColorPropertyId, emissionColor);
                    crtScreenRenderer.SetPropertyBlock(crtPropertyBlock);
                }
            }

            if (crtGlowLight != null)
            {
                crtGlowLight.color = lightColor;
                crtGlowLight.intensity = lightIntensity;
            }
        }

        private void CacheNormalCrtEmission()
        {
            if (crtScreenRenderer == null)
            {
                return;
            }

            crtPropertyBlock = new MaterialPropertyBlock();
            crtScreenRenderer.GetPropertyBlock(crtPropertyBlock);

            if (crtPropertyBlock.HasColor(EmissionColorPropertyId))
            {
                normalCrtEmission = crtPropertyBlock.GetColor(EmissionColorPropertyId);
                return;
            }

            Material sharedMaterial = crtScreenRenderer.sharedMaterial;
            if (sharedMaterial != null && sharedMaterial.HasProperty(EmissionColorPropertyId))
            {
                normalCrtEmission = sharedMaterial.GetColor(EmissionColorPropertyId);
            }
        }
    }
}
