using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ActivityGenerationPool
{
    [SerializeField] public int _minDistance;
    [SerializeField] public int _poolLength;
    [SerializeField] public List<ActivityData> _activities = new List<ActivityData>();
}
