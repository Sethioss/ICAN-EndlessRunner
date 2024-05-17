using UnityEngine;

[System.Serializable]
public class SplineBounds
{
    [SerializeField, Range(0, 1)] float _minValue;
    [SerializeField, Range(0, 1)] float _maxValue;
}
