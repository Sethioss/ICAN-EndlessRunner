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
    [SerializeField] public List<SplinePointInfo> AppliedBounds;
    [SerializeField] public SplinePointInfo CurrentBounds;
    [SerializeField] public SplineBoundsEditPlane CurrentBoundsEditPlane;

    public void ChangeBounds(List<SplinePointInfo> newBounds)
    {
        AppliedBounds = newBounds;
    }

    public void ChangeCurrentBounds(SplinePointInfo newBounds)
    {
        CurrentBounds = newBounds;
    }

    public void ChangeCurrentBounds(SplinePointInfo newBounds, SplineBoundsEditPlane BoundsEditPlane)
    {
        CurrentBounds = newBounds;
        CurrentBoundsEditPlane = BoundsEditPlane;
    }

    public float[] GetCurrentBoundsPositions()
    {
        float[] resultArray = new float[2];
        List<float> result = new List<float>();
        foreach(SplinePoint p in CurrentBounds.Positions)
        {
            result.Add(p.Position);
        }

        resultArray[0] = result[0];
        resultArray[1] = result[result.Count - 1];

        return resultArray;
    }

    public bool CurrentBoundPassesByOrigin(SplinePointInfo CurrentBounds)
    {
        return CurrentBounds.Positions[0].Position - CurrentBounds.Positions[1].Position >= 0.0f;
    }
}
