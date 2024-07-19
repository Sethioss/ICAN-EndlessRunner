#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineBoundsEditPlanePreviewer : MonoBehaviour
{
    [SerializeField] bool PreviewerEnabled = false;

    private List<SplinePointInfo> pointInfos;

    [SerializeField, Header("Optional")]
    public SplineBoundsEditPlane boundsPlane;

    [SerializeField][Min(2)] private int Precision;

    public SplineContainer PlayerSpline;

    [SerializeField]
    private SplineManager splineManager;

    private bool ValidData = true;

    Vector3 TempPoint;

    private void OnEnable()
    {
        if(!Application.IsPlaying(this))
        {
            if (boundsPlane)
            {
                pointInfos.Add(boundsPlane.pointInfo);
            }
            else
            {
                boundsPlane = GetComponent<SplineBoundsEditPlane>();
            }
        }
    }

public void OnDrawGizmos()
    {
        if (boundsPlane && boundsPlane.pointInfo.Positions.Count == 2)
        {
            pointInfos = new List<SplinePointInfo>() { boundsPlane.pointInfo };
            ValidData = true;
        }
        else
        {
            ValidData = false;
        }

        if (PreviewerEnabled && ValidData && boundsPlane.PreviewEditPlane)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < pointInfos.Count; ++i)
            {
                if (PlayerSpline.gameObject.activeSelf)
                {
                    if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE)
                    {
                        TempPoint = PlayerSpline.EvaluatePosition(pointInfos[i].Positions[0].Position) + PlayerSpline.EvaluateUpVector(pointInfos[i].Positions[0].Position) * 1.2f;
                        TempPoint += transform.position;

                        if (GetPassesByOrigin(pointInfos[i]))
                        {

                            for (int j = 1; j <= Precision; ++j)
                            {
                                float MaxPointPos = pointInfos[i].Positions[0].Position - 1.0f;
                                float PosOnSpline = Mathf.Lerp(MaxPointPos, pointInfos[i].Positions[pointInfos[i].Positions.Count - 1].Position, (float)j / Precision);
                                float RepeatedPoint = Mathf.Repeat(PosOnSpline, 1.0f);
                                Vector3 FinalPoint = PlayerSpline.EvaluatePosition(RepeatedPoint) + PlayerSpline.EvaluateUpVector(RepeatedPoint) * 1.2f;
                                FinalPoint += transform.position;
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
                                FinalPoint += transform.position;
                                Gizmos.DrawLine(FinalPoint, TempPoint);

                                TempPoint = FinalPoint;
                            }
                        }
                    }
                    if (pointInfos[i].PointPreviewType == SplinePointPreviewType.LINE_AND_SPHERES || pointInfos[i].PointPreviewType == SplinePointPreviewType.SPHERES)
                    {
                        foreach (SplinePoint point in pointInfos[i].Positions)
                        {

                            Random.InitState(((int)point.PositionPointType));
                            Gizmos.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                            TempPoint = PlayerSpline.EvaluatePosition(Mathf.Repeat(point.Position + splineManager.PlayerSpline.Origin, 1.0f));
                            TempPoint += transform.position;
                            Gizmos.DrawSphere(TempPoint, 0.4f);
                        }
                    }
                }
            }
        }
    }

    public bool GetPassesByOrigin(SplinePointInfo PointInfo)
    {
        return PointInfo.Positions[0].Position - PointInfo.Positions[PointInfo.Positions.Count - 1].Position >= 0.0f;
    }

    public List<float> GetPointsOfSpecificType(SplinePointPositionType positionInfoType)
    {
        List<float> points = new List<float>();
        foreach (SplinePointInfo positionInfo in pointInfos)
        {
            foreach (SplinePoint point in positionInfo.Positions)
            {
                if (point.PositionPointType == positionInfoType)
                {
                    points.Add(point.Position);
                }
            }
        }
        return points;
    }
}
#endif