using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes T as a struct to create a class that returns weighted random values.
/// When a Type is returned it decreases its probability of being rolled again. 
/// </summary>
/// <typeparam name="T"></typeparam>
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
        if (_weights[rolledValue] < _reRollThreshold)
        {
            AdjustWeights(returnType, _strength/2);
            return GetRandom(currentList);
        }

        AdjustWeights(returnType, -_strength);
        return returnType;
    }

    public void ResetWeights()
    {
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