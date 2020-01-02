using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests
{
    public class SpawnTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SpawnTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SpawnTestWithEnumeratorPasses()
        {
            //var spawner = new GameObject().AddComponent<Spawner>();

            //Assert.AreEqual(sizeOfGrid, numberOfGameObjects);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
