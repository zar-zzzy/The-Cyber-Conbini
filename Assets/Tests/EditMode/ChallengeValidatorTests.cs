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
        private ChallengeDefinition secondChallenge;
        private ChallengeDefinition thirdChallenge;
        private ChallengeDefinition fourthChallenge;
        private ChallengeDefinition fifthChallenge;
        private ChallengeDefinition sixthChallenge;

        [SetUp]
        public void SetUp()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);
            Assert.That(catalog, Is.Not.Null, "El catálogo del Módulo 1 debe existir para validar el reto real.");

            testGameObject = new GameObject("ChallengeValidatorTests");
            validator = testGameObject.AddComponent<ChallengeValidator>();
            challenge = catalog.GetChallenge(0);
            secondChallenge = catalog.GetChallenge(1);
            thirdChallenge = catalog.GetChallenge(2);
            fourthChallenge = catalog.GetChallenge(3);
            fifthChallenge = catalog.GetChallenge(4);
            sixthChallenge = catalog.GetChallenge(5);
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

        [TestCase("print(\"Turno nocturno iniciado\")")]
        [TestCase("print('Turno nocturno iniciado')")]
        [TestCase("print(  \"Turno nocturno iniciado\"  )")]
        public void Validate_SecondChallengeValidPrintVariants_ReturnSuccess(string playerInput)
        {
            ValidationResult result = validator.Validate(secondChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Turno nocturno iniciado"));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "¡Correcto! El turno nocturno ha comenzado."
            ));
        }

        [TestCase("print(\"Bienvenido al Cyber-Conbini\")")]
        [TestCase("Turno nocturno iniciado")]
        [TestCase("input(\"Turno nocturno iniciado\")")]
        public void Validate_SecondChallengeInvalidInputs_ReturnFailure(string playerInput)
        {
            ValidationResult result = validator.Validate(secondChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.FeedbackMessage, Is.Not.Empty);
        }

        [TestCase("print(\"Onigiri de salmón\")")]
        [TestCase("print('Onigiri de salmón')")]
        [TestCase("print(  \"Onigiri de salmón\"  )")]
        public void Validate_ThirdChallengeValidPrintVariants_ReturnSuccess(string playerInput)
        {
            ValidationResult result = validator.Validate(thirdChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Onigiri de salmón"));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "¡Correcto! El producto ha sido registrado."
            ));
        }

        [TestCase("print(\"Onigiri\")")]
        [TestCase("Onigiri de salmón")]
        [TestCase("input(\"Onigiri de salmón\")")]
        [TestCase("print(\"Bienvenido al Cyber-Conbini\")")]
        public void Validate_ThirdChallengeInvalidInputs_ReturnFailure(string playerInput)
        {
            ValidationResult result = validator.Validate(thirdChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.FeedbackMessage, Is.Not.Empty);
        }

        [TestCase("cliente = \"Aiko\"")]
        [TestCase("cliente = 'Aiko'")]
        [TestCase("cliente    =    \"Aiko\"")]
        [TestCase("cliente=\"Aiko\"")]
        public void Validate_FourthChallengeValidAssignments_ReturnSuccess(string playerInput)
        {
            ValidationResult result = validator.Validate(fourthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo(
                "> Variable cliente guardada correctamente."
            ));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "¡Correcto! El nombre de la cliente fue guardado."
            ));
        }

        [TestCase("cliente = Aiko")]
        [TestCase("nombre = \"Aiko\"")]
        [TestCase("Cliente = \"Aiko\"")]
        [TestCase("cliente == \"Aiko\"")]
        [TestCase("print(\"Aiko\")")]
        [TestCase("print(cliente)")]
        [TestCase("cliente = \"Yuki\"")]
        [TestCase("cliente = \"Aiko\"\nprint(cliente)")]
        [TestCase("cliente = \"Aiko\";")]
        [TestCase("cliente = \"A\\iko\"")]
        public void Validate_FourthChallengeInvalidAssignments_ReturnFailure(string playerInput)
        {
            ValidationResult result = validator.Validate(fourthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.FeedbackMessage, Is.Not.Empty);
        }

        [Test]
        public void Validate_FourthChallengeRichTextValue_EscapesMarkupInConsoleOutput()
        {
            ValidationResult result = validator.Validate(
                fourthChallenge,
                "cliente = \"<color=red>Aiko</color>\""
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Not.Contain("<color"));
            Assert.That(result.ConsoleOutput, Does.Not.Contain("</color>"));
            Assert.That(result.ConsoleOutput, Does.Contain(
                "&lt;color=red&gt;Aiko&lt;/color&gt;"
            ));
        }

        [Test]
        public void Validate_FourthChallengeOversizedInput_IsRejectedBeforePatternMatching()
        {
            string oversizedInput = new string('a', ChallengeValidator.MaxInputLength + 1);

            ValidationResult result = validator.Validate(fourthChallenge, oversizedInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error: instrucción demasiado larga."));
        }

        [TestCase("cliente = \"Aiko\"\nprint(cliente)")]
        [TestCase("cliente = 'Aiko'\nprint(cliente)")]
        [TestCase("cliente    =    \"Aiko\"\nprint( cliente )")]
        [TestCase("cliente=\"Aiko\"\nprint(cliente)")]
        [TestCase("cliente = \"Aiko\"\r\nprint(cliente)")]
        public void Validate_FifthChallengeValidAssignmentAndPrintVariants_ReturnSuccess(string playerInput)
        {
            ValidationResult result = validator.Validate(fifthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Aiko"));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "¡Correcto! El nombre de la cliente aparece en la terminal."
            ));
        }

        [TestCase("print(cliente)")]
        [TestCase("cliente = \"Aiko\"")]
        [TestCase("cliente = \"Aiko\"\nprint(\"Aiko\")")]
        [TestCase("cliente = \"Aiko\"\nprint(\"cliente\")")]
        [TestCase("cliente = \"Aiko\"\nprint(nombre)")]
        [TestCase("nombre = \"Aiko\"\nprint(nombre)")]
        [TestCase("cliente = \"Yuki\"\nprint(cliente)")]
        [TestCase("cliente == \"Aiko\"\nprint(cliente)")]
        [TestCase("cliente = Aiko\nprint(cliente)")]
        [TestCase("print(cliente)\ncliente = \"Aiko\"")]
        [TestCase("cliente = \"Aiko\"\nprint(cliente)\nprint(cliente)")]
        [TestCase("# comentario\ncliente = \"Aiko\"\nprint(cliente)")]
        [TestCase("cliente = \"Aiko\";\nprint(cliente)")]
        [TestCase("cliente = \"A\\iko\"\nprint(cliente)")]
        public void Validate_FifthChallengeInvalidAssignmentAndPrintInputs_ReturnFailure(string playerInput)
        {
            ValidationResult result = validator.Validate(fifthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.FeedbackMessage, Is.Not.Empty);
        }

        [Test]
        public void Validate_FifthChallengeRichTextValue_EscapesMarkupInConsoleOutput()
        {
            ValidationResult result = validator.Validate(
                fifthChallenge,
                "cliente = \"<color=red>Aiko</color>\"\nprint(cliente)"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Not.Contain("<color"));
            Assert.That(result.ConsoleOutput, Does.Not.Contain("</color>"));
            Assert.That(result.ConsoleOutput, Does.Contain(
                "&lt;color=red&gt;Aiko&lt;/color&gt;"
            ));
        }

        [Test]
        public void Validate_FifthChallengeOversizedInput_IsRejectedBeforePatternMatching()
        {
            string oversizedInput = new string('a', ChallengeValidator.MaxInputLength + 1);

            ValidationResult result = validator.Validate(fifthChallenge, oversizedInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error: instrucción demasiado larga."));
        }

        [TestCase("precio_onigiri = 450\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri=450\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri    =    450\nprint( precio_onigiri )")]
        public void Validate_SixthChallengeValidNumericAssignmentAndPrintVariants_ReturnSuccess(
            string playerInput
        )
        {
            ValidationResult result = validator.Validate(sixthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> 450"));
            Assert.That(result.FeedbackMessage, Is.EqualTo(
                "¡Correcto! Has completado el Módulo 1: Primer turno."
            ));
        }

        [TestCase("precio_onigiri = \"450\"\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 450.0\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 450.00\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 4,50\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 45\nprint(precio_onigiri)")]
        [TestCase("precio = 450\nprint(precio)")]
        [TestCase("precio_onigiri == 450\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 450\nprint(\"450\")")]
        [TestCase("precio_onigiri = 450\nprint(precio_bebida)")]
        [TestCase("print(precio_onigiri)\nprecio_onigiri = 450")]
        [TestCase("precio_onigiri = 450\nprint(precio_onigiri)\nprint(precio_onigiri)")]
        [TestCase("# comentario\nprecio_onigiri = 450\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 450;\nprint(precio_onigiri)")]
        [TestCase("precio_onigiri = 450\\nprint(precio_onigiri)")]
        public void Validate_SixthChallengeInvalidNumericAssignmentAndPrintInputs_ReturnFailure(
            string playerInput
        )
        {
            ValidationResult result = validator.Validate(sixthChallenge, playerInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.FeedbackMessage, Is.Not.Empty);
            Assert.That(result.ConsoleOutput, Does.Not.Contain("<color"));
            Assert.That(result.ConsoleOutput, Does.Not.Contain("</color>"));
        }

        [Test]
        public void Validate_SixthChallengeRichTextValue_DoesNotExposeMarkup()
        {
            ValidationResult result = validator.Validate(
                sixthChallenge,
                "precio_onigiri = <color=red>450</color>\nprint(precio_onigiri)"
            );

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Does.Not.Contain("<color"));
            Assert.That(result.ConsoleOutput, Does.Not.Contain("</color>"));
        }

        [Test]
        public void Validate_SixthChallengeOversizedInput_IsRejectedBeforePatternMatching()
        {
            string oversizedInput = new string('a', ChallengeValidator.MaxInputLength + 1);

            ValidationResult result = validator.Validate(sixthChallenge, oversizedInput);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Error: instrucción demasiado larga."));
        }
    }
}
