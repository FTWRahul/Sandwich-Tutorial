using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraPlacement : MonoBehaviour
{
    private Vector3 _originalPos;
    private Quaternion _originalRot;
    //public Transform endPos;
    private bool _lookAt;

    private void Awake()
    {
        _originalPos = transform.position;
        _originalRot = transform.rotation;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //PlaceCamera();
        IngredientFlipper.winEvent.AddListener(EndGameCameraTransition);
    }

    public void PlaceCamera()
    {
        _lookAt = false;
        transform.position = _originalPos;
        transform.rotation = _originalRot;
        var bounds = new Bounds(Spawner.itemsOnBoard[0].transform.position, Vector3.one);
        for (int i = 1; i < Spawner.itemsOnBoard.Count; i++)
        {
            bounds.Encapsulate(Spawner.itemsOnBoard[i].transform.position);
        }
        transform.position = new Vector3(bounds.center.x, transform.position.y,  bounds.center.z - bounds.size.y) ;   
    }

    [ContextMenu("TestCamRotation")]
    void EndGameCameraTransition()
    {
        StartCoroutine(RotateCamera());
    }

    IEnumerator RotateCamera()
    {
        yield return new WaitForSeconds(1f);
        var bounds = new Bounds(Spawner.itemsOnBoard[0].transform.position, Vector3.one);
        for (int i = 1; i < Spawner.itemsOnBoard.Count; i++)
        {
            bounds.Encapsulate(Spawner.itemsOnBoard[i].transform.position);
        }
        //Debug.Log(bounds.center);
        //GameObject boundGizmo = new GameObject("Bounds");
        //boundGizmo.transform.position = bounds.center;
        Vector3 endPosition = new Vector3(bounds.center.x - 2, 3, bounds.center.z /2);
        _lookAt = true;
        transform.DOMove(endPosition, 2f);
        //transform.DORotate(endPos.rotation.eulerAngles, 1f);
        //yield return new WaitForSeconds(1f);
        //transform.DOLookAt(bounds2.center, .5f);
    }

    private void Update()
    {
        if (_lookAt)
        {
            var bounds2 = new Bounds(Spawner.itemsOnBoard[0].transform.position, Vector3.one * .25f);
            for (int i = 1; i < Spawner.itemsOnBoard.Count; i++)
            {
                bounds2.Encapsulate(Spawner.itemsOnBoard[i].transform.position);
            }
            transform.LookAt(bounds2.center);
        }
    }
}
