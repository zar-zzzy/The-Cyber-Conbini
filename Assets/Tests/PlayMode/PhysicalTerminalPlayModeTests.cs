using System.Collections;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CyberConbini.Tests.PlayMode
{
    public class PhysicalTerminalPlayModeTests
    {
        [UnitySetUp]
        public IEnumerator LoadStore()
        {
            SceneManager.LoadScene("Conbini_Main", LoadSceneMode.Single);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Terminal_RemainsAttachedToPhysicalDisplay()
        {
            var canvas = GameObject.Find("UI_Terminal").GetComponent<Canvas>();
            var screen = GameObject.Find("CRT_Screen").transform;
            Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.WorldSpace));
            Assert.That(Vector3.Distance(canvas.transform.position, screen.position), Is.LessThan(.02f));
            Assert.That(Quaternion.Angle(canvas.transform.rotation, screen.rotation), Is.LessThan(.5f));
            var position = canvas.transform.position;
            var controller = Object.FindAnyObjectByType<TerminalExperienceController>();
            controller.RequestEnterTerminal(false);
            controller.AdvanceTransition(1f);
            yield return null;
            Assert.That(Vector3.Distance(canvas.transform.position, position), Is.LessThan(.001f));
        }

        [UnityTest]
        public IEnumerator Entry_FramesWholeDisplayStraightOn()
        {
            var controller = Object.FindAnyObjectByType<TerminalExperienceController>();
            controller.RequestEnterTerminal(false);
            controller.AdvanceTransition(1f);
            yield return null;
            var panel = GameObject.Find("Terminal_Panel").GetComponent<RectTransform>();
            var camera = Camera.main;
            Assert.That(Quaternion.Angle(camera.transform.rotation, panel.rotation), Is.LessThan(.5f));
            var corners = new Vector3[4];
            panel.GetWorldCorners(corners);
            foreach (var corner in corners)
            {
                var point = camera.WorldToViewportPoint(corner);
                Assert.That(point.z, Is.GreaterThan(camera.nearClipPlane));
                Assert.That(point.x, Is.InRange(.02f, .98f));
                Assert.That(point.y, Is.InRange(.02f, .98f));
            }
        }

        [UnityTest]
        public IEnumerator ExecutionAndReset_KeepDisplayGlowSubdued()
        {
            yield return CustomerVisitPlayModeTests.WaitForCheckout();
            var terminal = Object.FindAnyObjectByType<TerminalUIController>();
            var input = GameObject.Find("Input_Panel").GetComponent<TMP_InputField>();
            var glow = GameObject.Find("Light_CRT_Glow").GetComponent<Light>();
            input.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();
            Assert.That(Object.FindAnyObjectByType<CyberConbini.Gameplay.ChallengeFlowController>().IsCurrentChallengeCompleted, Is.True);
            Assert.That(glow.intensity, Is.InRange(.001f, .15f), "Success must not flood the register with light.");
            terminal.OnClickReset();
            Assert.That(glow.intensity, Is.InRange(.001f, .10f), "Reset must retain the subdued screen lighting.");
            input.text = "print(\"incorrecto\")";
            terminal.OnClickExecute();
            Assert.That(glow.intensity, Is.InRange(.001f, .10f), "Error must also retain the subdued lighting.");
            yield return null;
        }
    }
}
