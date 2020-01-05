using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PatternGeneration : MonoBehaviour , IPatternGenerator
{
    //Recursive variables
    private List<Vector2Int> _unOccupiedNodes = new List<Vector2Int>();
    private Vector2Int _pickedNode = new Vector2Int(-1,-1);
    private readonly Vector2Int _nullNode = new Vector2Int(-1,-1);
    private WeightedRandom<Vector2Int> _wRand;
    private int _depth = 0;

    public Spawner spawner;
    
    private void Awake()
    {
        _wRand = new WeightedRandom<Vector2Int>();
        spawner = GetComponent<Spawner>();
    }

    private void Start()
    {
        List<Vector2Int> centerPoint = spawner.Grid[Spawner.Width / 2, Spawner.Height / 2].GetNeighbours();
        _wRand.ResetValues(centerPoint, centerPoint.Count * 10);
    }
    
    [ContextMenu("GeneratePattern")]
    public void GeneratePattern()
    {
        int xOffset = Spawner.Width/2 + Random.Range(-1, 1);
        int zOffset = Spawner.Height/2 +Random.Range(-1, 1);
        spawner.PlaceIngredient(spawner.Grid[xOffset, zOffset], spawner.BreadSo,xOffset, zOffset);
        Vector2Int bread1Pos = spawner.Grid[xOffset, zOffset].pos;
        List<Vector2Int> possibleBread2Pos = spawner.Grid[xOffset, zOffset].GetNeighbours();

        int rand = Random.Range(0, possibleBread2Pos.Count);
        spawner.PlaceIngredient(spawner.Grid[xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y] , spawner.BreadSo,xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y);
        Vector2Int bread2Pos = spawner.Grid[xOffset + possibleBread2Pos[rand].x, zOffset + possibleBread2Pos[rand].y].pos;

        bool flip = false;
        int i = 0;
        for (int j = 0; j < 10; j++)
        {
            if (Spawner.itemsOnBoard.Count > spawner.MaxIngredients - 2)
            {
                break;
            }
            if (flip)
            {
                if(this.UnfoldIngredients(spawner.Grid[bread2Pos.x, bread2Pos.y]))
                {
                    _depth = 0;
                    _wRand.ResetWeights();
                }
            }
            else
            {
                if(this.UnfoldIngredients(spawner.Grid[bread1Pos.x, bread1Pos.y]))
                {
                    _depth = 0;
                    _wRand.ResetWeights();
                }
            }
            flip = !flip;
        }
        Camera.main.GetComponent<CameraPlacement>().PlaceCamera();
    }
    
    public bool UnfoldIngredients(Node node)
    {
        this._unOccupiedNodes.Clear();
        this._unOccupiedNodes = node.GetNeighbours();
        this._pickedNode = _nullNode;
        Vector2Int rand;
        for (int i = 0; i < this._unOccupiedNodes.Count; i++)
        {    
            if (this._unOccupiedNodes.Count == 1)
            {
                float coinToss = Random.value;
                if (coinToss < .7f)
                {
                    return true;
                }
            }
            rand = _wRand.GetRandom(_unOccupiedNodes);
            if (!spawner.Grid[node.pos.x + rand.x,node.pos.y + rand.y].hasIngredient)
            {
                this._pickedNode = node.pos + rand;
                break;
            }
            this._unOccupiedNodes.Remove(rand);
        }
        if (this._pickedNode.x < 0 || this._pickedNode.y < 0 || _depth >= spawner.MaxIngredients /2 || spawner._ingredientStack.Count < 1)
        {
            return true;
        }
        _depth++;
        spawner.PlaceIngredient(spawner.Grid[_pickedNode.x, _pickedNode.y] , spawner._ingredientStack.Pop(), _pickedNode.x, _pickedNode.y);
        return spawner.patternGeneration.UnfoldIngredients(spawner.Grid[_pickedNode.x, _pickedNode.y]);
    }
}