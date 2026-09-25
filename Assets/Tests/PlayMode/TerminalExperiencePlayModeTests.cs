using System.Collections;
using CyberConbini.Gameplay;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CyberConbini.Tests.PlayMode
{
    public class TerminalExperiencePlayModeTests
    {
        private TerminalExperienceController controller;
        private TMP_InputField inputField;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene("Conbini_Main", LoadSceneMode.Single);
            yield return null;

            controller = Object.FindAnyObjectByType<TerminalExperienceController>();
            inputField = GameObject.Find("Input_Panel").GetComponent<TMP_InputField>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(inputField, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator CompleteEntry_FocusesTerminalInput()
        {
            Assert.That(controller.RequestEnterTerminal(), Is.True);
            controller.AdvanceTransition(1f);
            yield return null;

            Assert.That(EventSystem.current.currentSelectedGameObject, Is.SameAs(inputField.gameObject));
            Assert.That(inputField.isFocused, Is.True);
        }

        [UnityTest]
        public IEnumerator Return_DeselectsInputAndKeepsDraft()
        {
            Assert.That(controller.RequestEnterTerminal(), Is.True);
            controller.AdvanceTransition(1f);
            yield return null;

            inputField.text = "print(\"borrador\")";
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.SameAs(inputField.gameObject));
            Assert.That(controller.RequestReturnToCashier(), Is.True);
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Null, "immediately after return request");
            yield return null;

            Assert.That(inputField.isFocused, Is.False, "input focus after one frame");
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.Null, "after one frame");
            Assert.That(inputField.text, Is.EqualTo("print(\"borrador\")"));
        }

        [UnityTest]
        public IEnumerator AllModuleOnePromptsAndTargets_FitTerminalPanel()
        {
            string[] solutions =
            {
                "print(\"Bienvenido al Cyber-Conbini\")",
                "print(\"Turno nocturno iniciado\")",
                "print(\"Onigiri de salmón\")",
                "cliente = \"Aiko\"",
                "cliente = \"Aiko\"\nprint(cliente)",
                "precio_onigiri = 450\nprint(precio_onigiri)"
            };
            TerminalUIController uiController = Object.FindAnyObjectByType<TerminalUIController>();
            ChallengeFlowController flowController = Object.FindAnyObjectByType<ChallengeFlowController>();
            TextMeshProUGUI prompt = GameObject.Find("Txt_PromptIntro").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI target = GameObject.Find("Txt_PromptTarget").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < solutions.Length; i++)
            {
                yield return null;
                prompt.ForceMeshUpdate();
                target.ForceMeshUpdate();
                Assert.That(prompt.isTextOverflowing, Is.False, $"Prompt M1_R{i + 1}");
                Assert.That(target.isTextOverflowing, Is.False, $"Target M1_R{i + 1}");

                if (i == solutions.Length - 1)
                {
                    break;
                }

                inputField.text = solutions[i];
                uiController.OnClickExecute();
                Assert.That(flowController.CanAdvance, Is.True, $"Advance M1_R{i + 1}");
                uiController.OnClickNextChallenge();
            }
        }
    }
}
