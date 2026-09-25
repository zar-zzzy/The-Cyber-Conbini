using CyberConbini.UI;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CyberConbini.Tests.EditMode
{
    public class TerminalExperienceControllerTests
    {
        private const float PositionTolerance = 0.0001f;
        private const float RotationTolerance = 0.01f;
        private const float FieldOfViewTolerance = 0.0001f;

        private GameObject controllerObject;
        private GameObject cameraObject;
        private GameObject cashierPointObject;
        private GameObject terminalPointObject;
        private GameObject inputObject;
        private TerminalExperienceController controller;
        private Camera mainCamera;
        private TMP_InputField inputField;

        [SetUp]
        public void SetUp()
        {
            controllerObject = new GameObject("TerminalExperienceControllerTests");
            cameraObject = new GameObject("Test Camera", typeof(Camera));
            cashierPointObject = new GameObject("CashierCameraPoint");
            terminalPointObject = new GameObject("TerminalCameraPoint");
            inputObject = new GameObject("Test Input", typeof(RectTransform), typeof(TMP_InputField));

            mainCamera = cameraObject.GetComponent<Camera>();
            inputField = inputObject.GetComponent<TMP_InputField>();

            cashierPointObject.transform.SetPositionAndRotation(
                new Vector3(0f, 1.55f, -1.25f),
                Quaternion.Euler(6.5f, 3f, 0f)
            );
            terminalPointObject.transform.SetPositionAndRotation(
                new Vector3(-0.10f, 1.42f, -0.62f),
                Quaternion.Euler(10f, -8f, 0f)
            );

            controller = controllerObject.AddComponent<TerminalExperienceController>();

            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("mainCamera").objectReferenceValue = mainCamera;
            serializedController.FindProperty("cashierCameraPoint").objectReferenceValue =
                cashierPointObject.transform;
            serializedController.FindProperty("terminalCameraPoint").objectReferenceValue =
                terminalPointObject.transform;
            serializedController.FindProperty("terminalInputField").objectReferenceValue = inputField;
            serializedController.FindProperty("enterDuration").floatValue = 0.45f;
            serializedController.FindProperty("returnDuration").floatValue = 0.40f;
            serializedController.FindProperty("cashierFieldOfView").floatValue = 60f;
            serializedController.FindProperty("terminalFieldOfView").floatValue = 42f;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            controller.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(inputObject);
            Object.DestroyImmediate(terminalPointObject);
            Object.DestroyImmediate(cashierPointObject);
            Object.DestroyImmediate(cameraObject);
            Object.DestroyImmediate(controllerObject);
        }

        [Test]
        public void Initialize_StartsInCashierView()
        {
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.CashierView));
        }

        [Test]
        public void RequestEnter_FromCashierView_IsAccepted()
        {
            bool accepted = controller.RequestEnterTerminal();

            Assert.That(accepted, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.EnteringTerminal));
        }

        [Test]
        public void RequestEnter_WhileEnteringTerminal_IsIgnored()
        {
            Assert.That(controller.RequestEnterTerminal(), Is.True);

            bool acceptedAgain = controller.RequestEnterTerminal();

            Assert.That(acceptedAgain, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.EnteringTerminal));
        }

        [Test]
        public void AdvanceTransition_CompletesEntryInTerminalView()
        {
            controller.RequestEnterTerminal();

            controller.AdvanceTransition(0.45f);

            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.TerminalView));
        }

        [Test]
        public void RequestReturn_FromTerminalView_IsAccepted()
        {
            EnterTerminalImmediately();

            bool accepted = controller.RequestReturnToCashier();

            Assert.That(accepted, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.ReturningToCashier));
        }

        [Test]
        public void RequestReturn_WhileReturningToCashier_IsIgnored()
        {
            EnterTerminalImmediately();
            Assert.That(controller.RequestReturnToCashier(), Is.True);

            bool acceptedAgain = controller.RequestReturnToCashier();

            Assert.That(acceptedAgain, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.ReturningToCashier));
        }

        [Test]
        public void AdvanceTransition_CompletesReturnInCashierView()
        {
            EnterTerminalImmediately();
            controller.RequestReturnToCashier();

            controller.AdvanceTransition(0.40f);

            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.CashierView));
        }

        [Test]
        public void RequestEnter_WhenInputFieldHasFocus_IsIgnored()
        {
            bool accepted = controller.RequestEnterTerminal(inputFieldHasFocus: true);

            Assert.That(accepted, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.CashierView));
        }

        [Test]
        public void CompleteEntry_ReachesTerminalTransformAndFieldOfView()
        {
            EnterTerminalImmediately();

            Assert.That(
                Vector3.Distance(mainCamera.transform.position, terminalPointObject.transform.position),
                Is.LessThan(PositionTolerance)
            );
            Assert.That(
                Quaternion.Angle(mainCamera.transform.rotation, terminalPointObject.transform.rotation),
                Is.LessThan(RotationTolerance)
            );
            Assert.That(mainCamera.fieldOfView, Is.EqualTo(42f).Within(FieldOfViewTolerance));
        }

        [Test]
        public void CompleteReturn_RestoresCashierTransformAndFieldOfView()
        {
            EnterTerminalImmediately();
            controller.RequestReturnToCashier();
            controller.AdvanceTransition(0.40f);

            Assert.That(
                Vector3.Distance(mainCamera.transform.position, cashierPointObject.transform.position),
                Is.LessThan(PositionTolerance)
            );
            Assert.That(
                Quaternion.Angle(mainCamera.transform.rotation, cashierPointObject.transform.rotation),
                Is.LessThan(RotationTolerance)
            );
            Assert.That(mainCamera.fieldOfView, Is.EqualTo(60f).Within(FieldOfViewTolerance));
        }

        [Test]
        public void Initialize_DisablesInputDraftRestorationOnEscapeAndDeactivation()
        {
            inputField.restoreOriginalTextOnEscape = true;
            inputField.resetOnDeActivation = true;

            controller.Initialize();

            Assert.That(inputField.restoreOriginalTextOnEscape, Is.False);
            Assert.That(inputField.resetOnDeActivation, Is.False);
        }

        private void EnterTerminalImmediately()
        {
            Assert.That(controller.RequestEnterTerminal(), Is.True);
            controller.AdvanceTransition(0.45f);
            Assert.That(controller.CurrentState, Is.EqualTo(TerminalExperienceState.TerminalView));
        }
    }
}
