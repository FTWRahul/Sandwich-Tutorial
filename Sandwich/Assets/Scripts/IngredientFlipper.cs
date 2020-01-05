using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;
    private Sequence NudgeSequence;
    
    private void Start()
    {
        NudgeSequence = DOTween.Sequence();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(NudgeSlice(Vector3.right));
        }
    }

    public void AttemptFlip(Vector3 dir)
    {
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
                break;
            }
        }
        for (int i = 0; i < neighbouringNodes.Count; i++)
        {
            neighbouringNodes[i] += _slice.Node.pos;
        }

        Vector3 nudgeDir = NudgeDirection(swipeDirection);
        StartCoroutine(NudgeSlice(nudgeDir));
        //if exists, get that slices node pos / gameobj and stack count
        //else nudge
    }

    private IEnumerator NudgeSlice(Vector3 dir)
    {
        NudgeSequence.Complete();
        NudgeSequence.Prepend(transform.DOLocalRotate(dir * 10f, .2f, RotateMode.WorldAxisAdd));
        yield return new WaitForSeconds(.2f);
        NudgeSequence.Append(transform.DOLocalRotate(Vector3.zero, 1f)
            .SetEase(Ease.OutElastic));
    }

    private Vector3 NudgeDirection(Vector3 dir)
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
