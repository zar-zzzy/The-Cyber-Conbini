using System.Collections;
using System.Linq;
using CyberConbini.Gameplay;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace CyberConbini.Tests.PlayMode
{
    public class VisitWindowsPlayModeTests
    {
        [UnitySetUp]
        public IEnumerator LoadStore()
        {
            SceneManager.LoadScene("Conbini_Main");
            yield return null;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            var windows = GameObject.Find("UI_VisitWindows");
            if (windows != null) Object.Destroy(windows);
            yield return null; // Let the owner's OnDestroy release its saved time scale first.
            Time.timeScale = 1f;
        }

        private static Transform Root => GameObject.Find("UI_VisitWindows").transform;
        private static bool Visible(string window) => Root.Find(window).gameObject.activeInHierarchy;
        private static void Click(string path)
        {
            var button = Root.Find(path).GetComponent<Button>();
            Assert.That(button.IsInteractable(), Is.True, path);
            Assert.That(button.gameObject.activeInHierarchy, Is.True, path);
            button.onClick.Invoke();
        }
        private static void Begin() => Click("Window_Start/Card/Btn_BeginShift");
        private static TMP_InputField Input => GameObject.Find("Input_Panel").GetComponent<TMP_InputField>();
        private static TerminalExperienceController CameraFlow => Object.FindAnyObjectByType<TerminalExperienceController>();
        private static TerminalUIController Terminal => Object.FindAnyObjectByType<TerminalUIController>();
        private static ChallengeFlowController Flow => Object.FindAnyObjectByType<ChallengeFlowController>();

        [UnityTest]
        public IEnumerator Start_HoldsCustomerAndBlocksTerminal_UntilBegin()
        {
            Assert.That(Visible("Window_Start"), Is.True, "Inicio debe abrirse automáticamente.");
            Vector3 position = GameObject.Find("Customer").transform.position;
            Assert.That(CameraFlow.RequestEnterTerminal(false), Is.False);
            yield return WaitRealtime(1f);
            Assert.That(Vector3.Distance(position, GameObject.Find("Customer").transform.position), Is.LessThan(.001f));
            Click("Window_Start/Card/Btn_HowToPlay");
            Assert.That(Visible("Window_HowToPlay"), Is.True);
            Assert.That(Visible("Window_Start"), Is.False);
            Click("Window_HowToPlay/Card/Btn_BackToStart");
            Assert.That(Visible("Window_Start"), Is.True);
            Begin();
            Assert.That(Visible("Window_Start"), Is.False);
            yield return CustomerVisitPlayModeTests.WaitForCheckout();
        }

        [UnityTest]
        public IEnumerator Pause_FreezesVisit_CancelPreservesDraft_ContinueResumes()
        {
            Begin();
            yield return WaitRealtime(2f);
            Input.text = "borrador conservado";
            Time.timeScale = .75f;
            Click("MenuAccess/Btn_OpenPause");
            Assert.That(Visible("Window_Pause"), Is.True);
            Assert.That(Time.timeScale, Is.Zero);
            yield return null; // Unity applies the new deltaTime at the next frame boundary.
            Vector3 position = GameObject.Find("Customer").transform.position;
            Click("Window_Pause/Card/Btn_RequestRestart");
            Assert.That(Visible("Window_ConfirmRestart"), Is.True);
            Assert.That(EventSystem.current.currentSelectedGameObject.name, Is.EqualTo("Btn_Cancel"));
            yield return WaitRealtime(.3f);
            Assert.That(Vector3.Distance(position, GameObject.Find("Customer").transform.position), Is.LessThan(.001f));
            Click("Window_ConfirmRestart/Card/Btn_Cancel");
            Click("Window_Pause/Card/Btn_RequestStart");
            Assert.That(Visible("Window_ConfirmReturnToStart"), Is.True);
            Click("Window_ConfirmReturnToStart/Card/Btn_Cancel");
            Assert.That(Input.text, Is.EqualTo("borrador conservado"));
            Click("Window_Pause/Card/Btn_Continue");
            Assert.That(Time.timeScale, Is.EqualTo(.75f));
            Assert.That(Visible("Window_Pause"), Is.False);
        }

        [UnityTest]
        public IEnumerator Hint_UsesCurrentChallenge_BlocksBackground_AndRestoresDraftFocus()
        {
            Begin();
            yield return CustomerVisitPlayModeTests.WaitForCheckout();
            Assert.That(CameraFlow.RequestEnterTerminal(false), Is.True);
            CameraFlow.AdvanceTransition(1f);
            yield return null;
            Input.text = "print(\"Bienvenido al Cyber-Conbini\")";
            Terminal.OnClickHint();
            Assert.That(Visible("Window_Hint"), Is.True);
            Assert.That(Root.Find("Window_Hint/Card/Txt_ActiveHint").GetComponent<TMP_Text>().text, Is.EqualTo(Flow.CurrentChallenge.Hint));
            Assert.That(CameraFlow.RequestReturnToCashier(), Is.False);
            Terminal.OnClickExecute();
            Assert.That(Flow.IsCurrentChallengeCompleted, Is.False, "El modal debe bloquear acciones de fondo.");
            Click("Window_Hint/Card/Btn_CloseHint");
            yield return null;
            yield return null;
            Assert.That(Input.text, Is.EqualTo("print(\"Bienvenido al Cyber-Conbini\")"));
            Assert.That(Input.isFocused, Is.True);
            Terminal.OnClickExecute();
            Terminal.OnClickNextChallenge();
            Terminal.OnClickHint();
            Assert.That(Flow.CurrentIndex, Is.EqualTo(1));
            Assert.That(Root.Find("Window_Hint/Card/Txt_ActiveHint").GetComponent<TMP_Text>().text, Is.EqualTo(Flow.CurrentChallenge.Hint));
            Click("Window_Hint/Card/Btn_ReturnToTerminal");
            Assert.That(Visible("Window_Hint"), Is.False);
        }

        [UnityTest]
        public IEnumerator Restart_RequiresConfirmation_ResetsWholeVisitAndProgress()
        {
            Begin();
            yield return CustomerVisitPlayModeTests.WaitForCheckout();
            Input.text = "print(\"Bienvenido al Cyber-Conbini\")";
            Terminal.OnClickExecute();
            Terminal.OnClickNextChallenge();
            Assert.That(Flow.CurrentIndex, Is.EqualTo(1));
            Click("MenuAccess/Btn_OpenPause");
            Click("Window_Pause/Card/Btn_RequestRestart");
            Click("Window_ConfirmRestart/Card/Btn_ConfirmRestart");
            yield return WaitReload();
            Assert.That(Visible("Window_Start"), Is.False);
            Assert.That(Flow.CurrentIndex, Is.Zero);
            Assert.That(Flow.IsCurrentChallengeCompleted, Is.False);
            Assert.That(Input.text, Is.Empty);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
            Assert.That(Object.FindAnyObjectByType<CustomerVisitController>().IsReadyForCheckout, Is.False);
            Assert.That(GameObject.Find("Visit_Onigiri").transform.position.z, Is.GreaterThan(.6f));
        }

        [UnityTest]
        public IEnumerator ReturnHome_ConfirmationReloadsToStart_AndBeginWorksAgain()
        {
            Begin();
            yield return null;
            Click("MenuAccess/Btn_OpenPause");
            Click("Window_Pause/Card/Btn_RequestStart");
            Click("Window_ConfirmReturnToStart/Card/Btn_ConfirmStart");
            yield return WaitReload();
            Assert.That(Visible("Window_Start"), Is.True);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
            Assert.That(CameraFlow.RequestEnterTerminal(false), Is.False);
            Begin();
            Assert.That(CameraFlow.RequestEnterTerminal(false), Is.True);
        }

        [UnityTest]
        public IEnumerator Completion_WaitsForExit_ThenPlayAgainStartsFreshVisit()
        {
            Begin();
            yield return CustomerVisitPlayModeTests.WaitForCheckout();
            string[] solutions = {
                "print(\"Bienvenido al Cyber-Conbini\")", "print(\"Turno nocturno iniciado\")",
                "print(\"Onigiri de salmón\")", "cliente = \"Aiko\"",
                "cliente = \"Aiko\"\nprint(cliente)", "precio_onigiri = 450\nprint(precio_onigiri)" };
            foreach (string solution in solutions)
            {
                Input.text = solution;
                Terminal.OnClickExecute();
                if (Flow.CanAdvance) Terminal.OnClickNextChallenge();
            }
            var visit = Object.FindAnyObjectByType<CustomerVisitController>();
            Assert.That(Flow.AreAllChallengesCompleted, Is.True);
            Assert.That(Visible("Window_Completed"), Is.False);
            float deadline = Time.realtimeSinceStartup + 35f;
            while (!visit.IsModuleClosed && Time.realtimeSinceStartup < deadline)
            {
                Assert.That(Visible("Window_Completed"), Is.False, "No anticipar el recibo durante la salida.");
                yield return null;
            }
            Assert.That(visit.IsModuleClosed, Is.True);
            yield return null;
            Assert.That(Visible("Window_Completed"), Is.True);
            Assert.That(GameObject.Find("Customer").transform.position.z, Is.GreaterThan(6f));
            Click("Window_Completed/Card/Btn_PlayAgain");
            yield return WaitReload();
            Assert.That(Visible("Window_Start"), Is.False);
            Assert.That(Flow.AreAllChallengesCompleted, Is.False);
            Assert.That(Flow.CurrentIndex, Is.Zero);
        }

        private static IEnumerator WaitReload()
        {
            // Async scene load may finish after several frames on slower machines.
            var previous = Root;
            float deadline = Time.realtimeSinceStartup + 10f;
            while (previous != null && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(previous == null, Is.True, "Se debe recargar la visita.");
            yield return null;
            yield return null;
        }

        private static IEnumerator WaitRealtime(float seconds)
        {
            float end = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < end) yield return null;
        }
    }
}
