using CyberConbini.Gameplay;
using NUnit.Framework;
using UnityEngine;

namespace CyberConbini.Tests.EditMode
{
    public class ChallengeValidatorTests
    {
        private GameObject testGameObject;
        private ChallengeValidator validator;

        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("ChallengeValidatorTests");
            validator = testGameObject.AddComponent<ChallengeValidator>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void ValidateFirstChallenge_DoubleQuotedPrint_ReturnsSuccess()
        {
            ValidationResult result = validator.ValidateFirstChallenge(
                "print(\"Bienvenido al Cyber-Conbini\")"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
            Assert.That(result.FeedbackMessage, Does.Contain("Correcto"));
        }

        [Test]
        public void ValidateFirstChallenge_SingleQuotedPrint_ReturnsSuccess()
        {
            ValidationResult result = validator.ValidateFirstChallenge(
                "print('Bienvenido al Cyber-Conbini')"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
        }

        [Test]
        public void ValidateFirstChallenge_PrintWithExtraSpaces_ReturnsSuccess()
        {
            ValidationResult result = validator.ValidateFirstChallenge(
                "print(  \"Bienvenido al Cyber-Conbini\"  )"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Bienvenido al Cyber-Conbini"));
        }

        [Test]
        public void ValidateFirstChallenge_MessageWithoutPrint_ReturnsSyntaxError()
        {
            ValidationResult result = validator.ValidateFirstChallenge(
                "Bienvenido al Cyber-Conbini"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Contain("falta la función print()"));
            Assert.That(result.FeedbackMessage, Does.Contain("print"));
        }

        [Test]
        public void ValidateFirstChallenge_PrintWithDifferentMessage_ReturnsMismatch()
        {
            ValidationResult result = validator.ValidateFirstChallenge("print(\"Hola\")");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Hola"));
            Assert.That(result.FeedbackMessage, Does.Contain(ChallengeValidator.TARGET_MESSAGE));
        }

        [Test]
        public void ValidateFirstChallenge_DifferentInstruction_ReturnsFailure()
        {
            ValidationResult result = validator.ValidateFirstChallenge(
                "input(\"Bienvenido al Cyber-Conbini\")"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Contain("Error de sintaxis"));
            Assert.That(result.FeedbackMessage, Does.Contain("print"));
        }
    }
}
