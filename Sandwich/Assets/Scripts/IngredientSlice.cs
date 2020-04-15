using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour class for creating new ingredients.
/// </summary>
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

    public GameObject particles;
    
    
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
    }

    /// <summary>
    /// Takes in a node and populates info
    /// </summary>
    /// <param name="node"></param>
    public void Init(Node node)
    {
        gameObject.name = ingredientData.ingredientName;
        _node = node;
    }
}