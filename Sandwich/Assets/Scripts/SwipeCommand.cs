﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SwipeCommand : ICommand
{
    private IngredientFlipper _ingredientFlipper;
    private IngredientSlice _slice;
    private IngredientSlice _nextSlice;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _flipDirection;
    private Node _node;
   
    private Sequence _flipSequence;

    private int _stackCount = 0;
    private int _nextStackCount = 0;

    //Public constructor for creating a command.
    public SwipeCommand(IngredientFlipper ingredientFlipper , IngredientSlice slice, IngredientSlice nextSlice, Vector3 initialPosition,Quaternion initialRotation, Vector3 flipDirection, Node node, int stackCount , int nextStackCount)
    {
        _slice = slice;
        _nextSlice = nextSlice;
        _initialPosition = initialPosition;
        _flipDirection = flipDirection;
        _node = node;
        _stackCount = stackCount;
        _nextStackCount = nextStackCount;
        _initialRotation = initialRotation;
        _ingredientFlipper = ingredientFlipper;
    }

    //Interface implementation
    public void Execute()
    {
        int displacement = _stackCount + (_nextStackCount + 1);
        FlipSlice(_flipDirection, _nextSlice, displacement);
        _nextSlice.GetComponent<IngredientFlipper>().stackCount = displacement;
        PlayAudio();
    }

    //Flip animation
    private async void FlipSlice(Vector3 dir, IngredientSlice next, float yDisplacement)
    {
        Spawner.canUndo = false;
        _flipSequence = DOTween.Sequence();
        _flipSequence.Complete();
        _slice.Node = null;
        Vector3 pos = next.transform.position;
        _flipSequence.Prepend(_slice.transform.DOJump(new Vector3(pos.x, (yDisplacement * .25f) + .25f, pos.z), (yDisplacement * .25f), 1, .5f)
            .SetEase(Ease.OutQuad));
        _flipSequence.Join(_slice.transform.DORotate(dir * 180, .5f, RotateMode.WorldAxisAdd));
        //GameObject.Instantiate(_slice.particles, new Vector3(next.transform.localPosition.x,(_nextStackCount + 1) * .25f,next.transform.localPosition.y), Quaternion.Euler(90, 0 ,0));
        await Task.Delay(600);
        ToggleAfterDelay();
    }
   
    //Undo animation
    private async void FlipSlice(Vector3 dir, Vector3 next, float yDisplacement, float speed)
    {
        Spawner.canUndo = false;
        _flipSequence = DOTween.Sequence();
        _flipSequence.Complete();
        _slice.transform.parent = null;
        _slice.Node = _node;
        _ingredientFlipper.nextSlice = null;
        Vector3 pos = next;
        _flipSequence.Prepend(_slice.transform.DOJump(new Vector3(pos.x, 0 , pos.z), (yDisplacement + .25f), 1, speed)
            .SetEase(Ease.OutQuad));
        _flipSequence.Join(_slice.transform.DOLocalRotate(dir * 180, speed, RotateMode.WorldAxisAdd));
        _ingredientFlipper.stackCount = _stackCount;
        _nextSlice.GetComponent<IngredientFlipper>().stackCount = _nextStackCount;
        await Task.Delay(Mathf.RoundToInt((speed + 1) * 1000 ));
        ResetRotation();
    }

    //Async delayed function callback
    private void ToggleAfterDelay()
    {
        _slice.transform.parent = _nextSlice.transform;
        Spawner.canUndo = true;
    }

    //Async delayed function callback
    private void ResetRotation()
    {
        _slice.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Spawner.canUndo = true;
    }
    //Undo interface implementation 
    public void Undo(float speed)
    {
        FlipSlice(- _flipDirection, _initialPosition, _stackCount, speed);
        PlayAudio();
    }

    void PlayAudio()
    {
       AudioManager.instance.PlaySwipeSound();
    }

    public string FlipperName()
    {
        return _slice.name;
    }
}