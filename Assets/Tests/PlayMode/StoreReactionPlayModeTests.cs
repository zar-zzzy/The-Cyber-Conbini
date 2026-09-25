using System.Collections;
using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CyberConbini.Tests.PlayMode
{
    public class StoreReactionPlayModeTests
    {
        private TerminalUIController terminal;
        private TMP_InputField inputField;
        private Light scannerLight;
        private Transform customer;
        private Quaternion customerInitialRotation;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene("Conbini_Main", LoadSceneMode.Single);
            yield return null;

            terminal = Object.FindAnyObjectByType<TerminalUIController>();
            inputField = GameObject.Find("Input_Panel")?.GetComponent<TMP_InputField>();
            scannerLight = GameObject.Find("Light_Scanner_Success")?.GetComponent<Light>();
            customer = GameObject.Find("Customer")?.transform;

            Assert.That(terminal, Is.Not.Null);
            Assert.That(inputField, Is.Not.Null);
            Assert.That(customer, Is.Not.Null);
            customerInitialRotation = customer.localRotation;
        }

        [UnityTest]
        public IEnumerator SuccessfulSolution_FlashesScannerLight()
        {
            Assert.That(scannerLight, Is.Not.Null, "Falta la luz de respuesta de la tienda.");
            Assert.That(scannerLight.intensity, Is.Zero);

            inputField.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();

            Assert.That(scannerLight.intensity, Is.GreaterThan(0f));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Reset_AfterSuccess_TurnsOffScannerLight()
        {
            Assert.That(scannerLight, Is.Not.Null);
            inputField.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();
            Assert.That(scannerLight.intensity, Is.GreaterThan(0f));

            terminal.OnClickReset();

            Assert.That(scannerLight.intensity, Is.Zero);
            yield return null;
        }

        [UnityTest]
        public IEnumerator IncorrectSolution_DoesNotFlashScannerLight()
        {
            Assert.That(scannerLight, Is.Not.Null);
            inputField.text = "print(\"Hola\")";
            terminal.OnClickExecute();

            Assert.That(scannerLight.intensity, Is.Zero);
            Assert.That(Quaternion.Angle(customer.localRotation, customerInitialRotation), Is.LessThan(0.1f));
            yield return null;
        }

        [UnityTest]
        public IEnumerator SuccessfulSolution_CustomerNodsThenReturnsToIdlePose()
        {
            inputField.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();

            yield return new WaitForSeconds(0.15f);
            Assert.That(Quaternion.Angle(customer.localRotation, customerInitialRotation), Is.GreaterThan(1f));

            yield return new WaitForSeconds(0.7f);
            Assert.That(Quaternion.Angle(customer.localRotation, customerInitialRotation), Is.LessThan(0.1f));
        }

        [UnityTest]
        public IEnumerator Reset_DuringCustomerNod_RestoresIdlePose()
        {
            inputField.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();
            yield return new WaitForSeconds(0.15f);
            Assert.That(Quaternion.Angle(customer.localRotation, customerInitialRotation), Is.GreaterThan(1f));

            terminal.OnClickReset();

            Assert.That(Quaternion.Angle(customer.localRotation, customerInitialRotation), Is.LessThan(0.1f));
        }

        [UnityTest]
        public IEnumerator Flash_EndsWithoutPlayerAction()
        {
            Assert.That(scannerLight, Is.Not.Null);
            inputField.text = "print(\"Bienvenido al Cyber-Conbini\")";
            terminal.OnClickExecute();
            Assert.That(scannerLight.intensity, Is.GreaterThan(0f));

            yield return new WaitForSeconds(0.7f);

            Assert.That(scannerLight.intensity, Is.Zero);
        }
    }
}
