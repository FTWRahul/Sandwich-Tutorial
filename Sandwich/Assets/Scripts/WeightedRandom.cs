using System.Collections.Generic;
using UnityEngine;

public class WeightedRandom<T> where T : struct
{
    private readonly Dictionary<T, float> _weights;
    private readonly List<T> _values;
    float _strength;
    private readonly float _reRollThreshold = 70f;

    public void ResetValues(List<T> variables, float strength)
    {
        _weights.Clear();
        _values.Clear();
        foreach (var type in variables)
        {
            if (!_weights.ContainsKey(type))
            {
                _weights.Add(type, 100f);
                _values.Add(type);
            }
        }
        this._strength = strength;
    }

    public WeightedRandom(List<T> variables, float strength)
    {
        _weights = new Dictionary<T, float>();
        _values = new List<T>();
        foreach (var type in variables)
        {
            if (!_weights.ContainsKey(type))
            {
                _weights.Add(type, 100f);
                _values.Add(type);
            }
        }
        this._strength = strength;
    }

    public WeightedRandom()
    {
        _weights = new Dictionary<T, float>();
        _values = new List<T>();
        this._strength = 20f;
    }

    public T GetRandom()
    {
        int random = Random.Range(0, _values.Count);
        T rolledValue = _values[random];
        T returnType = rolledValue;
        if (_weights[rolledValue] < _reRollThreshold)
        {
            AdjustWeights(returnType, _strength/2);
            returnType = GetRandom();
        }
        AdjustWeights(returnType, -_strength);
        return returnType;
    }

    public T GetRandom(List<T> currentList)
    {
        int random = Random.Range(0, currentList.Count);
        T rolledValue = currentList[random];
        T returnType = rolledValue;
        //Debug.Log("Rolled " + rolledValue);
        if (_weights[rolledValue] < _reRollThreshold)
        {
            //Debug.Log("Rerolled " + rolledValue);
            AdjustWeights(returnType, _strength/2);
            //Debug.Log("Increasing Weight to be  "+ _weights[returnType]);
            return GetRandom(currentList);
        }
        //Debug.Log("Did not reroll " + rolledValue);

        AdjustWeights(returnType, -_strength);
        //Debug.Log("Decreasing Weight to be  "+ _weights[returnType]);
        //Debug.Log("Returning  "+ returnType);
        return returnType;
    }

    public void ResetWeights()
    {
        //Debug.Log("Weights Reset");
        for (int i = 0; i < _values.Count; i++)
        {
            _weights[_values[i]] = 100f;
        }
    }

    private void AdjustWeights(T type, float amount)
    {
        _weights[type] += amount;
    }
}