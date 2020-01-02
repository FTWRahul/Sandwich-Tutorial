using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool hasIngredient;
    public Vector2Int pos;
    
    public List<Vector2Int> GetNeighbours()
    {
        List<Vector2Int> returnList = new List<Vector2Int>();
        if (pos.y < Spawner.Height - 1)
        {
            returnList.Add(pos + Vector2Int.up);
        }
        if (pos.y != 0)
        {
            returnList.Add(pos + Vector2Int.down);
        }
        if (pos.x != 0)
        {
            returnList.Add(pos + Vector2Int.left);
        }
        if (pos.x < Spawner.Width - 1)
        {
            returnList.Add(pos + Vector2Int.right);
        }
        return returnList;
    }
}
