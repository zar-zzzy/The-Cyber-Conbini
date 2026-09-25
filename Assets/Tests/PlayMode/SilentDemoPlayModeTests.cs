using System.Collections;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CyberConbini.Tests.PlayMode
{
    public class SilentDemoPlayModeTests
    {
        [UnityTest]
        public IEnumerator MainScene_StartsWithoutPlayingAudio()
        {
            SceneManager.LoadScene("Conbini_Main", LoadSceneMode.Single);
            yield return null;

            AssertNoPlayingAudio();
        }

        [UnityTest]
        public IEnumerator ValidSolution_DoesNotPlayTemporaryAudio()
        {
            SceneManager.LoadScene("Conbini_Main", LoadSceneMode.Single);
            yield return null;

            TerminalUIController terminal = Object.FindAnyObjectByType<TerminalUIController>();
            TMP_InputField input = GameObject.Find("Input_Panel")?.GetComponent<TMP_InputField>();
            Assert.That(terminal, Is.Not.Null);
            Assert.That(input, Is.Not.Null);

            input.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();

            AssertNoPlayingAudio();
        }

        private static void AssertNoPlayingAudio()
        {
            foreach (AudioSource source in Object.FindObjectsByType<AudioSource>())
            {
                Assert.That(source.isPlaying, Is.False, $"Sonido activo: {source.name}");
            }
        }
    }
}
