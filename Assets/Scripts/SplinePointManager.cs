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
    JUMP_POINT = 0,
    BOUNDS = 1,
    PREVIEW = 2,
}

[System.Serializable]
public struct SplinePointInfo
{
    public static bool operator == (SplinePointInfo left, SplinePointInfo right)
    {
        if(left.Positions.Count != right.Positions.Count)
        {
            return false;
        }

        for(int i = 0; i < left.Positions.Count; ++i)
        {
            if ((left.Positions[i].PositionPointType != right.Positions[i].PositionPointType) || (left.Positions[i].Position != right.Positions[i].Position))
            {
                return false;
            }
        }
        return true;
    }

    public static bool operator != (SplinePointInfo left, SplinePointInfo right)
    {
        if (left.Positions.Count == right.Positions.Count)
        {
            for (int i = 0; i < left.Positions.Count; ++i)
            {
                if ((left.Positions[i].PositionPointType == right.Positions[i].PositionPointType) && (left.Positions[i].Position == right.Positions[i].Position))
                {
                    return false;
                }
            }
        }
        return true;
    }

    [SerializeField]
    //TODO: Find a way to store these in an array rather than a list, a solution might be to turn this into a class
    [Header("Only put two positions, it might lead to undefined behaviour, and \nonly the first and last point will be taken in consideration anyway")]
    public List<SplinePoint> Positions;
    [SerializeField]
    public SplinePointPreviewType PointPreviewType;
}

[System.Serializable]
public struct SplinePoint
{
    [SerializeField]
    [Range(0.01f, 0.99f)]
    public float Position;
    [SerializeField]
    public SplinePointPositionType PositionPointType;
}

public class SplinePointManager : MonoBehaviour
{
    [SerializeField]
    private bool ManagerEnabled = true;

    public bool PreviewingPrefab = false;

    [SerializeField]
    public List<SplinePointInfo> pointInfos;

    [SerializeField] [Min(2)]private int Precision;

    [SerializeField]
    public SplineContainer PlayerSpline;

    Vector3 TempPoint;

    private void Start()
    {
        //DEBUG - REMOVE LATER
        //gameObject.GetComponent<SplineManager>().ChangeBounds(pointInfos);
        //gameObject.GetComponent<SplineManager>().ChangeCurrentBounds(pointInfos[0]);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if(ManagerEnabled)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < pointInfos.Count; ++i)
            {
                if (PlayerSpline.gameObject.activeSelf)
                {
                    if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE)
                    {
                        TempPoint = PlayerSpline.EvaluatePosition(pointInfos[i].Positions[0].Position) + PlayerSpline.EvaluateUpVector(pointInfos[i].Positions[0].Position) * 1.2f;
                        
                        if (GetPassesByOrigin(pointInfos[i]))
                        {

                            for (int j = 1; j <= Precision; ++j)
                            {
                                float MaxPointPos = pointInfos[i].Positions[0].Position - 1.0f;
                                float PosOnSpline = Mathf.Lerp(MaxPointPos, pointInfos[i].Positions[pointInfos[i].Positions.Count - 1].Position, (float)j / Precision);
                                float RepeatedPoint = Mathf.Repeat(PosOnSpline, 1.0f);
                                Vector3 FinalPoint = PlayerSpline.EvaluatePosition(RepeatedPoint) + PlayerSpline.EvaluateUpVector(RepeatedPoint) * 1.2f;
                                Gizmos.DrawLine(TempPoint, FinalPoint);

                                TempPoint = FinalPoint;
                            }
                        }
                        else
                        {
                            for (int j = 0; j <= Precision; ++j)
                            {
                                float temp = Mathf.Lerp(pointInfos[i].Positions[0].Position, pointInfos[i].Positions[pointInfos[i].Positions.Count - 1].Position, (float)j / Precision);
                                Vector3 FinalPoint = PlayerSpline.EvaluatePosition(temp) + PlayerSpline.EvaluateUpVector(temp) * 1.2f;
                                Gizmos.DrawLine(FinalPoint, TempPoint);

                                TempPoint = FinalPoint;
                            }
                        }
                    }
                    if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.SPHERES)
                    {
                        foreach (SplinePoint point in pointInfos[i].Positions)
                        {
                            switch((int)point.PositionPointType)
                            {
                                //Jump point
                                case 0:
                                {
                                    Gizmos.color = Color.green;
                                    break;
                                }
                                //Bounds
                                case 1:
                                {
                                    Gizmos.color = Color.red;
                                    break;
                                }
                                //Other
                                default:
                                {
                                    Gizmos.color = Color.black;
                                    break;
                                }
                            }
                            Gizmos.DrawSphere(PlayerSpline.EvaluatePosition(Mathf.Repeat(point.Position + GetComponent<SplineManager>().PlayerSpline.Origin, 1.0f)), 0.4f);
                        }
                    }
                }
            }
        }
    }
#endif

    public bool GetPassesByOrigin(SplinePointInfo PointInfo)
    {
        return PointInfo.Positions[0].Position - PointInfo.Positions[PointInfo.Positions.Count - 1].Position >= 0.0f;
    }

    public List<float> GetPointsOfSpecificType(SplinePointPositionType positionInfoType)
    {
        List<float> points = new List<float>();
        foreach (SplinePointInfo positionInfo in pointInfos)
        {
            foreach(SplinePoint point in positionInfo.Positions)
            {
                if (point.PositionPointType == positionInfoType)
                {
                    points.Add(point.Position);
                }
            }        }
        return points;
    }
}

