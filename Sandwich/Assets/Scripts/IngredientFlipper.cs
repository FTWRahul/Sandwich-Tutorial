using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;
    [HideInInspector]
    public IngredientSlice nextSlice;
    private Sequence _nudgeSequence;
    private Vector2Int _selectedNeighbour;
    [HideInInspector]
    public int stackCount = 0;

    public static bool hasWon;
    //private static int _internalCounter = 0;
    public static List<IngredientFlipper> ingredientsOnStack = new List<IngredientFlipper>();
    
    private void Start()
    {
        _nudgeSequence = DOTween.Sequence();
        //_flipSequence = DOTween.Sequence();
        
        _slice = GetComponent<IngredientSlice>();
        float randomDelay = Random.value;
        Invoke(nameof(DelayedDrop), randomDelay);
    }
    
    private void DelayedDrop()
    {
        try
        {
            transform.DOMoveY(0, .5f).SetEase(_slice.ingredientData.ease);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void AttemptFlip(Vector3 dir)
    {
        if (hasWon)
        {
            FindObjectOfType<LevelEnd>().TakeBite();
            return;
        }
        if (transform.parent != null)
        {
            if (transform.GetComponentInParent<IRespondToTouch>() != null)
            {
                nextSlice.GetComponent<IRespondToTouch>().AttemptFlip(dir);
                return;
            }
        }
        if (nextSlice != null)
        {
            return;
        }
        List<Vector2Int> neighbouringNodes = _slice.Node.GetNeighbours();
        
        Vector3 swipeDirection = GetSwipeDirection(dir);
        Vector2Int internalDir = new Vector2Int(Mathf.RoundToInt(swipeDirection.x), Mathf.RoundToInt(swipeDirection.z));

        bool exists = false;
        for (int i = 0; i < neighbouringNodes.Count; i++)
        {
            if (internalDir == neighbouringNodes[i])
            {
                exists = true;
                _selectedNeighbour = neighbouringNodes[i] + _slice.Node.pos;
                break;
            }
        }
        
        Vector3 flipDirection = FlipDirection(swipeDirection);

        if (exists)
        {
            for (int i = 0; i < Spawner.itemsOnBoard.Count; i++)
            {
                if (Spawner.itemsOnBoard[i].Node != null)
                {
                    if (Spawner.itemsOnBoard[i].Node.pos == _selectedNeighbour)
                    {
                        nextSlice = Spawner.itemsOnBoard[i];
                        break;
                    }
                }
            }
        }
        if (nextSlice != null)
        {
            //Debug.Log("Current Stack count for object = " + gameObject.name + "  " + stackCount);
            var nextStackCount = nextSlice.GetComponent<IngredientFlipper>().stackCount;
            //Debug.Log("Next Stack count for object = " + nextSlice.gameObject.name + "  " + nextStackCount);
            //int displacement = stackCount + (nextStackCount + 1);
            ICommand swipeCommand = new SwipeCommand(this, _slice, nextSlice, transform.position, transform.rotation, flipDirection, _slice.Node, stackCount, nextStackCount);
            swipeCommand.Execute();
            Spawner.commands.Add(swipeCommand);
            //Debug.Log("Spawwner confirmation " + Spawner.commands.Count);
            if (_slice.ingredientData.isBread && nextSlice.ingredientData.isBread)
            {
                CheckWin();
            }
            return;
        }
        StartCoroutine(NudgeSlice(flipDirection));
    }

    void CheckWin()
    {
        var count = nextSlice.GetComponent<IngredientFlipper>().stackCount;
        if (count == Spawner.itemsOnBoard.Count - 1)
        {
            Debug.Log("Win");
            Debug.Log("Stack count is " +count);
            hasWon = true;
            StartCoroutine(RotateAll());
        }
        else
        {
            Debug.Log("Undo and keep Going");
            Debug.Log("Stack count is " +count);
        }
    }

    IEnumerator RotateAll()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < ingredientsOnStack.Count; i++)
        {
            //Debug.Log(ingredientsOnStack[i].name + " : Pos = " + ingredientsOnStack[i].transform.position);
            ingredientsOnStack[i].transform.parent = null;
            yield return new WaitForSeconds(.5f);
        }
        yield return new WaitForSeconds(2);
        foreach (var ingredient in Spawner.itemsOnBoard)
        {
            Debug.Log(ingredient.transform.localRotation.eulerAngles+ " Rotation for " + ingredient.name);
            if (Mathf.Approximately(Mathf.Abs(ingredient.transform.localRotation.eulerAngles.y), 180) &&
                Mathf.Approximately(Mathf.Abs(ingredient.transform.localRotation.eulerAngles.z), 180))
            {
                Debug.Log("DO notin");
            }
            else if (Mathf.Approximately(Mathf.Abs(ingredient.transform.localRotation.eulerAngles.y), 180))
            {
                Debug.Log("Setting Y to 0 for  : " + ingredient.name);
                ingredient.transform.rotation = Quaternion.Euler(new Vector3(ingredient.transform.localRotation.eulerAngles.x, 0, ingredient.transform.localRotation.eulerAngles.z));
            }
            else if (Mathf.Approximately(Mathf.Abs(ingredient.transform.localRotation.eulerAngles.z), 180))
            {
                Debug.Log("Setting Y to 180 for  : " + ingredient.name);
                ingredient.transform.rotation = Quaternion.Euler(new Vector3(ingredient.transform.localRotation.eulerAngles.x, 180, ingredient.transform.localRotation.eulerAngles.z));
            }
        }
//        List<IngredientSlice> orderedStack = new List<IngredientSlice>();
//        var orderedEnumerable = Spawner.itemsOnBoard.OrderBy(t => t.transform.localPosition.y);
//        orderedStack = orderedEnumerable.ToList();
//        foreach (var ingredient in orderedStack)
//        {
//            Debug.Log(ingredient.name + "   " + ingredient.transform.localPosition.y);
//            ingredient.transform.localRotation = Quaternion.Euler(Vector3.zero);
//        }
//        
//        for (int i = 0; i < ingredientsOnStack.Count; i++)
//        {
//            ingredientsOnStack[i].transform.rotation = Quaternion.Euler(Vector3.zero);
//            yield return new WaitForSeconds(.5f);
//            Debug.Log(ingredientsOnStack[i].name + " : After Rotation : Pos = " + ingredientsOnStack[i].transform.position);
//        }
    }
    
    
    private IEnumerator NudgeSlice(Vector3 dir)
    {
        _nudgeSequence.Complete();
        _nudgeSequence.Prepend(transform.DOLocalRotate(dir * 10f, .2f, RotateMode.WorldAxisAdd));
        yield return new WaitForSeconds(.2f);
        _nudgeSequence.Append(transform.DOLocalRotate(Vector3.zero, .5f)
            .SetEase(Ease.OutElastic));
    }
    
    private Vector3 FlipDirection(Vector3 dir)
    {
        if (dir == Vector3.right)
        {
            return Vector3.back;
        }
        else if (dir == Vector3.left)
        {
            return Vector3.forward;
        }
        else if (dir == Vector3.forward)
        {
            return Vector3.right;
        }
        else
        {
            return Vector3.left;
        }
    }
    private Vector3 GetSwipeDirection(Vector3 dir)
    {
        float right = Vector3.Angle(dir, Vector3.right);
        float left = Vector3.Angle(dir, Vector3.left);
        float forward = Vector3.Angle(dir, Vector3.forward);
        float back = Vector3.Angle(dir, Vector3.back);

        float a = GetSmaller(right, left);
        float b = GetSmaller(forward, back);
        float final = GetSmaller(a, b);

        if (final == right)
        {
            Debug.Log("Flip Right");
            return Vector3.right;
        }
        else if (final == left)
        {
            Debug.Log("Flip Left");
            return Vector3.left;
        }
        else if (final == forward)
        {
            Debug.Log("Flip Up");
            return Vector3.forward;
        }
        else
        {
            Debug.Log("Flip Down");
            return Vector3.back;
        }
    }

    float GetSmaller(float a, float b)
    {
        if (a < b)
        {
            return a;
        }
        else
        {
            return b;
        }
    }
}

