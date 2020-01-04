using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    //Grid variables
    private static int _height;
    private static int _width;
    private int _maxIngredients;
    private Node[,] _grid;

    public Node[,] Grid => _grid;

    //Getter Properties
    public static int Width => _width;
    public static int Height => _height;
    
    //Exposed variables in inspector
    [SerializeField]
    private Vector2Int heightMinMax;
    [SerializeField]
    private Vector2Int widthMinMax;
    [SerializeField]
    private Vector2Int ingredientMinMax;
    [SerializeField]
    private IngredientSO breadSo;
    [SerializeField]
    private List<IngredientSO> ingredientsToSpawn;
    
    //Recursive variables
    private Stack<IngredientSO> _ingredientStack;
    private List<Vector2Int> _unOccupiedNodes = new List<Vector2Int>();
    private Vector2Int _pickedNode = new Vector2Int(-1,-1);
    private readonly Vector2Int _nullNode = new Vector2Int(-1,-1);
    private WeightedRandom<Vector2Int> _wRand;
    
    private static int _depth = 0;
    //private static int _ingredientsSpawned = 0;

    public static List<GameObject> itemsOnBoard = new List<GameObject>();
    private List<IngredientSO> _tempingredientsList = new List<IngredientSO>();

    
    private void Awake()
    {
        _wRand = new WeightedRandom<Vector2Int>();
        
        _ingredientStack = new Stack<IngredientSO>();
        
        _grid = new Node[widthMinMax.y,heightMinMax.y];

        PopulateStack();

        RandomizeBoard();

        CreateGrid();
    }

    private void RandomizeBoard()
    {
        _height = Random.Range(heightMinMax.x, heightMinMax.y);
        _width = Random.Range(widthMinMax.x, widthMinMax.y);
        _maxIngredients = Random.Range(ingredientMinMax.x, ingredientMinMax.y);
    }

    private void PopulateStack()
    {
        _tempingredientsList.Clear();
        for (int i = 0; i < ingredientsToSpawn.Count; i++)
        {
            _tempingredientsList.Add(ingredientsToSpawn[i]);
        }
        for (int i = 0; i < _tempingredientsList.Count; i++)
        {
            int rand = Random.Range(0, _tempingredientsList.Count);
            _ingredientStack.Push(_tempingredientsList[rand]);
            _tempingredientsList.Remove(_tempingredientsList[rand]);
        }
    }

    [ContextMenu("CreateGrid")]
    void CreateGrid()
    {
        //Debug.Log("Creating Grid");
        //Debug.Break();

        //_grid = new Node[_width,_height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                CreateNode(x, y);
            }
        }
        List<Vector2Int> centerPoint = _grid[_width / 2, _height / 2].GetNeighbours();
        _wRand.ResetValues(centerPoint, centerPoint.Count * 10);
        GeneratePattern();
    }

    private void CreateNode(int x, int y)
    {
        Node go = new GameObject("X : " + x + " Y : " + y).AddComponent<Node>();
        var transform1 = go.transform;
        transform1.parent = this.transform;
        transform1.position = new Vector3(x,0,y);
        go.pos.x = x;
        go.pos.y = y;
        _grid[x, y] = go;
    }

    [ContextMenu("GeneratePattern")]
    private void GeneratePattern()
    {
        int xOffset = _width/2 + Random.Range(-1, 1);
        int zOffset = _height/2 +Random.Range(-1, 1);
        PlaceIngredient(_grid[xOffset, zOffset],breadSo,xOffset, zOffset);
        Vector2Int bread1Pos = _grid[xOffset, zOffset].pos;
        List<Vector2Int> possibleBread2Pos = _grid[xOffset, zOffset].GetNeighbours();

        int rand = Random.Range(0, possibleBread2Pos.Count);
        PlaceIngredient(_grid[xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y] ,breadSo,xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y);
        Vector2Int bread2Pos = _grid[xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y].pos;

        bool flip = false;
        int i = 0;
        for (int j = 0; j < 10; j++)
        {
            if (itemsOnBoard.Count > _maxIngredients - 2)
            {
                break;
            }
            if (flip)
            {
                if(UnfoldIngredients(_grid[bread2Pos.x, bread2Pos.y]))
                {
                    _depth = 0;
                    _wRand.ResetWeights();
                }
            }
            else
            {
                if(UnfoldIngredients(_grid[bread1Pos.x, bread1Pos.y]))
                {
                    _depth = 0;
                    _wRand.ResetWeights();
                }
            }
            flip = !flip;
            
        }
    }

    private void PlaceIngredient(Node node ,IngredientSO data , int xPos, int yPos)
    {
        IngredientSlice slice = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<IngredientSlice>();
        slice.transform.position = new Vector3(xPos, 10, yPos);
        slice.ingredientData = data;
        slice.Init(node);
        ToggleNode(xPos, yPos);
        //_ingredientsSpawned++;
        itemsOnBoard.Add(slice.gameObject);
    }

    private bool UnfoldIngredients(Node node)
    {
        _unOccupiedNodes.Clear();
        _unOccupiedNodes = node.GetNeighbours();
        _pickedNode = _nullNode;
        Vector2Int rand;
        for (int i = 0; i < _unOccupiedNodes.Count; i++)
        {    
            if (_unOccupiedNodes.Count == 1)
            {
                float coinToss = Random.value;
                if (coinToss < .7f)
                {
                    return true;
                }
            }
            rand = _wRand.GetRandom(_unOccupiedNodes);
            if (!_grid[node.pos.x + rand.x,node.pos.y + rand.y].hasIngredient)
            {
                _pickedNode = node.pos + rand;
                break;
            }
            _unOccupiedNodes.Remove(rand);
        }
        if (_pickedNode.x < 0 || _pickedNode.y < 0 || _depth >= _maxIngredients /2 || _ingredientStack.Count < 1)
        {
            return true;
        }
        _depth++;
        PlaceIngredient(node ,_ingredientStack.Pop(), _pickedNode.x, _pickedNode.y);
        return UnfoldIngredients(_grid[_pickedNode.x, _pickedNode.y]);
    }
    
    private void ToggleNode(int x, int y)
    {
        _grid[x, y].hasIngredient = true;
    }
    
    [ContextMenu("ResetGame")]
    public void ResetGame()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Destroy(_grid[x, y].gameObject);
                _grid[x, y] = null;
            }
        }
        foreach (var item in itemsOnBoard)
        {
            Destroy(item);
        }
        itemsOnBoard.Clear();

        _ingredientStack.Clear();
        PopulateStack();
        RandomizeBoard();
        CreateGrid();
        //GeneratePattern();
        Camera.main.GetComponent<CameraPlacement>().PlaceCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
}