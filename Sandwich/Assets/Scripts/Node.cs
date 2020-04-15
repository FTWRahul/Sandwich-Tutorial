using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node is a grid point that holds its position
/// </summary>
public class Node : MonoBehaviour
{
    public bool hasIngredient;
    public Vector2Int pos;
    private List<Vector2Int> _returnList = new List<Vector2Int>();
    
    /// <summary>
    /// Returns the neighbours that exist
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetNeighbours()
    {
        _returnList.Clear();
        
        if (pos.y < Spawner.Height - 1)
        {
            _returnList.Add(Vector2Int.up);
        }
        if (pos.y != 0)
        {
            _returnList.Add(Vector2Int.down);
        }
        if (pos.x != 0)
        {
            _returnList.Add(Vector2Int.left);
        }
        if (pos.x < Spawner.Width - 1)
        {
            _returnList.Add(Vector2Int.right);
        }
        return _returnList;
    }
}
