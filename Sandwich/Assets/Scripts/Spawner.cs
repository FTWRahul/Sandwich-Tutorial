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
        PlaceIngredient(breadSo,xOffset, zOffset);
        List<Vector2Int> possibleBread2Pos = _grid[xOffset, zOffset].GetNeighbours();
        int rand = Random.Range(0, possibleBread2Pos.Count);
        PlaceIngredient(breadSo,possibleBread2Pos[rand].x, possibleBread2Pos[rand].y);

        if (UnfoldIngredients(_grid[xOffset, zOffset]))
        {
            _depth = 0;
            if (UnfoldIngredients(_grid[possibleBread2Pos[rand].x, possibleBread2Pos[rand].y]))
            {
                _depth = 0;
                
                if(itemsOnBoard.Count < _maxIngredients/2)
                {
                    if(UnfoldIngredients(_grid[xOffset, zOffset]))
                    {
                        _depth = 0;
                        if(itemsOnBoard.Count < _maxIngredients/2)
                        {
                            if(UnfoldIngredients(_grid[possibleBread2Pos[rand].x, possibleBread2Pos[rand].y]))
                            {
                                _depth = 0;
                            }
                        }
                    }
                }
            }
        }
//        Debug.Log("Gonna start Loop now");
//        Debug.Break();
//        bool flip = false;
//        int i = 0;
//        while (_ingredientsSpawned < _maxIngredients)
//        {
//            Debug.Log("Loop started: Count:  " + i);
//            Debug.Break();
//
//            if (flip)
//            {
//                Debug.Log("Recursion Started:  " + i);
//                Debug.Break();
//
//
//                if(UnfoldIngredients(_grid[possibleBread2Pos[rand].x, possibleBread2Pos[rand].y]))
//                {
//                    _depth = 0;
//                }
//                Debug.Log("Recursion Ended:  " + i);
//                Debug.Break();
//
//
//            }
//            else
//            {
//                Debug.Log("Recursion Started:  " + i);
//                Debug.Break();
//
//                if(UnfoldIngredients(_grid[possibleBread2Pos[rand].x, possibleBread2Pos[rand].y]))
//                {
//                    _depth = 0;
//                }
//                Debug.Log("Recursion Ended:  " + i);
//                Debug.Break();
//
//
//            }
//            Debug.Log("Loop Ending" + i);
//            i++;
//            flip = !flip;
//        }
    }

    private void PlaceIngredient(IngredientSO data , int xPos, int yPos)
    {
        IngredientSlice slice = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<IngredientSlice>();
        slice.transform.position = new Vector3(xPos, 10, yPos);
        slice.ingredientData = data;
        slice.Init();
        ToggleNode(xPos, yPos);
        //_ingredientsSpawned++;
        itemsOnBoard.Add(slice.gameObject);
    }

    private bool UnfoldIngredients(Node node)
    {
        _unOccupiedNodes.Clear();
        _unOccupiedNodes = node.GetNeighbours();
        _pickedNode = _nullNode;
        _wRand.ResetValues(_unOccupiedNodes, _unOccupiedNodes.Count * 10);
        Vector2Int rand = new Vector2Int();
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
            rand = _wRand.GetRandom();
            if (!_grid[rand.x, rand.y].hasIngredient)
            {
                _pickedNode = rand;
                break;
            }
            _unOccupiedNodes.Remove(rand);
        }
        if (_pickedNode.x < 0 || _pickedNode.y < 0 || _depth >= _maxIngredients /2 || _ingredientStack.Count < 1)
        {
            return true;
        }
        _depth++;
        PlaceIngredient(_ingredientStack.Pop(), _pickedNode.x, _pickedNode.y);
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