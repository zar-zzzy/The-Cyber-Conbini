using CyberConbini.Gameplay;
using NUnit.Framework;
using UnityEditor;

namespace CyberConbini.Tests.EditMode
{
    public class ChallengeCatalogTests
    {
        private const string CatalogPath = "Assets/Data/Challenges/Module1ChallengeCatalog.asset";

        [Test]
        public void Module1Catalog_LoadsWithOnlyFirstChallenge()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);

            Assert.That(catalog, Is.Not.Null);
            Assert.That(catalog.ModuleId, Is.EqualTo("M1"));
            Assert.That(catalog.ModuleTitle, Is.EqualTo("Módulo 1 — Primer turno"));
            Assert.That(catalog.PlannedChallengeCount, Is.EqualTo(6));
            Assert.That(catalog.ChallengeCount, Is.EqualTo(1));
            Assert.That(catalog.GetChallenge(0).Id, Is.EqualTo("M1_R1"));
        }

        [Test]
        public void FirstChallenge_ContainsAllRequiredPrintLiteralData()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);
            Assert.That(catalog, Is.Not.Null);

            ChallengeDefinition challenge = catalog.GetChallenge(0);

            Assert.That(challenge.Id, Is.EqualTo("M1_R1"));
            Assert.That(challenge.Title, Is.EqualTo("Saludo inicial"));
            Assert.That(challenge.Prompt, Is.EqualTo(
                "El primer cliente ha llegado. Muestra el siguiente mensaje en la terminal:"
            ));
            Assert.That(challenge.TargetDisplayText, Is.EqualTo("Bienvenido al Cyber-Conbini"));
            Assert.That(challenge.Hint, Is.EqualTo(
                "Pista: usa print() y escribe el mensaje entre comillas."
            ));
            Assert.That(challenge.ExpectedOutput, Is.EqualTo("Bienvenido al Cyber-Conbini"));
            Assert.That(challenge.SuccessFeedback, Is.EqualTo(
                "¡Correcto! El cliente ha sido recibido."
            ));
            Assert.That(challenge.ValidationType, Is.EqualTo(ChallengeValidationType.PrintLiteral));
            Assert.That(challenge.Rules, Is.Not.Null);
            Assert.That(challenge.Rules.ExpectedValue, Is.EqualTo("Bienvenido al Cyber-Conbini"));
            Assert.That(challenge.HasRequiredData, Is.True);
        }
    }
}
