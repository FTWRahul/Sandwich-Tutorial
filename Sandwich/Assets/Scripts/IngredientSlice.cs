using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IngredientSlice : MonoBehaviour 
{
    public IngredientSO ingredientData; 
    MeshRenderer _mRenderer;
    private Node _node;

    public Node Node => _node;

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
        _node = node;
    }
}