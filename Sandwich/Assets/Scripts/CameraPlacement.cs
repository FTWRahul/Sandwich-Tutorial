using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlacement : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        //PlaceCamera();
    }

    public void PlaceCamera()
    {
        var bounds = new Bounds(Spawner.itemsOnBoard[0].transform.position, Vector3.one);
        for (int i = 1; i < Spawner.itemsOnBoard.Count; i++)
        {
            bounds.Encapsulate(Spawner.itemsOnBoard[i].transform.position);
        }
        transform.position = new Vector3(bounds.center.x, transform.position.y,  bounds.center.z - bounds.size.y) ;   
    }

}
