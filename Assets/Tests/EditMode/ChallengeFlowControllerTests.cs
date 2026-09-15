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
    }
}
