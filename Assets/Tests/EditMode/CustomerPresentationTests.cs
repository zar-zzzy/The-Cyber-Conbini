using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CyberConbini.Tests.EditMode
{
    public class CustomerPresentationTests
    {
        private const string ScenePath = "Assets/Scenes/Conbini_Main.unity";

        private Scene scene;
        private bool openedScene;

        [SetUp]
        public void SetUp()
        {
            scene = SceneManager.GetSceneByPath(ScenePath);
            if (!scene.isLoaded)
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
                openedScene = true;
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (openedScene)
            {
                EditorSceneManager.CloseScene(scene, true);
                openedScene = false;
            }
        }

        [Test]
        public void CustomerVisual_HasLoopingHumanoidIdle()
        {
            Animator animator = GetCustomerAnimator();

            Assert.That(animator.avatar, Is.Not.Null);
            Assert.That(animator.avatar.isValid && animator.avatar.isHuman, Is.True);
            Assert.That(animator.runtimeAnimatorController, Is.Not.Null,
                "El cliente no debe aparecer en T-pose.");
            Assert.That(animator.runtimeAnimatorController.animationClips.Any(
                clip => clip != null && clip.isHumanMotion && clip.isLooping), Is.True);
        }

        [Test]
        public void CustomerVisual_IdleDoesNotMoveItsScenePosition()
        {
            Animator animator = GetCustomerAnimator();

            Assert.That(animator.applyRootMotion, Is.False);
        }

        private Animator GetCustomerAnimator()
        {
            GameObject customer = scene.GetRootGameObjects().Single(root => root.name == "Customer");
            Transform visual = customer.transform.Find("Customer_Visual");

            Assert.That(visual, Is.Not.Null);
            Assert.That(visual.gameObject.activeInHierarchy, Is.True);
            return visual.GetComponent<Animator>();
        }
    }
}
