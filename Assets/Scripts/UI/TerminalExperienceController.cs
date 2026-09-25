using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberConbini.UI
{
    public enum TerminalExperienceState
    {
        CashierView,
        EnteringTerminal,
        TerminalView,
        ReturningToCashier
    }

    /// <summary>
    /// Controls the reversible camera transition between the cashier and terminal views.
    /// </summary>
    public sealed class TerminalExperienceController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform cashierCameraPoint;
        [SerializeField] private Transform terminalCameraPoint;

        [Header("Input Protection")]
        [SerializeField] private TMP_InputField terminalInputField;

        [Header("Transition Settings")]
        [Min(0f)]
        [SerializeField] private float enterDuration = 0.45f;

        [Min(0f)]
        [SerializeField] private float returnDuration = 0.40f;

        [SerializeField] private float cashierFieldOfView = 60f;
        [SerializeField] private float terminalFieldOfView = 42f;

        private Vector3 transitionStartPosition;
        private Quaternion transitionStartRotation;
        private float transitionStartFieldOfView;
        private float transitionElapsed;

        public TerminalExperienceState CurrentState { get; private set; } =
            TerminalExperienceState.CashierView;

        private bool IsTransitioning =>
            CurrentState == TerminalExperienceState.EnteringTerminal ||
            CurrentState == TerminalExperienceState.ReturningToCashier;

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            AdvanceTransition(Time.deltaTime);
        }

        private void OnGUI()
        {
            Event keyboardEvent = Event.current;
            if (keyboardEvent == null || keyboardEvent.type != EventType.KeyDown)
            {
                return;
            }

            if (keyboardEvent.keyCode == KeyCode.E)
            {
                RequestEnterTerminal();
            }
            else if (keyboardEvent.keyCode == KeyCode.Escape)
            {
                RequestReturnToCashier();
            }
        }

        public void Initialize()
        {
            CurrentState = TerminalExperienceState.CashierView;
            transitionElapsed = 0f;

            if (terminalInputField != null)
            {
                terminalInputField.restoreOriginalTextOnEscape = false;
                terminalInputField.resetOnDeActivation = false;
            }

            if (HasRequiredCameraReferences())
            {
                ApplyView(cashierCameraPoint, cashierFieldOfView);
            }
        }

        public bool RequestEnterTerminal()
        {
            return RequestEnterTerminal(IsTerminalInputFocused());
        }

        public bool RequestEnterTerminal(bool inputFieldHasFocus)
        {
            if (
                CurrentState != TerminalExperienceState.CashierView ||
                inputFieldHasFocus ||
                !HasRequiredCameraReferences()
            )
            {
                return false;
            }

            BeginTransition(TerminalExperienceState.EnteringTerminal);
            return true;
        }

        public bool RequestReturnToCashier()
        {
            if (
                CurrentState != TerminalExperienceState.TerminalView ||
                !HasRequiredCameraReferences()
            )
            {
                return false;
            }

            if (terminalInputField != null)
            {
                terminalInputField.DeactivateInputField();
                EventSystem eventSystem = EventSystem.current;
                if (eventSystem != null &&
                    eventSystem.currentSelectedGameObject == terminalInputField.gameObject)
                {
                    eventSystem.SetSelectedGameObject(null);
                }
            }

            BeginTransition(TerminalExperienceState.ReturningToCashier);
            return true;
        }

        public void AdvanceTransition(float deltaTime)
        {
            if (!IsTransitioning || !HasRequiredCameraReferences())
            {
                return;
            }

            bool isEntering = CurrentState == TerminalExperienceState.EnteringTerminal;
            Transform targetPoint = isEntering ? terminalCameraPoint : cashierCameraPoint;
            float targetFieldOfView = isEntering ? terminalFieldOfView : cashierFieldOfView;
            float duration = isEntering ? enterDuration : returnDuration;

            transitionElapsed += Mathf.Max(0f, deltaTime);
            float normalizedTime = duration <= 0f
                ? 1f
                : Mathf.Clamp01(transitionElapsed / duration);
            float smoothedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            Transform cameraTransform = mainCamera.transform;
            cameraTransform.position = Vector3.Lerp(
                transitionStartPosition,
                targetPoint.position,
                smoothedTime
            );
            cameraTransform.rotation = Quaternion.Slerp(
                transitionStartRotation,
                targetPoint.rotation,
                smoothedTime
            );
            mainCamera.fieldOfView = Mathf.Lerp(
                transitionStartFieldOfView,
                targetFieldOfView,
                smoothedTime
            );

            if (normalizedTime < 1f)
            {
                return;
            }

            ApplyView(targetPoint, targetFieldOfView);
            CurrentState = isEntering
                ? TerminalExperienceState.TerminalView
                : TerminalExperienceState.CashierView;

            if (isEntering && terminalInputField != null &&
                terminalInputField.isActiveAndEnabled && terminalInputField.interactable)
            {
                terminalInputField.Select();
                terminalInputField.ActivateInputField();
            }
        }

        private void BeginTransition(TerminalExperienceState transitionState)
        {
            Transform cameraTransform = mainCamera.transform;
            transitionStartPosition = cameraTransform.position;
            transitionStartRotation = cameraTransform.rotation;
            transitionStartFieldOfView = mainCamera.fieldOfView;
            transitionElapsed = 0f;
            CurrentState = transitionState;
        }

        private bool IsTerminalInputFocused()
        {
            if (terminalInputField == null)
            {
                return false;
            }

            if (terminalInputField.isFocused)
            {
                return true;
            }

            EventSystem eventSystem = EventSystem.current;
            return eventSystem != null &&
                eventSystem.currentSelectedGameObject == terminalInputField.gameObject;
        }

        private bool HasRequiredCameraReferences()
        {
            return mainCamera != null &&
                cashierCameraPoint != null &&
                terminalCameraPoint != null;
        }

        private void ApplyView(Transform cameraPoint, float fieldOfView)
        {
            mainCamera.transform.SetPositionAndRotation(cameraPoint.position, cameraPoint.rotation);
            mainCamera.fieldOfView = fieldOfView;
        }
    }
}
