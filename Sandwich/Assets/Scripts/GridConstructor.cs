using UnityEngine;

public class GridConstructor : MonoBehaviour
{
    public static int _height;
    public static int _width;
    public int _maxIngredients;
    public Node[,] _grid;
    public Spawner spawner;

    private void Awake()
    {
        spawner = GetComponent<Spawner>();
    }

    public void InitilizeGrid(int x, int y)
    {
        _grid = new Node[x,y];
    }
    
    public void DeconstructGrid()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Destroy(this._grid[x, y].gameObject);
                this._grid[x, y] = null;
            }
        }
    }

    [ContextMenu("CreateGrid")]
    public void CreateGrid()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                spawner.CreateNode(x, y);
            }
        }
    }

    public void ToggleNode(int x, int y)
    {
        _grid[x, y].hasIngredient = true;
    }
}