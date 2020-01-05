using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;

    private void Start()
    {
        _slice = GetComponent<IngredientSlice>();
        float randomDelay = Random.value;
        Invoke(nameof(DelayedDrop), randomDelay);
    }

    private void DelayedDrop()
    {
        try
        {
            // Debug.Log("Trying");
            transform.DOMoveY(0, .5f).SetEase(_slice.ingredientData.ease);
        }
        catch (Exception e)
        {
            // Debug.Log("Caught");
            Console.WriteLine(e);
            throw;
        }
    }

    public void AttemptFlip(Vector3 dir)
    {
        Debug.Log("TouchResponse from node " + _slice.Node.pos);
        List<Vector2Int> neighbouringNodes = _slice.Node.GetNeighbours();
        for (int i = 0; i < neighbouringNodes.Count; i++)
        {
            neighbouringNodes[i] += _slice.Node.pos;
        }
        
        float right = Vector3.Angle(dir, Vector3.right);
        float left = Vector3.Angle(dir,  Vector3.left);
        float up = Vector3.Angle(dir, Vector3.forward);
        float down = Vector3.Angle(dir, Vector3.back);
        
        float a = GetSmaller(right, left);
        float b = GetSmaller(up, down);
        float final = GetSmaller(a, b);

        if (final == right)
        {
            Debug.Log("Flip Right");
        }
        else if (final == left)
        {
            Debug.Log("Flip Left");
        }
        else if (final == up)
        {
            Debug.Log("Flip Up");
        }
        else
        {
            Debug.Log("Flip Down");
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
