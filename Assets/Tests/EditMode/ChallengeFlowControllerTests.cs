using CyberConbini.Gameplay;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace CyberConbini.Tests.EditMode
{
    public class ChallengeFlowControllerTests
    {
        private const string CatalogPath = "Assets/Data/Challenges/Module1ChallengeCatalog.asset";

        private GameObject testGameObject;

        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
        }

        [Test]
        public void Initialize_StartsAtFirstCatalogChallenge()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);
            Assert.That(catalog, Is.Not.Null);

            testGameObject = new GameObject("ChallengeFlowControllerTests");
            ChallengeValidator validator = testGameObject.AddComponent<ChallengeValidator>();
            ChallengeFlowController flowController =
                testGameObject.AddComponent<ChallengeFlowController>();

            bool initialized = flowController.Initialize(catalog, validator);

            Assert.That(initialized, Is.True);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(0));
            Assert.That(flowController.CurrentChallenge, Is.SameAs(catalog.GetChallenge(0)));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R1"));
            Assert.That(flowController.PlannedChallengeCount, Is.EqualTo(6));
        }

        [Test]
        public void TryAdvance_BeforeSolvingFirstChallenge_DoesNotAdvance()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.False);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(0));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R1"));
            Assert.That(flowController.CanAdvance, Is.False);
        }

        [Test]
        public void SolveFirstAndTryAdvance_MovesOnceToSecondChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();

            ValidationResult result = flowController.ValidateCurrent(
                "print(\"Bienvenido al Cyber-Conbini\")"
            );
            bool advanced = flowController.TryAdvance();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(advanced, Is.True);
            Assert.That(flowController.IsChallengeCompleted("M1_R1"), Is.True);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(1));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R2"));
            Assert.That(flowController.PlannedChallengeCount, Is.EqualTo(6));
            Assert.That(flowController.CanAdvance, Is.False);
        }

        [Test]
        public void TryAdvance_TwiceWithoutSolvingSecondChallenge_DoesNotMoveAgain()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            flowController.ValidateCurrent("print(\"Bienvenido al Cyber-Conbini\")");

            bool firstAdvance = flowController.TryAdvance();
            bool secondAdvance = flowController.TryAdvance();

            Assert.That(firstAdvance, Is.True);
            Assert.That(secondAdvance, Is.False);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(1));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R2"));
            Assert.That(flowController.IsChallengeCompleted("M1_R1"), Is.True);
        }

        [Test]
        public void SolveSecondChallenge_AdvancesToThirdChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            flowController.ValidateCurrent("print(\"Bienvenido al Cyber-Conbini\")");
            flowController.TryAdvance();

            ValidationResult result = flowController.ValidateCurrent(
                "print(\"Turno nocturno iniciado\")"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(flowController.IsChallengeCompleted("M1_R2"), Is.True);
            Assert.That(flowController.CanAdvance, Is.True);

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.True);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(2));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R3"));
            Assert.That(flowController.CanAdvance, Is.False);
        }

        [Test]
        public void SolveThirdChallenge_AdvancesToFourthChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            flowController.ValidateCurrent("print(\"Bienvenido al Cyber-Conbini\")");
            flowController.TryAdvance();
            flowController.ValidateCurrent("print(\"Turno nocturno iniciado\")");
            flowController.TryAdvance();

            ValidationResult result = flowController.ValidateCurrent(
                "print(\"Onigiri de salmón\")"
            );

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(flowController.IsChallengeCompleted("M1_R3"), Is.True);
            Assert.That(flowController.CanAdvance, Is.True);

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.True);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(3));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R4"));
            Assert.That(flowController.CanAdvance, Is.False);
        }

        [Test]
        public void TryAdvance_FromThirdBeforeSolving_DoesNotAdvanceToFourthChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            flowController.ValidateCurrent("print(\"Bienvenido al Cyber-Conbini\")");
            flowController.TryAdvance();
            flowController.ValidateCurrent("print(\"Turno nocturno iniciado\")");
            flowController.TryAdvance();

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.False);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(2));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R3"));
        }

        [Test]
        public void SolveFourthChallenge_AdvancesToFifthChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            flowController.ValidateCurrent("print(\"Bienvenido al Cyber-Conbini\")");
            flowController.TryAdvance();
            flowController.ValidateCurrent("print(\"Turno nocturno iniciado\")");
            flowController.TryAdvance();
            flowController.ValidateCurrent("print(\"Onigiri de salmón\")");
            flowController.TryAdvance();

            ValidationResult result = flowController.ValidateCurrent("cliente = \"Aiko\"");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(flowController.IsChallengeCompleted("M1_R4"), Is.True);
            Assert.That(flowController.IsCurrentChallengeCompleted, Is.True);
            Assert.That(flowController.CanAdvance, Is.True);

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.True);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(4));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R5"));
            Assert.That(flowController.CanAdvance, Is.False);
        }

        [Test]
        public void TryAdvance_FromFourthBeforeSolving_DoesNotAdvanceToFifthChallenge()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            SolveAndAdvance(flowController, "print(\"Bienvenido al Cyber-Conbini\")");
            SolveAndAdvance(flowController, "print(\"Turno nocturno iniciado\")");
            SolveAndAdvance(flowController, "print(\"Onigiri de salmón\")");

            bool advanced = flowController.TryAdvance();

            Assert.That(advanced, Is.False);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(3));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R4"));
        }

        [Test]
        public void SolveFifthChallenge_WhenNoSixthChallenge_CannotAdvancePastCatalog()
        {
            ChallengeFlowController flowController = CreateInitializedFlow();
            SolveAndAdvance(flowController, "print(\"Bienvenido al Cyber-Conbini\")");
            SolveAndAdvance(flowController, "print(\"Turno nocturno iniciado\")");
            SolveAndAdvance(flowController, "print(\"Onigiri de salmón\")");
            SolveAndAdvance(flowController, "cliente = \"Aiko\"");

            ValidationResult result = flowController.ValidateCurrent(
                "cliente = \"Aiko\"\nprint(cliente)"
            );
            bool advanced = flowController.TryAdvance();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ConsoleOutput, Is.EqualTo("> Aiko"));
            Assert.That(flowController.IsChallengeCompleted("M1_R5"), Is.True);
            Assert.That(flowController.IsCurrentChallengeCompleted, Is.True);
            Assert.That(flowController.CanAdvance, Is.False);
            Assert.That(advanced, Is.False);
            Assert.That(flowController.CurrentIndex, Is.EqualTo(4));
            Assert.That(flowController.CurrentChallenge.Id, Is.EqualTo("M1_R5"));
        }

        private static void SolveAndAdvance(ChallengeFlowController flowController, string input)
        {
            ValidationResult result = flowController.ValidateCurrent(input);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(flowController.TryAdvance(), Is.True);
        }

        private ChallengeFlowController CreateInitializedFlow()
        {
            ChallengeCatalog catalog = AssetDatabase.LoadAssetAtPath<ChallengeCatalog>(CatalogPath);
            Assert.That(catalog, Is.Not.Null);

            testGameObject = new GameObject("ChallengeFlowControllerTests");
            ChallengeValidator validator = testGameObject.AddComponent<ChallengeValidator>();
            ChallengeFlowController flowController =
                testGameObject.AddComponent<ChallengeFlowController>();

            Assert.That(flowController.Initialize(catalog, validator), Is.True);
            return flowController;
        }
    }
}
