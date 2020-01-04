using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IngredientSlice : MonoBehaviour , IRespondToTouch
{
    public IngredientSO ingredientData; 
    MeshRenderer _mRenderer;
    private Node _node;

    private void Awake()
    {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _mRenderer = GetComponent<MeshRenderer>();
        _mRenderer.material = material;
    }

    public void Init(Node node)
    {
        _mRenderer.material.SetColor("_BaseColor", ingredientData.tempColor);
        transform.localScale = new Vector3(1, .5f, 1);
        gameObject.name = ingredientData.ingredientName;
        float randomDelay = Random.value;
        _node = node;
        Invoke(nameof(DelayedDrop), randomDelay);
    }

    private void DelayedDrop()
    {
        try
        {
           // Debug.Log("Trying");
            transform.DOMoveY(0, .5f).SetEase(ingredientData.ease);
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
        Debug.Log("TouchResponse from node " + _node.pos);
        List<Vector2Int> neighbouringNodes = _node.GetNeighbours();
        for (int i = 0; i < neighbouringNodes.Count; i++)
        {
            neighbouringNodes[i] += _node.pos;
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

public interface IRespondToTouch
{
    void AttemptFlip(Vector3 dir);
}