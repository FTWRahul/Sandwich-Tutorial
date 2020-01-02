using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool hasIngredient;
    public Vector2Int pos;
    private List<Vector2Int> _returnList = new List<Vector2Int>();
    
    public List<Vector2Int> GetNeighbours()
    {
        _returnList.Clear();   
//        if (pos.y < Spawner.Height - 1)
//        {
//            _returnList.Add(pos + Vector2Int.up);
//        }
//        if (pos.y != 0)
//        {
//            _returnList.Add(pos + Vector2Int.down);
//        }
//        if (pos.x != 0)
//        {
//            _returnList.Add(pos + Vector2Int.left);
//        }
//        if (pos.x < Spawner.Width - 1)
//        {
//            _returnList.Add(pos + Vector2Int.right);
        //}
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
