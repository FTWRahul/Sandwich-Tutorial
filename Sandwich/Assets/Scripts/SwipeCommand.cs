using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SwipeCommand : ICommand
{
    private IngredientSlice _slice;
    private IngredientSlice _nextSlice;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _flipDirection;
    private Node _node;
   
    private Sequence _flipSequence;

    private int _stackCount = 0;

    public SwipeCommand(IngredientSlice slice, IngredientSlice nextSlice, Vector3 initialPosition,Quaternion initialRotation, Vector3 flipDirection, Node node, int stackCount)
    {
        _slice = slice;
        _nextSlice = nextSlice;
        _initialPosition = initialPosition;
        _flipDirection = flipDirection;
        _node = node;
        _stackCount = stackCount;
        _initialRotation = initialRotation;
    }

    public void Execute()
    {
        FlipSlice(_flipDirection, _nextSlice, _stackCount);
    }

    private async void FlipSlice(Vector3 dir, IngredientSlice next, float yDisplacement)
    {
        _flipSequence = DOTween.Sequence();
        _flipSequence.Complete();
        _slice.Node = null;
        //Debug.Log("Model Height =" + IngredientSO.ModelHeight);
        Vector3 pos = next.transform.position;
        _flipSequence.Prepend(_slice.transform.DOJump(new Vector3(pos.x, yDisplacement * .25f, pos.z), (yDisplacement * .25f), 1, .5f)
            .SetEase(Ease.OutQuad));
        _flipSequence.Join(_slice.transform.DORotate(dir * 180, .5f, RotateMode.WorldAxisAdd));
        await Task.Delay(600);
        ToggleAfterDelay();
    }
   
    private async void FlipSlice(Vector3 dir, Vector3 next, float yDisplacement, float speed)
    {
        _flipSequence = DOTween.Sequence();
        _flipSequence.Complete();
        _slice.transform.parent = null;
        _slice.Node = _node;
        _slice.GetComponent<IngredientFlipper>().nextSlice = null;
        //ResetRotation();
        _slice.GetComponent<IngredientFlipper>().stackCount = 0;
        _nextSlice.GetComponent<IngredientFlipper>().stackCount = 0;
        //Debug.Log("Model Height =" + IngredientSO.ModelHeight);
        Vector3 pos = next;
        _flipSequence.Prepend(_slice.transform.DOJump(new Vector3(pos.x, yDisplacement * .25f, pos.z), (yDisplacement * .25f), 1, speed)
            .SetEase(Ease.OutQuad));
        _flipSequence.Join(_slice.transform.DOLocalRotate(dir * 180, speed, RotateMode.WorldAxisAdd));
        //_flipSequence.Append(_slice.transform.DORotate(Vector3.zero, .01f));
        await Task.Delay(100);
        ResetRotation();
    }

    private void ToggleAfterDelay()
    {
        //Task.Delay(600).ContinueWith(t => ToggleAfterDelay());
        _slice.transform.parent = _nextSlice.transform;
    }

    private void ResetRotation()
    {
        _slice.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }


    public void Undo(float speed)
    {
        FlipSlice(- _flipDirection, _initialPosition, 0, speed);
    }
   
}