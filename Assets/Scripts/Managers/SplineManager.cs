using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public struct SplineInfo
{
    [SerializeField] public SplineContainer PlayerSpline;
    [Range(0f, 1f)]
    [SerializeField] public float Origin;
}
public class SplineManager : MonoBehaviour
{
    [SerializeField] public SplineInfo PlayerSpline;
    [SerializeField] public List<SplineBounds> AppliedBounds;
    [SerializeField] public SplineBounds CurrentBounds;
}
