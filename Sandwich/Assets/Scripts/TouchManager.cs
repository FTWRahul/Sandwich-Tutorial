using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    Touch _recievedTouch;
    Vector3 _startPoint;
    Vector3 _endPoint;
    private IRespondToTouch _itemTouched;
    [SerializeField] private float _deadZone;

    private Camera _mainCam;
    
    void Awake()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit;
            _recievedTouch = Input.GetTouch(0);
            Ray ray = _mainCam.ScreenPointToRay(_recievedTouch.position);
            if (Physics.Raycast(ray, out hit))
            {
                IRespondToTouch hitRisponder = null;
                if (_itemTouched == null)
                {
                    try
                    {
                        hitRisponder = hit.transform.GetComponent<IRespondToTouch>();
                        _itemTouched = hitRisponder;
                        _startPoint = hit.point;
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (_itemTouched != null)
                {
                    if (_recievedTouch.phase == TouchPhase.Ended || _recievedTouch.phase == TouchPhase.Canceled)
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
    }
}
