using System.Collections.Generic;
using UnityEngine;

public class WeightedRandom<T> where T : struct
{
    private Dictionary<T, float> _weights;
    private List<T> _values;
    float _strength;
    private float _reRollThreshold = 70f;

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

    private void AdjustWeights(T type, float amount)
    {
        _weights[type] += amount;
    }
}