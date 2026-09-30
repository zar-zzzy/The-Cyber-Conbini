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

        [Tooltip("Luz breve sobre el producto al resolver un reto")]
        [SerializeField] private Light scannerSuccessLight;

        [SerializeField, Min(0.01f)] private float scannerFlashDuration = 0.45f;
        [SerializeField, Min(0f)] private float scannerFlashIntensity = 3f;

        [Tooltip("Raíz del cliente para un asentimiento breve al acertar")]
        [SerializeField] private Transform customerReactionTarget;
        [SerializeField, Min(0.01f)] private float customerNodDuration = 0.65f;
        [SerializeField, Min(0f)] private float customerNodAngle = 5f;

        [Header("Paleta de Color de Feedback")]
        [SerializeField] private Color colorSuccess = new Color(0.20f, 0.85f, 0.55f, 0.95f); // Verde menta
        [SerializeField] private Color colorError = new Color(0.95f, 0.75f, 0.25f, 0.95f);   // Amarillo suave / ámbar
        [SerializeField] private Color colorHint = new Color(0.35f, 0.80f, 0.95f, 0.95f);    // Cian didáctico

        // Colores y estados base de la pantalla CRT
        private Color normalCrtEmission = new Color(0.04f, 0.30f, 0.10f);
        [SerializeField] private Color successCrtEmission = new Color(0.06f, 0.09f, 0.16f);
        [SerializeField] private Color normalGlowColor = new Color(0.48f, 0.64f, 0.97f);
        [SerializeField] private Color successGlowColor = new Color(0.49f, 0.81f, 1.0f);
        [SerializeField, Min(0f)] private float normalGlowIntensity = 0.035f;
        [SerializeField, Min(0f)] private float successGlowIntensity = 0.075f;

        private MaterialPropertyBlock crtPropertyBlock;
        private float scannerFlashRemaining;
        private float customerNodRemaining;
        private Quaternion customerNormalRotation;
        private bool visitInteractionAllowed = true;
        private bool visitControlsClosure;
        private bool modalInputBlocked;

        public event System.Action<string> HintRequested;
        public void SetModalInputBlocked(bool blocked) => modalInputBlocked = blocked;

        public bool VisitInteractionAllowed => visitInteractionAllowed;

        public void SetVisitInteractionAllowed(bool allowed)
        {
            visitInteractionAllowed = allowed;
            visitControlsClosure = true;
            if (buttonExecute != null) buttonExecute.interactable = allowed;
            if (buttonHint != null) buttonHint.interactable = allowed;
            if (buttonReset != null) buttonReset.interactable = allowed;
            SetNextChallengeAvailability(allowed && flowController != null && flowController.CanAdvance);
        }

        public void SetVisitStatus(string message)
        {
            if (consoleOutputText != null) consoleOutputText.text = message;
        }

        public void ShowVisitCompleted()
        {
            SetVisitInteractionAllowed(false);
            if (progressText != null) progressText.text = "Módulo completado";
            if (feedbackPanel != null) feedbackPanel.SetActive(true);
            if (feedbackMessageText != null)
                feedbackMessageText.text = "Cliente atendido. ¡Has completado tu primer turno!";
            if (feedbackBackground != null) feedbackBackground.color = colorSuccess;
        }

        private void Awake()
        {
            if (flowController == null)
            {
                flowController = GetComponent<ChallengeFlowController>();
            }

            CacheNormalCrtEmission();
            if (customerReactionTarget != null)
            {
                customerNormalRotation = customerReactionTarget.localRotation;
            }
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

        private void Update()
        {
            if (scannerSuccessLight != null && scannerFlashRemaining > 0f)
            {
                scannerFlashRemaining = Mathf.Max(0f, scannerFlashRemaining - Time.deltaTime);
                scannerSuccessLight.intensity = scannerFlashIntensity *
                    (scannerFlashRemaining / Mathf.Max(0.01f, scannerFlashDuration));
            }

            if (customerReactionTarget != null && customerNodRemaining > 0f)
            {
                customerNodRemaining = Mathf.Max(0f, customerNodRemaining - Time.deltaTime);
                float progress = 1f - customerNodRemaining / Mathf.Max(0.01f, customerNodDuration);
                float angle = Mathf.Sin(progress * Mathf.PI) * customerNodAngle;
                customerReactionTarget.localRotation = customerNormalRotation * Quaternion.Euler(angle, 0f, 0f);
            }
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
            if (!visitInteractionAllowed || modalInputBlocked) return;
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
                feedbackMessageText.text = result.IsSuccess && visitControlsClosure && flowController.AreAllChallengesCompleted
                    ? "¡Correcto! Compra registrada. El cliente recoge su pedido..."
                    : result.FeedbackMessage;
            }

            if (result.IsSuccess)
            {
                // Éxito: Bloquear edición y activar brillo verde menta
                inputField.interactable = false;

                if (feedbackBackground != null)
                {
                    feedbackBackground.color = colorSuccess;
                }

                SetCrtScreenVisuals(successCrtEmission, successGlowColor, successGlowIntensity);
                if (scannerSuccessLight != null)
                {
                    scannerFlashRemaining = Mathf.Max(0.01f, scannerFlashDuration);
                    scannerSuccessLight.intensity = scannerFlashIntensity;
                }
                customerNodRemaining = Mathf.Max(0.01f, customerNodDuration);
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

                SetCrtScreenVisuals(normalCrtEmission, normalGlowColor, normalGlowIntensity);
                ResetScannerFlash();
                ResetCustomerNod();
                SetNextChallengeAvailability(false);
            }
        }

        /// <summary>
        /// Muestra la pista didáctica al presionar "PISTA".
        /// </summary>
        public void OnClickHint()
        {
            if (!visitInteractionAllowed || modalInputBlocked) return;
            if (HintRequested != null)
            {
                HintRequested.Invoke(flowController != null && flowController.CurrentChallenge != null
                    ? flowController.CurrentChallenge.Hint : string.Empty);
                return;
            }
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
            if (!visitInteractionAllowed || modalInputBlocked) return;
            ResetTerminalState();
        }

        /// <summary>
        /// Avanza al siguiente reto disponible después de resolver el actual.
        /// </summary>
        public void OnClickNextChallenge()
        {
            if (!visitInteractionAllowed || modalInputBlocked) return;
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
            SetCrtScreenVisuals(normalCrtEmission, normalGlowColor, normalGlowIntensity);
            ResetScannerFlash();
            ResetCustomerNod();
        }

        private void ResetScannerFlash()
        {
            scannerFlashRemaining = 0f;
            if (scannerSuccessLight != null)
            {
                scannerSuccessLight.intensity = 0f;
            }
        }

        private void ResetCustomerNod()
        {
            customerNodRemaining = 0f;
            if (customerReactionTarget != null)
            {
                customerReactionTarget.localRotation = customerNormalRotation;
            }
        }

        private void SetNextChallengeAvailability(bool canAdvance, bool showWhenCompleted = false)
        {
            if (buttonNextChallenge == null)
            {
                return;
            }

            buttonNextChallenge.interactable = canAdvance;
            buttonNextChallenge.gameObject.SetActive(canAdvance || showWhenCompleted);
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
