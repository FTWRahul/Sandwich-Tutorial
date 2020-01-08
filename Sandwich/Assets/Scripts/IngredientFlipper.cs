using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;
    private IngredientSlice _nextSlice;
    private Sequence _nudgeSequence;
    private Sequence _flipSequence;
    private Vector2Int _selectedNeighbour;

    [HideInInspector]
    public int stackCount = 0;
    public int childCount = 0;
    
    private static List<IngredientFlipper> _ingredientFlippers = new List<IngredientFlipper>();
    
    private void Start()
    {
        _ingredientFlippers.Add(this);
        //Debug.Log(_ingredientFlippers.Count);

        _nudgeSequence = DOTween.Sequence();
        _flipSequence = DOTween.Sequence();
        
        _slice = GetComponent<IngredientSlice>();
        float randomDelay = Random.value;
        Invoke(nameof(DelayedDrop), randomDelay);
    }

    private void OnDisable()
    {
        _ingredientFlippers.Remove(this);
        //Debug.Log("Removed "+ _ingredientFlippers.Count);
    }

    private void DelayedDrop()
    {
        try
        {
            transform.DOMoveY(0, .5f).SetEase(_slice.ingredientData.ease);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void AttemptFlip(Vector3 dir)
    {
        Debug.Log("Swipe Object name = " + gameObject.name);
        if (transform.parent != null)
        {
            Debug.Log("parent object found");
            Debug.Log("Parent object name = "+ transform.parent.gameObject.name);
            if (transform.GetComponentInParent<IRespondToTouch>() != null)
            {
                Debug.Log("Found component in parent");
                _nextSlice.GetComponent<IRespondToTouch>().AttemptFlip(dir);
                //transform.GetComponentInParent<IRespondToTouch>().AttemptFlip(dir);
                return;
            }
            Debug.Log("Did not find component in parent");
        }
        Debug.Log("parent object not found");
        
        Debug.Log("TouchResponse from node " + _slice.Node.pos);
        List<Vector2Int> neighbouringNodes = _slice.Node.GetNeighbours();
        
        Vector3 swipeDirection = GetSwipeDirection(dir);
        Vector2Int internalDir = new Vector2Int(Mathf.RoundToInt(swipeDirection.x), Mathf.RoundToInt(swipeDirection.z));

        bool exists = false;
        for (int i = 0; i < neighbouringNodes.Count; i++)
        {
            if (internalDir == neighbouringNodes[i])
            {
                exists = true;
                _selectedNeighbour = neighbouringNodes[i] + _slice.Node.pos;
                break;
            }
        }
        
        Vector3 flipDirection = FlipDirection(swipeDirection);

        if (exists)
        {
            for (int i = 0; i < Spawner.itemsOnBoard.Count; i++)
            {
                if (Spawner.itemsOnBoard[i].Node != null)
                {
                    if (Spawner.itemsOnBoard[i].Node.pos == _selectedNeighbour)
                    {
                        _nextSlice = Spawner.itemsOnBoard[i];
                        break;
                    }
                }
            }

            //Get child count of the current slice
            //get child count of the next slice
            //float displacement = GetComponentsInChildren<IngredientFlipper>().Length + _nextSlice.GetComponentsInChildren<IngredientFlipper>().Length;
            //int currentChildCount = GetTreeChildCountReal(transform);
            //childCount = 0;
            //int nextChildCount = GetTreeChildCountReal(_nextSlice.transform) + 1;
            //childCount = 0;
            
            
            Debug.Log("Current Stack count for object = " + gameObject.name + "  " + stackCount);
            Debug.Log("Next Stack count for object = " + _nextSlice.gameObject.name + "  " + _nextSlice.GetComponent<IngredientFlipper>().stackCount);
            int displacement = stackCount + (_nextSlice.GetComponent<IngredientFlipper>().stackCount + 1);
            //Debug.Log("My stack count = " +gameObject.name + "    "+ this.stackCount);
            StartCoroutine(FlipSlice(flipDirection, _nextSlice, displacement));
            _nextSlice.GetComponent<IngredientFlipper>().stackCount = displacement;
        }
        else
        {
            StartCoroutine(NudgeSlice(flipDirection));
        }
    }

//    public int GetTreeChildCountReal( Transform inTransform)
//    {
//        if (inTransform.childCount > 0)
//        {
//            childCount += inTransform.childCount;
//
//            return GetTreeChildCountReal(inTransform.GetChild(0));
//        }
//        return childCount;
//    }

    private IEnumerator FlipSlice(Vector3 dir, IngredientSlice next, float yDisplacement)
    {
        _flipSequence.Complete();
        Debug.Log("YDISPLACEMENT =" + yDisplacement);
        //Debug.Log("Model Height =" + IngredientSO.ModelHeight);
        Vector3 pos = next.transform.position;
        _flipSequence.Prepend(transform.DOJump(new Vector3(pos.x, yDisplacement * .25f, pos.z), (yDisplacement * .25f) + 1, 1, .5f)
                .SetEase(Ease.OutQuad));
        _flipSequence.Join(transform.DORotate(dir * 180, .5f));
        yield return new WaitForSeconds(.6f);
        transform.parent = _nextSlice.transform;
        _slice.Node = null;
    }
    
    private IEnumerator NudgeSlice(Vector3 dir)
    {
        _nudgeSequence.Complete();
        _nudgeSequence.Prepend(transform.DOLocalRotate(dir * 10f, .2f, RotateMode.WorldAxisAdd));
        yield return new WaitForSeconds(.2f);
        _nudgeSequence.Append(transform.DOLocalRotate(Vector3.zero, .5f)
            .SetEase(Ease.OutElastic));
    }
    
    private Vector3 FlipDirection(Vector3 dir)
    {
        if (dir == Vector3.right)
        {
            return Vector3.back;
        }
        else if (dir == Vector3.left)
        {
            return Vector3.forward;
        }
        else if (dir == Vector3.forward)
        {
            return Vector3.right;
        }
        else
        {
            return Vector3.left;
        }
    }
    private Vector3 GetSwipeDirection(Vector3 dir)
    {
        float right = Vector3.Angle(dir, Vector3.right);
        float left = Vector3.Angle(dir, Vector3.left);
        float forward = Vector3.Angle(dir, Vector3.forward);
        float back = Vector3.Angle(dir, Vector3.back);

        float a = GetSmaller(right, left);
        float b = GetSmaller(forward, back);
        float final = GetSmaller(a, b);

        if (final == right)
        {
            Debug.Log("Flip Right");
            return Vector3.right;
        }
        else if (final == left)
        {
            Debug.Log("Flip Left");
            return Vector3.left;
        }
        else if (final == forward)
        {
            Debug.Log("Flip Up");
            return Vector3.forward;
        }
        else
        {
            Debug.Log("Flip Down");
            return Vector3.back;
        }
    }

    float GetSmaller(float a, float b)
    {
        if (a < b)
        {
            return a;
        }
        else
        {
            return b;
        }
    }
}

