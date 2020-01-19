using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class LevelEnd : MonoBehaviour , IRespondToTouch
{
    public int biteCount = 1;

    public int maxBites = 3;
    
    [ContextMenu("TakeBite")]
    public void TakeBite()
    {
        if (biteCount <= maxBites)
        {
            foreach (var ingredient in Spawner.itemsOnBoard)
            {
                ingredient.modelList[biteCount - 1].SetActive(false);
                if (biteCount != maxBites)
                {
                    ingredient.modelList[biteCount].SetActive(true);
                }
            }

            biteCount++;
        }
    }

    public void AttemptFlip(Vector3 dir)
    {
        TakeBite();
    }
}
