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
    
    [SerializeField]
    private GameObject _noBite;
    [SerializeField]
    private GameObject _oneBite;
    [SerializeField]
    private GameObject _twoBite;
    
    public List<GameObject> modelList = new List<GameObject>();
    
    public Node Node
    {
        get { return _node; }
        set { _node = value; }
    }

    private void Awake()
    {
        modelList.Add(_noBite);
        modelList.Add(_oneBite);
        modelList.Add(_twoBite);
        //Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        //_mRenderer = GetComponent<MeshRenderer>();
        //_mRenderer.material = material;
    }

    public void Init(Node node)
    {
        //_mRenderer.material.SetColor("_BaseColor", ingredientData.tempColor);
        //transform.localScale = new Vector3(1, .25f, 1);
        gameObject.name = ingredientData.ingredientName;
        _node = node;
    }
}