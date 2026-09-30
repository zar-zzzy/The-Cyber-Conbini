using System;
using System.Collections.Generic;
using CyberConbini.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CyberConbini.UI
{
    public enum VisitWindowState
    {
        Start, HowToPlay, Playing, Pause, ConfirmRestart, ConfirmReturnToStart, Hint, Completed
    }

    /// <summary>Owns visit menus; challenge and customer controllers retain their gameplay responsibilities.</summary>
    public sealed class VisitWindowController : MonoBehaviour
    {
        [Header("Existing gameplay")]
        [SerializeField] private CustomerVisitController visit;
        [SerializeField] private TerminalUIController terminal;
        [SerializeField] private TerminalExperienceController experience;
        [SerializeField] private TMP_InputField terminalInput;
        [SerializeField] private CanvasGroup terminalInteractionGroup;

        [Header("Window roots")]
        [SerializeField] private GameObject startWindow;
        [SerializeField] private GameObject howToPlayWindow;
        [SerializeField] private GameObject pauseWindow;
        [SerializeField] private GameObject confirmRestartWindow;
        [SerializeField] private GameObject confirmReturnWindow;
        [SerializeField] private GameObject hintWindow;
        [SerializeField] private GameObject completedWindow;
        [SerializeField] private GameObject menuAccess;
        [SerializeField] private TMP_Text activeHintText;

        private readonly List<(Button button, UnityAction action)> bindings = new();
        private GameObject[] windows;
        private bool holdsTime;
        private float previousTimeScale;
        private bool reloading;
        private static string beginAfterReloadPath;

        public VisitWindowState CurrentState { get; private set; }
        public bool IsModalOpen => CurrentState != VisitWindowState.Playing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetReloadIntent() => beginAfterReloadPath = null;

        private void Awake()
        {
            windows = new[] { startWindow, howToPlayWindow, pauseWindow, confirmRestartWindow,
                confirmReturnWindow, hintWindow, completedWindow };
            Bind(startWindow, "Btn_BeginShift", BeginShift);
            Bind(startWindow, "Btn_HowToPlay", () => Show(VisitWindowState.HowToPlay));
            Button quit = Bind(startWindow, "Btn_QuitDesktop", QuitDesktop);
            quit.gameObject.SetActive(Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer);
            Bind(howToPlayWindow, "Btn_BackToStart", () => Show(VisitWindowState.Start));
            Bind(pauseWindow, "Btn_Continue", ContinueShift);
            Bind(pauseWindow, "Btn_RequestRestart", () => Show(VisitWindowState.ConfirmRestart));
            Bind(pauseWindow, "Btn_RequestStart", () => Show(VisitWindowState.ConfirmReturnToStart));
            Bind(confirmRestartWindow, "Btn_Cancel", () => Show(VisitWindowState.Pause));
            Bind(confirmRestartWindow, "Btn_ConfirmRestart", () => ReloadVisit(true));
            Bind(confirmReturnWindow, "Btn_Cancel", () => Show(VisitWindowState.Pause));
            Bind(confirmReturnWindow, "Btn_ConfirmStart", () => ReloadVisit(false));
            Bind(hintWindow, "Btn_CloseHint", CloseHint);
            Bind(hintWindow, "Btn_ReturnToTerminal", CloseHint);
            Bind(completedWindow, "Btn_PlayAgain", () => ReloadVisit(true));
            Bind(completedWindow, "Btn_ReturnToStart", () => ReloadVisit(false));
            Register(menuAccess.transform.Find("Btn_OpenPause").GetComponent<Button>(), OpenPause);
            terminal.HintRequested += OpenHint;
            Show(VisitWindowState.Start);
            if (beginAfterReloadPath == gameObject.scene.path)
            {
                beginAfterReloadPath = null;
                BeginShift();
            }
        }

        private void Update()
        {
            if (CurrentState == VisitWindowState.Playing && visit.IsModuleClosed)
                Show(VisitWindowState.Completed);
            UpdateMenuAccess();
        }

        private void OnGUI()
        {
            Event input = Event.current;
            if (input == null || input.type != EventType.KeyDown || input.keyCode != KeyCode.Escape || !IsModalOpen)
                return;
            input.Use(); // Consume before releasing the camera's modal input guard.
            switch (CurrentState)
            {
                case VisitWindowState.Hint: CloseHint(); break;
                case VisitWindowState.HowToPlay: Show(VisitWindowState.Start); break;
                case VisitWindowState.Pause: ContinueShift(); break;
                case VisitWindowState.ConfirmRestart:
                case VisitWindowState.ConfirmReturnToStart: Show(VisitWindowState.Pause); break;
            }
        }

        public void BeginShift()
        {
            if (reloading || CurrentState != VisitWindowState.Start) return;
            visit.BeginVisit();
            Show(VisitWindowState.Playing);
        }

        public void OpenPause()
        {
            if (reloading || CurrentState != VisitWindowState.Playing ||
                experience.CurrentState != TerminalExperienceState.CashierView) return;
            Show(VisitWindowState.Pause);
        }

        private void ContinueShift()
        {
            if (CurrentState == VisitWindowState.Pause) Show(VisitWindowState.Playing);
        }

        private void OpenHint(string hint)
        {
            if (reloading || CurrentState != VisitWindowState.Playing) return;
            activeHintText.text = hint;
            Show(VisitWindowState.Hint);
        }

        private void CloseHint()
        {
            if (CurrentState != VisitWindowState.Hint) return;
            Show(VisitWindowState.Playing);
            if (experience.CurrentState == TerminalExperienceState.TerminalView && terminalInput.interactable)
            {
                terminalInput.Select();
                terminalInput.ActivateInputField();
            }
        }

        private void Show(VisitWindowState state)
        {
            if (reloading) return;
            CurrentState = state;
            bool pauseTime = state == VisitWindowState.Pause || state == VisitWindowState.ConfirmRestart ||
                state == VisitWindowState.ConfirmReturnToStart || state == VisitWindowState.Hint;
            if (pauseTime && !holdsTime)
            {
                previousTimeScale = Time.timeScale;
                holdsTime = true;
                Time.timeScale = 0f;
            }
            else if (!pauseTime) ReleaseTime();

            // Keep the visit's checkout gate independent from modal input protection.
            experience.SetUIInputBlocked(IsModalOpen);
            terminal.SetModalInputBlocked(IsModalOpen);
            terminalInteractionGroup.interactable = !IsModalOpen;
            terminalInteractionGroup.blocksRaycasts = !IsModalOpen;
            if (IsModalOpen) terminalInput.DeactivateInputField();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            foreach (GameObject window in windows) window.SetActive(false);
            GameObject target = WindowFor(state);
            if (target != null)
            {
                target.SetActive(true);
                string focus = state == VisitWindowState.ConfirmRestart || state == VisitWindowState.ConfirmReturnToStart
                    ? "Btn_Cancel" : null;
                Button first = focus == null ? target.GetComponentInChildren<Button>() :
                    target.transform.Find("Card/" + focus).GetComponent<Button>();
                if (first != null && EventSystem.current != null) first.Select();
            }
            UpdateMenuAccess();
        }

        private GameObject WindowFor(VisitWindowState state) => state switch
        {
            VisitWindowState.Start => startWindow,
            VisitWindowState.HowToPlay => howToPlayWindow,
            VisitWindowState.Pause => pauseWindow,
            VisitWindowState.ConfirmRestart => confirmRestartWindow,
            VisitWindowState.ConfirmReturnToStart => confirmReturnWindow,
            VisitWindowState.Hint => hintWindow,
            VisitWindowState.Completed => completedWindow,
            _ => null
        };

        private void UpdateMenuAccess() => menuAccess.SetActive(!reloading &&
            CurrentState == VisitWindowState.Playing &&
            experience.CurrentState == TerminalExperienceState.CashierView);

        private Button Bind(GameObject window, string name, UnityAction action)
        {
            Button button = window.transform.Find("Card/" + name).GetComponent<Button>();
            Register(button, action);
            return button;
        }

        private void Register(Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            bindings.Add((button, action));
        }

        private void ReloadVisit(bool begin)
        {
            if (reloading) return;
            reloading = true;
            ReleaseTime();
            beginAfterReloadPath = begin ? gameObject.scene.path : null;
            foreach (var binding in bindings) binding.button.interactable = false;
            SceneManager.LoadSceneAsync(gameObject.scene.path, LoadSceneMode.Single);
        }

        private void QuitDesktop()
        {
            ReleaseTime();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ReleaseTime()
        {
            if (!holdsTime) return;
            Time.timeScale = previousTimeScale;
            holdsTime = false;
        }

        private void OnDestroy()
        {
            ReleaseTime();
            if (terminal != null) terminal.HintRequested -= OpenHint;
            foreach (var binding in bindings)
                if (binding.button != null) binding.button.onClick.RemoveListener(binding.action);
        }
    }
}
