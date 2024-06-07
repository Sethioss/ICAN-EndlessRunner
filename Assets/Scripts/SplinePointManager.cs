using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public enum SplinePointPreviewType
{
    LINE = 0,
    SPHERES = 1,
    LINE_AND_SPHERES = 2,
}
public enum SplinePointPositionType
{
    DEFAULT = 0,
    JUMP_POINTS = 1,
    BOUNDS = 2,
    PREVIEW = 3,
}

[System.Serializable]
public struct SplinePointInfo
{
    [SerializeField]
    [Range(0, 1)]
    public List<float> Positions;
    [SerializeField]
    public SplinePointPositionType PositionPointType;
    [SerializeField]
    public SplinePointPreviewType PointPreviewType;
}

public class SplinePointManager : MonoBehaviour
{
    [SerializeField]
    public List<SplinePointInfo> pointInfos;

    [SerializeField] [Min(2)]private int Precision;

    [SerializeField]
    SplineContainer PlayerSpline;

    Vector3 TempPoint;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < pointInfos.Count; ++i)
        {
            Random.InitState(((int)pointInfos[i].PositionPointType));
            Gizmos.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            if (PlayerSpline.gameObject.activeSelf)
            {
                if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE)
                {
                    if (GetIsInvertedBounds(pointInfos[i]))
                    {
                        TempPoint = PlayerSpline.EvaluatePosition(pointInfos[i].Positions[0]) + PlayerSpline.EvaluateUpVector(pointInfos[i].Positions[0]) * 1.2f;

                        for (int j = 1; j <= Precision; ++j)
                        {
                            float MaxPointPos = pointInfos[i].Positions[0] - 1.0f;
                            float PosOnSpline = Mathf.Lerp(MaxPointPos, pointInfos[i].Positions[pointInfos[i].Positions.Count - 1], (float)j / Precision);
                            float RepeatedPoint = Mathf.Repeat(PosOnSpline, 1.0f);
                            Vector3 FinalPoint = PlayerSpline.EvaluatePosition(RepeatedPoint) + PlayerSpline.EvaluateUpVector(RepeatedPoint) * 1.2f;
                            Gizmos.DrawLine(TempPoint, FinalPoint);

                            TempPoint = FinalPoint;
                        }
                    }
                    else
                    {
                        TempPoint = PlayerSpline.EvaluatePosition(pointInfos[i].Positions[0]) + PlayerSpline.EvaluateUpVector(pointInfos[i].Positions[0]) * 1.2f;

                        for (int j = 0; j <= Precision; ++j)
                        {
                            float temp = Mathf.Lerp(pointInfos[i].Positions[0], pointInfos[i].Positions[pointInfos[i].Positions.Count - 1], (float)j / Precision);
                            Vector3 FinalPoint = PlayerSpline.EvaluatePosition(temp) + PlayerSpline.EvaluateUpVector(temp) * 1.2f;
                            Gizmos.DrawLine(FinalPoint, TempPoint);

                            TempPoint = FinalPoint;
                        }
                    }
                }
                if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.SPHERES)
                {
                    foreach (float position in pointInfos[i].Positions)
                    {
                        Gizmos.DrawSphere(PlayerSpline.EvaluatePosition(Mathf.Repeat(position + GetComponent<SplineManager>().PlayerSpline.Origin, 1.0f)), 0.4f);
                    }
                }
            }
        }


    }
#endif

    public bool GetIsInvertedBounds(SplinePointInfo PointInfo)
    {
        return PointInfo.Positions[0] - PointInfo.Positions[PointInfo.Positions.Count - 1] >= 0.0f;
    }

    public List<float> GetPointsOfSpecificType(SplinePointPositionType positionInfoType)
    {
        List<float> points = new List<float>();
        foreach (SplinePointInfo positionInfo in pointInfos)
        {
            if (positionInfo.PositionPointType == positionInfoType)
            {
                foreach (float position in positionInfo.Positions)
                {
                    points.Add(position);
                }
            }
        }
        return points;
    }
}

