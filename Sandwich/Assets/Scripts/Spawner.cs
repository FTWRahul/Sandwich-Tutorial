using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.Analytics;

public class Spawner : MonoBehaviour
{
    public Node[,] Grid => gridConstructor._grid;
    
    //Getter Properties
    public static int Width => GridConstructor._width;
    public static int Height => GridConstructor._height;
    
    public int MaxIngredients => gridConstructor._maxIngredients;

    public IngredientSO BreadSo => breadSo;

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
    private List<IngredientSO> ingredientsToSpawn = new List<IngredientSO>();
    
    public IPatternGenerator patternGeneration;
    public GridConstructor gridConstructor;
    public Stack<IngredientSO> _ingredientStack;
    
    public static List<IngredientSlice> itemsOnBoard = new List<IngredientSlice>();
    private List<IngredientSO> _tempingredientsList = new List<IngredientSO>();
    public static List<ICommand> commands = new List<ICommand>();
    public static bool canUndo;
    private bool _isRewinding = false;


    private void Awake()
    {

        patternGeneration = GetComponent<IPatternGenerator>();
        _ingredientStack = new Stack<IngredientSO>();
        gridConstructor = GetComponent<GridConstructor>();
        
        PopulateStack();

        RandomizeBoard();

        gridConstructor.InitilizeGrid(widthMinMax.y, heightMinMax.y);
        gridConstructor.CreateGrid();
    }

    private void Start()
    {
        patternGeneration.GeneratePattern();
    }

    private void RandomizeBoard()
    {
        GridConstructor._height = Random.Range(heightMinMax.x, heightMinMax.y);
        GridConstructor._width = Random.Range(widthMinMax.x, widthMinMax.y);
        gridConstructor._maxIngredients = Random.Range(ingredientMinMax.x, ingredientMinMax.y);
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

    public void CreateNode(int x, int y)
    {
        Node go = new GameObject("X : " + x + " Y : " + y).AddComponent<Node>();
        var transform1 = go.transform;
        transform1.parent = this.transform;
        transform1.position = new Vector3(x,0,y);
        go.pos.x = x;
        go.pos.y = y;
        gridConstructor._grid[x, y] = go;
    }

    public void PlaceIngredient(Node node ,IngredientSO data , int xPos, int yPos)
    {
        IngredientSlice slice = Instantiate(data.model).GetComponent<IngredientSlice>(); // GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<IngredientSlice>();
        //slice.gameObject.AddComponent<IngredientFlipper>();
        slice.transform.position = new Vector3(xPos, 10, yPos);
        slice.ingredientData = data;
        slice.Init(node);
        gridConstructor.ToggleNode(xPos, yPos);
        //_ingredientsSpawned++;
        itemsOnBoard.Add(slice);
    }

    [ContextMenu("ResetGame")]
    public void ResetGame()
    {
        gridConstructor.DeconstructGrid();
        IngredientFlipper.hasWon = false;
        foreach (var item in itemsOnBoard)
        {
            Destroy(item.gameObject);
        }
        itemsOnBoard.Clear();
        _ingredientStack.Clear();
        PopulateStack();
        RandomizeBoard();
        gridConstructor.CreateGrid();
        patternGeneration.GeneratePattern();
        Camera.main.GetComponent<CameraPlacement>().PlaceCamera();
        LevelEnd.biteCount = 1;
        Analytics.CustomEvent("Went To Next Level");

    }

    public void Retry()
    {
        if (canUndo && !IngredientFlipper.hasWon)
        {
            StartCoroutine(UnfoldProper());
            Analytics.CustomEvent("Level Retry");
        }
    }

    public void Undo()
    {
        if (commands.Count > 0 && canUndo)
        {
            int i = commands.Count - 1;
            Dictionary<string, object> data =  new Dictionary<string, object>();
            data.Add("Undid Move", commands[i].FlipperName());
            Analytics.CustomEvent("Move Undo", data);
            commands[i].Undo(.5f);
            commands.Remove(commands[i]);
        }
    }

    public IEnumerator UnfoldProper()
    {
        
        if (!_isRewinding && !IngredientFlipper.hasWon)
        {
            _isRewinding = true;
            for (int i = commands.Count; i > 0; i--)
            {
                //Debug.Log("I = " + (i - 1));
                commands[i -1 ].Undo(.2f);
                yield return new WaitForSeconds(.2f);
                commands.Remove(commands[(i - 1)]);
            }
            foreach (var item in itemsOnBoard)
            {
                item.GetComponent<IngredientFlipper>().stackCount = 0;
            }
            _isRewinding = false;
        }
        
    }
    
}