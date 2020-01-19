﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    private Touch _receivedTouch;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private IRespondToTouch _itemTouched;
    [SerializeField] private float _deadZone = .5f;

    private Camera _mainCam;
    
    void Awake()
    {
        _mainCam = Camera.main;
    }

    public void ResetIResponder()
    {
        _itemTouched = null;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit;
            _receivedTouch = Input.GetTouch(0);
            Ray ray = _mainCam.ScreenPointToRay(_receivedTouch.position);
            if (Physics.Raycast(ray, out hit))
            {
                IRespondToTouch hitResponder = null;
                if (_itemTouched == null)
                {
                    try
                    {
                        hitResponder = hit.transform.GetComponent<IRespondToTouch>();
                        _itemTouched = hitResponder;
                        _startPoint = hit.point;
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (_itemTouched != null)
                {
                    if (_receivedTouch.phase == TouchPhase.Ended || _receivedTouch.phase == TouchPhase.Canceled)
                    {
                        _endPoint = hit.point;
                        Vector3 swipeDirection = (_endPoint - _startPoint);
                        if (swipeDirection.magnitude >= _deadZone)
                        {
                            _itemTouched.AttemptFlip(swipeDirection);
                        }
                        _itemTouched = null;
                    }
                }
            }
        }

        if (Input.touchCount < 0)
        {
            _itemTouched = null;
        }
    }
}
