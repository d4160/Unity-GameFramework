using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using d4160.Core.MonoBehaviours;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace d4160.Core.Tests.Runtime
{
    public class DontDestroyOnLoadTests
    {
        [OneTimeSetUp]
        public void Setup()
        {

        }

        [UnityTest]
        public IEnumerator TestDontDestroyOnLoad()
        {
            // Arrange
            var scene = SceneManager.CreateScene("New Test Scene");
            SceneManager.SetActiveScene(scene);

            // Act
            GameObject obj = new GameObject("DontDestroyOnLoad");
            obj.AddComponent<DontDestroyOnLoad>();

            yield return null;

            yield return SceneManager.UnloadSceneAsync(scene);

            // Assert
            Assert.IsTrue(GameObject.Find("DontDestroyOnLoad"));

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestDestroyOnLoad()
        {
            // Arrange
            var scene = SceneManager.CreateScene("New Test Scene");
            SceneManager.SetActiveScene(scene);

            // Act
            new GameObject("DestroyableObject");

            yield return null;

            yield return SceneManager.UnloadSceneAsync(scene);

            // Assert
            Assert.IsFalse(GameObject.Find("DestroyableObject"));

            yield return null;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }
    }
}