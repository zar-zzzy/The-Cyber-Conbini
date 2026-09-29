using System.Collections;
using CyberConbini.Gameplay;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CyberConbini.Tests.PlayMode
{
    public class CustomerVisitPlayModeTests
    {
        private static readonly string[] Solutions =
        {
            "print(\"Bienvenido al Cyber-Conbini\")", "print(\"Turno nocturno iniciado\")",
            "print(\"Onigiri de salmón\")", "cliente = \"Aiko\"",
            "cliente = \"Aiko\"\nprint(cliente)", "precio_onigiri = 450\nprint(precio_onigiri)"
        };

        [UnitySetUp]
        public IEnumerator LoadStore()
        {
            SceneManager.LoadScene("Conbini_Main");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Arrival_StartsOutside_CollectsProduct_AndWaitsAtCounter()
        {
            Transform customer = GameObject.Find("Customer").transform;
            Assert.That(customer.position.z, Is.GreaterThan(5f), "La visita comienza fuera de la tienda.");
            yield return WaitForCheckout();
            Transform product = GameObject.Find("Visit_Onigiri").transform;
            Assert.That(product.position.z, Is.LessThan(.6f), "El producto debe quedar sobre el mostrador.");
            Assert.That(product.position.y, Is.InRange(1f, 1.3f));
            Vector3 position = customer.position;
            yield return new WaitForSeconds(1f);
            Assert.That(Vector3.Distance(customer.position, position), Is.LessThan(.01f));
        }

        [UnityTest]
        public IEnumerator IncorrectAndFirstFiveSolutions_DoNotSendCustomerAway()
        {
            yield return WaitForCheckout();
            var terminal = Object.FindAnyObjectByType<TerminalUIController>();
            var flow = Object.FindAnyObjectByType<ChallengeFlowController>();
            var input = GameObject.Find("Input_Panel").GetComponent<TMP_InputField>();
            Transform customer = GameObject.Find("Customer").transform;
            Vector3 position = customer.position;
            input.text = "print(\"incorrecto\")";
            terminal.OnClickExecute();
            for (int i = 0; i < 5; i++)
            {
                input.text = Solutions[i];
                terminal.OnClickExecute();
                Assert.That(flow.CanAdvance, Is.True);
                terminal.OnClickNextChallenge();
            }
            yield return new WaitForSeconds(1f);
            Assert.That(Vector3.Distance(customer.position, position), Is.LessThan(.01f));
            Assert.That(GameObject.Find("Visit_Onigiri").transform.position.z, Is.LessThan(.6f));
        }

        [UnityTest]
        public IEnumerator FinalSolution_CustomerTakesPurchase_Exits_ThenModuleCloses()
        {
            yield return WaitForCheckout();
            var terminal = Object.FindAnyObjectByType<TerminalUIController>();
            var input = GameObject.Find("Input_Panel").GetComponent<TMP_InputField>();
            var flow = Object.FindAnyObjectByType<ChallengeFlowController>();
            Transform customer = GameObject.Find("Customer").transform;
            for (int i = 0; i < Solutions.Length; i++)
            {
                input.text = Solutions[i];
                terminal.OnClickExecute();
                if (i < Solutions.Length - 1) terminal.OnClickNextChallenge();
            }
            Assert.That(flow.CanAdvance, Is.False);
            var progress = GameObject.Find("Txt_Progress").GetComponent<TMP_Text>();
            Assert.That(progress.text, Does.Not.Contain("completado"));
            float deadline = Time.realtimeSinceStartup + 30f;
            while (!progress.text.Contains("completado") && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(progress.text, Does.Contain("completado"));
            Assert.That(customer.position.z, Is.GreaterThan(6f));
            Assert.That(GameObject.Find("Visit_Onigiri").transform.position.z, Is.GreaterThan(6f));
            Vector3 exit = customer.position;
            terminal.OnClickExecute();
            yield return new WaitForSeconds(.5f);
            Assert.That(Vector3.Distance(customer.position, exit), Is.LessThan(.01f), "No repetir la salida.");
        }

        internal static IEnumerator WaitForCheckout()
        {
            var visit = Object.FindAnyObjectByType<CustomerVisitController>();
            Assert.That(visit, Is.Not.Null);
            float deadline = Time.realtimeSinceStartup + 30f;
            while (!visit.IsReadyForCheckout && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(visit.IsReadyForCheckout, Is.True, "El cliente debe llegar y depositar el producto.");
        }
    }
}
