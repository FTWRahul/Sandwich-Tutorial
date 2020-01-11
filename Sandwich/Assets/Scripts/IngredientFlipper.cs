﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;
    [HideInInspector]
    public IngredientSlice nextSlice;
    private Sequence _nudgeSequence;
    private Sequence _flipSequence;
    private Vector2Int _selectedNeighbour;

    [HideInInspector]
    public int stackCount = 0;
    
    private void Start()
    {
        _nudgeSequence = DOTween.Sequence();
        //_flipSequence = DOTween.Sequence();
        
        _slice = GetComponent<IngredientSlice>();
        float randomDelay = Random.value;
        Invoke(nameof(DelayedDrop), randomDelay);
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
        //Debug.Log("Swipe Object name = " + gameObject.name);
        if (transform.parent != null)
        {
            //Debug.Log("parent object found");
            //Debug.Log("Parent object name = "+ transform.parent.gameObject.name);
            if (transform.GetComponentInParent<IRespondToTouch>() != null)
            {
                //Debug.Log("Found component in parent");
                nextSlice.GetComponent<IRespondToTouch>().AttemptFlip(dir);
                //transform.GetComponentInParent<IRespondToTouch>().AttemptFlip(dir);
                return;
            }
            //Debug.Log("Did not find component in parent");
        }
        //Debug.Log("parent object not found");
        
        //Debug.Log("TouchResponse from node " + _slice.Node.pos);
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
                        nextSlice = Spawner.itemsOnBoard[i];
                        break;
                    }
                }
            }
        }
        if (nextSlice != null)
        {
            Debug.Log("Current Stack count for object = " + gameObject.name + "  " + stackCount);
            Debug.Log("Next Stack count for object = " + nextSlice.gameObject.name + "  " + nextSlice.GetComponent<IngredientFlipper>().stackCount);
            int displacement = stackCount + (nextSlice.GetComponent<IngredientFlipper>().stackCount + 1);
            ICommand swipeCommand = new SwipeCommand(_slice, nextSlice, transform.position, transform.rotation, flipDirection, _slice.Node, displacement);
            swipeCommand.Execute();
            Spawner.commands.Add(swipeCommand);
            Debug.Log("Spawwner confirmation " + Spawner.commands.Count);
            //StartCoroutine(FlipSlice(flipDirection, _nextSlice, displacement));
            nextSlice.GetComponent<IngredientFlipper>().stackCount = displacement;
            return;
        }
        StartCoroutine(NudgeSlice(flipDirection));
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

