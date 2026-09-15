using CyberConbini.Gameplay;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace CyberConbini.Tests.EditMode
{
    public class ChallengeValidatorTests
    {
        private const string CatalogPath = "Assets/Data/Challenges/Module1ChallengeCatalog.asset";

        private GameObject testGameObject;
        private ChallengeValidator validator;
        private ChallengeDefinition challenge;

        [SetUp]
        public void SetUp()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);
            Assert.That(catalog, Is.Not.Null, "El catálogo del Módulo 1 debe existir para validar el reto real.");

            testGameObject = new GameObject("ChallengeValidatorTests");
            validator = testGameObject.AddComponent<ChallengeValidator>();
            challenge = catalog.GetChallenge(0);
        }

        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
        }

        [Test]
        public void Validate_DoubleQuotedPrint_ReturnsSuccess()
        {
            ValidationResult result = validator.Validate(
                challenge,
                "print(\"Bienvenido al Cyber-Conbini\")"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
            Assert.That(result.FeedbackMessage, Is.EqualTo("¡Correcto! El cliente ha sido recibido."));
        }

        [Test]
        public void Validate_SingleQuotedPrint_ReturnsSuccess()
        {
            ValidationResult result = validator.Validate(
                challenge,
                "print('Bienvenido al Cyber-Conbini')"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
        }

        [Test]
        public void Validate_PrintWithExtraSpaces_ReturnsSuccess()
        {
            ValidationResult result = validator.Validate(
                challenge,
                "print(  \"Bienvenido al Cyber-Conbini\"  )"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
        }

        [Test]
        public void Validate_MessageWithoutPrint_ReturnsSyntaxError()
        {
            ValidationResult result = validator.Validate(challenge, "Bienvenido al Cyber-Conbini");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error de sintaxis: falta la función print()."));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "Recuerda envolver el texto dentro de la función print(\"... \")."
            ));
        }

        [Test]
        public void Validate_PrintWithDifferentMessage_ReturnsMismatch()
        {
            ValidationResult result = validator.Validate(challenge, "print(\"Hola\")");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Hola"));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "Aún no coincide. Se esperaba: \"Bienvenido al Cyber-Conbini\"."
            ));
        }

        [Test]
        public void Validate_DifferentInstruction_ReturnsFailure()
        {
            ValidationResult result = validator.Validate(
                challenge,
                "input(\"Bienvenido al Cyber-Conbini\")"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error: instrucción no reconocida."));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "Aún no funciona. Revisa la instrucción e inténtalo otra vez."
            ));
        }

        [Test]
        public void Validate_InputLongerThanLimit_IsRejectedBeforePatternMatching()
        {
            string oversizedInput = new string('a', ChallengeValidator.MaxInputLength + 1);

            ValidationResult result = validator.Validate(challenge, oversizedInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error: instrucción demasiado larga."));
        }

        [Test]
        public void Validate_RichTextInWrongLiteral_EscapesMarkupInConsoleOutput()
        {
            ValidationResult result = validator.Validate(
                challenge,
                "print(\"<color=red>Hola</color>\")"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Not.Contain("<color"));
            Assert.That(result.ConsoleOutput, Does.Not.Contain("</color>"));
            Assert.That(result.ConsoleOutput, Does.Contain("&lt;color=red&gt;Hola&lt;/color&gt;"));
        }
    }
}
