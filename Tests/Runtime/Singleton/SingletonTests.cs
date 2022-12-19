using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using d4160.ResourceUtils;
using UnityEditor;

namespace d4160.Singleton.Tests.Runtime
{
    public class _SingletonTests
    {
        // Tests - call a Singleton
        // That exists
        // That doesn't exists and have a prefab
        // That doesn't exists and haven't a prefab

        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        public SingletonTests CreateNewSingleton()
        {
            GameObject newGO = new GameObject("SingletonSO");
            return newGO.AddComponent<SingletonTests>();
        }

        [UnityTest]
        public IEnumerator GetSingletonFromHierarchy()
        {
            SingletonTests newSingleton = CreateNewSingleton();
            SingletonTests getSingleton = SingletonTests.Instance;

            Assert.AreEqual(newSingleton, getSingleton);

            yield return null;

            Object.DestroyImmediate(newSingleton.gameObject);
        }

        [UnityTest]
        public IEnumerator GetSingletonFromResources()
        {
            if(!Directory.Exists($"{Application.dataPath}{Path.DirectorySeparatorChar}Resources"))
            {
                Directory.CreateDirectory($"{Application.dataPath}{Path.DirectorySeparatorChar}Resources");
            }

            string prefabPath = "Assets/Resources/TemporaryTestSingleton.prefab";

            SingletonTests newSingleton = CreateNewSingleton();
            PrefabUtility.SaveAsPrefabAsset(newSingleton.gameObject, prefabPath);

            yield return null;

            Object.DestroyImmediate(newSingleton.gameObject);

            SingletonTests getSingleton = SingletonTests.Instance;

            Assert.AreEqual($"{typeof(SingletonTests)} Singleton (R)", getSingleton.name);

            yield return null;

            AssetDatabase.DeleteAsset(prefabPath);

            Object.DestroyImmediate(getSingleton.gameObject);
        }

        [UnityTest]
        public IEnumerator GetSingletonFromNewGO()
        {
            SingletonTests getSingleton = SingletonTests.Instance;

            Assert.AreEqual($"{typeof(SingletonTests)} Singleton (New)", getSingleton.name);

            yield return null;

            Object.DestroyImmediate(getSingleton.gameObject);
        }

        [UnityTest]
        public IEnumerator CheckUniqueInstance()
        {
            SingletonTests newSingleton1 = CreateNewSingleton();
            SingletonTests newSingleton2 = CreateNewSingleton();
            SingletonTests newSingleton3 = CreateNewSingleton();

            SingletonTests getSingleton = SingletonTests.Instance;

            Assert.AreEqual(getSingleton, newSingleton1);

            SingletonTests[] singletons = Object.FindObjectsOfType<SingletonTests>();

            Assert.AreEqual(1, singletons.Length);

            yield return null;

            Object.DestroyImmediate(getSingleton.gameObject);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }
    }

    [ResourcesPath("TemporaryTestSingleton")]
    public class SingletonTests : Singleton<SingletonTests>
    {
    }
}