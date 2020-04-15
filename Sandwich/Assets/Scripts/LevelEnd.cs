using System.Threading.Tasks;
using UnityEngine;

public class LevelEnd : IRespondToTouch
{
    public static int biteCount = 1;

    public int maxBites = 3;
    
    /// <summary>
    /// Non mono hit responder for pure logic of bite.
    /// </summary>
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
            AudioManager.instance.PlayBiteSound();
            biteCount++;
            if (biteCount > maxBites)
            {
                NewLevel();
            }
        }
    }

    //calls the new level
    public async void NewLevel()
    {
        await Task.Delay(500);
        Object.FindObjectOfType<Spawner>().ResetGame();
    }

    //Interface implementation 
    public void AttemptFlip(Vector3 dir)
    {
        TakeBite();
    }
}
