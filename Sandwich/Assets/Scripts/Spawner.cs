using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.Analytics;

public class Spawner : MonoBehaviour
{
    public Node[,] Grid => gridConstructor.grid;
    
    //Getter Properties
    public static int Width => GridConstructor.Width;
    public static int Height => GridConstructor.Height;
    public int MaxIngredients => gridConstructor.maxIngredients;
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
    
    //Static objects for ease of access
    public static List<IngredientSlice> itemsOnBoard = new List<IngredientSlice>();
    public static List<ICommand> commands = new List<ICommand>();
    public static bool canUndo;
    
    //Etc
    private List<IngredientSO> _tempingredientsList = new List<IngredientSO>();
    private bool _isRewinding = false;
    

    private void Awake()
    {
        //Initialization 
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

    /// <summary>
    /// Randomize the height and width of the game board.
    /// </summary>
    private void RandomizeBoard()
    {
        GridConstructor.Height = Random.Range(heightMinMax.x, heightMinMax.y);
        GridConstructor.Width = Random.Range(widthMinMax.x, widthMinMax.y);
        gridConstructor.maxIngredients = Random.Range(ingredientMinMax.x, ingredientMinMax.y);
    }

    /// <summary>
    /// Creates a stack of ingredients based on the list provided from the inspector
    /// </summary>
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

    /// <summary>
    /// Creates a mew node at position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateNode(int x, int y)
    {
        Node go = new GameObject("X : " + x + " Y : " + y).AddComponent<Node>();
        var transform1 = go.transform;
        transform1.parent = this.transform;
        transform1.position = new Vector3(x,0,y);
        go.pos.x = x;
        go.pos.y = y;
        gridConstructor.grid[x, y] = go;
    }

    /// <summary>
    /// Place Ingredient at specified node with the given data
    /// </summary>
    /// <param name="node"></param>
    /// <param name="data"></param>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
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

    /// <summary>
    /// Performs clean up and generates a new level
    /// </summary>
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

    //TODO: Move the function for retry and undo into another file
    
    /// <summary>
    /// Retry all moves
    /// </summary>
    public void Retry()
    {
        if (canUndo && !IngredientFlipper.hasWon)
        {
            StartCoroutine(UnfoldProper());
            Analytics.CustomEvent("Level Retry");
        }
    }

    /// <summary>
    /// Undo Last move
    /// </summary>
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
                commands[i -1].Undo(.2f);
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
    
    public void AddIngredientToList(IngredientSO ingredient)
    {
        ingredientsToSpawn.Add(ingredient);
    }
    
}