using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    private string gameID = "3515370";
    private string placementID = "Banner";

    public bool testMode;

    public void DisplayBanner(bool shouldDisplay)
    {
        if (shouldDisplay)
        {
            StartCoroutine(ShowAds());
        }
        else
        {
            StopCoroutine(ShowAds());
            Advertisement.Banner.Hide();
        }
    }
    
    private IEnumerator ShowAds()
    {
        Advertisement.Initialize(gameID, testMode);

        while (!Advertisement.IsReady(placementID))
        {
            yield return null;
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placementID);
    }
}
