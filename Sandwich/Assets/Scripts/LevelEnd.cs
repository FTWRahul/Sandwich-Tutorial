using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class LevelEnd : IRespondToTouch
{
    public static int biteCount = 1;

    public int maxBites = 3;
    
    public void TakeBite()
    {
        //Debug.Log(biteCount + "Count should be");
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
            //Debug.Log(biteCount + "Count increased");
        }
        
    }

    public void AttemptFlip(Vector3 dir)
    {
        TakeBite();
        //Debug.Log("Called");
    }
}
