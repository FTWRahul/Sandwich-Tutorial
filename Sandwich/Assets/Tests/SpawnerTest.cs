using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SpawnerTest
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SpawnerGridsElements_BetweenSelectedHeightAndWidth_AreNotNull()
        {
            var spawner = CreateSpawner();
            yield return null;
            bool returnBool = true;
            for (int i = 0; i < Spawner.Height; i++)
            {
                for (int j = 0; j < Spawner.Width; j++)
                {
                    if (spawner.Grid[j, i] == null)
                    {
                        returnBool = false;
                        break;
                    }
                }
                if (returnBool == false)
                {
                    break;
                }
            }
            if (returnBool)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        [UnityTest]
        public IEnumerator SpawnerGridsElements_AfterSelectedHeightAndWidth_AreNull()
        {
            var spawner = CreateSpawner();
            yield return null;
            int x = 0;
            int y = 0;
            bool returnBool = true;
            foreach (var node in spawner.Grid)
            {
                if (node == null)
                {
                    if (x < Spawner.Width - 1 && y < Spawner.Height - 1 )
                    {
                        returnBool = false;
                        break;
                    }
                }
                x++;
                y++;
            }
            if (returnBool)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        public Spawner CreateSpawner()
        {
            Spawner spawner = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Spawner").GetComponent<Spawner>());

            return spawner;
        }
    }
    
}
