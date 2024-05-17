using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivityGenerationPool
{
    [SerializeField][MinValue(0)] public int _minDistance;
    [SerializeField] public List<ActivityData> _activities = new List<ActivityData>();
}
