using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.Analytics;

/// <summary>
/// Touch responder mono class for ingredient slices.
/// </summary>
public class IngredientFlipper : MonoBehaviour , IRespondToTouch
{
    private IngredientSlice _slice;
    [HideInInspector]
    public IngredientSlice nextSlice;
    private Sequence _nudgeSequence;
    private Sequence _endSequence;
    private Vector2Int _selectedNeighbour;
    [HideInInspector]
    public int stackCount = 0;

    public static bool hasWon;

    public static UnityEvent winEvent = new UnityEvent();
    
    
    private void Start()
    {
        //Reference assignment
        _nudgeSequence = DOTween.Sequence();
        _endSequence = DOTween.Sequence();
        
        _slice = GetComponent<IngredientSlice>();
        float randomDelay = Random.value;
        Invoke(nameof(DelayedDrop), randomDelay);
    }
    
    /// <summary>
    /// Initial delayed drop of ingredients on board.
    /// </summary>
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
    /// <summary>
    /// Interface implementation for calculating the direction of flip and performing additional logic on it
    /// </summary>
    /// <param name="dir"></param>
    public void AttemptFlip(Vector3 dir)
    {
        //When level is over don't perform any logic.
        if (hasWon)
        {
            return;
        }
        //If this object is already in another stack then call the flip from that object instead of this.
        if (transform.parent != null)
        {
            if (transform.GetComponentInParent<IRespondToTouch>() != null)
            {
                //Recursive function for getting the deepest touch responder of any given ingredient
                nextSlice.GetComponent<IRespondToTouch>().AttemptFlip(dir);
                return;
            }
        }
        //additional saftey check
        if (nextSlice != null)
        {
            return;
        }
        List<Vector2Int> neighbouringNodes = _slice.Node.GetNeighbours();
        
        Vector3 swipeDirection = GetSwipeDirection(dir);
        Vector2Int internalDir = new Vector2Int(Mathf.RoundToInt(swipeDirection.x), Mathf.RoundToInt(swipeDirection.z));

        bool exists = false;
        //Checking to see if what neighbours exist around this slice
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
            // Sets next slice to be the one from the confirmed neighbour list.
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
            //Creates a new command for swiping the ingredients onto another.
            //Command is added to the list of commands for Undo potential.
            var nextStackCount = nextSlice.GetComponent<IngredientFlipper>().stackCount;
            ICommand swipeCommand = new SwipeCommand(this, _slice, nextSlice, transform.position, transform.rotation, flipDirection, _slice.Node, stackCount, nextStackCount);
            swipeCommand.Execute();
            Spawner.commands.Add(swipeCommand);
            StartCoroutine(SpawnParticles(nextStackCount + 1));
            //After each successful flip checks for wining condition based on this logic
            if (_slice.ingredientData.isBread && nextSlice.ingredientData.isBread)
            {
                CheckWin();
            }
            return;
        }
        //If move is not possible calls the animation.
        StartCoroutine(NudgeSlice(flipDirection));
    }

    /// <summary>
    /// Spawns particles at the center of the stack.
    /// </summary>
    /// <param name="nextStackCount"></param>
    /// <returns></returns>
    IEnumerator SpawnParticles(int nextStackCount)
    {
        yield return new WaitForSeconds(.3f);
        Vector3 pos = new Vector3(nextSlice.transform.position.x,((nextStackCount + stackCount) * .25f) ,nextSlice.transform.position.z);
        Instantiate(_slice.particles, pos, Quaternion.Euler(90, 0, 0));
    }

    /// <summary>
    /// Checks to see if all the ingredients are incorporated between the two pieces of bread
    /// </summary>
    void CheckWin()
    {
        var count = nextSlice.GetComponent<IngredientFlipper>().stackCount;
        if (count == Spawner.itemsOnBoard.Count - 1)
        {
            Debug.Log("Win");
            winEvent.Invoke();
            hasWon = true;
            AudioManager.instance.PlayWinSound();
            StartCoroutine(RotateAll());
            Analytics.CustomEvent("Sandwich Eaten");

        }
        else
        {
            //TODO: Make the Undo Button unusable and play animation on retry button.
            //maybe even add a rewarded add for a retry.
            Debug.Log("Undo and keep Going");
        }
    }

    /// <summary>
    /// When the sandwich is made, all ingredients need to be flipped in a certain orientation to return them to their original rotation.
    /// This is a hard process as many ingredients are flipped over several times through out the run.
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateAll()
    {
        _endSequence.Complete();
        yield return new WaitForSeconds(.5f);
        //Set all transform parents to null
        for (int i = 0; i < Spawner.itemsOnBoard.Count; i++)
        {
            Spawner.itemsOnBoard[i].transform.parent = null;
        }
        foreach (var ingredient in Spawner.itemsOnBoard)
        {
            //conditions for how to rotate each piece based on their current rotation
            //Also adjusts the height of the unparented object
            if (Mathf.Approximately(Mathf.Abs(ingredient.transform.rotation.eulerAngles.y), 180) &&
                Mathf.Approximately(Mathf.Abs(ingredient.transform.rotation.eulerAngles.z), 180))
            {
                ingredient.transform.rotation = Quaternion.Euler(Vector3.zero);
                ingredient.transform.position += new Vector3(0, -.25f, 0);
            }
            else if (Mathf.Approximately(Mathf.Abs(ingredient.transform.rotation.eulerAngles.y), 180))
            {
                ingredient.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            else if (Mathf.Approximately(Mathf.Abs(ingredient.transform.rotation.eulerAngles.z), 180))
            {
                ingredient.transform.rotation = Quaternion.Euler(Vector3.zero);
                ingredient.transform.position += new Vector3(0, -.25f, 0);
            }
            _endSequence.Prepend(ingredient.transform.DOJump(ingredient.transform.position, ingredient.transform.position.y, 1, 1f).SetEase(Ease.InOutQuad));
        }
    }
    
    /// <summary>
    /// Animation for when move is impossible
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IEnumerator NudgeSlice(Vector3 dir)
    {
        _nudgeSequence.Complete();
        _nudgeSequence.Prepend(transform.DOLocalRotate(dir * 10f, .2f, RotateMode.WorldAxisAdd));
        yield return new WaitForSeconds(.2f);
        _nudgeSequence.Append(transform.DOLocalRotate(Vector3.zero, .5f)
            .SetEase(Ease.OutElastic));
    }
    
    /// <summary>
    /// Returns Vector of the direction based on ingame conversions.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 FlipDirection(Vector3 dir)
    {
        if (dir == Vector3.right)
        {
            return Vector3.back;
        }
        if (dir == Vector3.left)
        {
            return Vector3.forward;
        }
        if (dir == Vector3.forward)
        {
            return Vector3.right;
        }
        return Vector3.left;
    }
    
    /// <summary>
    /// Compares vectors to determine which cardinal direction the swipe was performed in
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
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
            return Vector3.right;
        }
        else if (final == left)
        {
            return Vector3.left;
        }
        else if (final == forward)
        {
            return Vector3.forward;
        }
        else
        {
            return Vector3.back;
        }
    }

    /// <summary>
    /// Returns the smaller of the two flotes
    /// TODO: Move to extension method class for floats, maybe all numeric structs
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    float GetSmaller(float a, float b)
    {
        if (a < b)
        {
            return a;
        }
        return b;
    }
}

