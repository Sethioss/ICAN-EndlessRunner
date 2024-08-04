using UnityEngine;

[System.Serializable]
public class ActivitiesTransitionInfo
{
    [SerializeField] public BoundsArchetype _from;
    [SerializeField] public BoundsArchetype _to;
    [SerializeField] public SOActivity _activity;
}
