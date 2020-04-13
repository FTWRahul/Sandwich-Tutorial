using UnityEngine;
using UnityEngine.Serialization;

public class GridConstructor : MonoBehaviour
{
    public static int Height;
    public static int Width;
    public int maxIngredients;
    public Node[,] grid;
    public Spawner spawner;

    private void Awake()
    {
        spawner = GetComponent<Spawner>();
    }

    /// <summary>
    /// Sets a new node at the x and y position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitilizeGrid(int x, int y)
    {
        grid = new Node[x,y];
    }
    
    /// <summary>
    /// Destroys the objects in the grid
    /// </summary>
    public void DeconstructGrid()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Destroy(this.grid[x, y].gameObject);
                this.grid[x, y] = null;
            }
        }
    }

    /// <summary>
    /// Creates nodes at given x and y
    /// </summary>
    public void CreateGrid()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                spawner.CreateNode(x, y);
            }
        }
    }

    public void ToggleNode(int x, int y)
    {
        grid[x, y].hasIngredient = true;
    }
}