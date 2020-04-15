using System;
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
    private float timeSinceLastTap;
    private LevelEnd _endResponder;
    private Camera _mainCam;
    
    void Awake()
    {
        _endResponder = new LevelEnd();
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
            //Checking if level is over
            if (IngredientFlipper.hasWon && timeSinceLastTap > .5f)
            {
                TakeBite();
                return;
            }
            else if (!IngredientFlipper.hasWon)
            {
                //assign hit responder
                RaycastHit hit;
                _receivedTouch = Input.GetTouch(0);
                Ray ray = _mainCam.ScreenPointToRay(_receivedTouch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    IRespondToTouch hitResponder = null;
                    if (_itemTouched == null)
                    {
                        //Error handling hit response
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
                        //when touch ends calls the interface method on the responder.
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
        }

        //setting responder to null
        if (Input.touchCount < 0)
        {
            _itemTouched = null;
        }
        timeSinceLastTap += Time.deltaTime;
    }

    /// <summary>
    /// Swaps hit responder
    /// </summary>
    [ContextMenu("TakeBite")]
    private void TakeBite()
    {
        timeSinceLastTap = 0;
        _itemTouched = _endResponder;
        _itemTouched.AttemptFlip(Vector3.one);
    }
}
