using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;

[SerializeField]
public enum SplinePointPositionType : uint
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
    public SplinePointPositionType PositionType;
}


public class SplinePointManager : MonoBehaviour
{
    [SerializeField]
    public List<SplinePointInfo> pointInfos;

    [SerializeField]
    SplineContainer PlayerSpline;
    //List<SplineContainer> splines = new List<SplineContainer>();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //for(int i = 0; i < positionInfos.Count; ++i)
        //{
        //    for (int j = 0; j < splines.Count; ++j)
        //    {
        //        if (splines[j].gameObject.activeSelf)
        //        {
        //            foreach (float position in positionInfos[i].Positions)
        //            {
        //                Gizmos.color = new Color(1.0f, ((float)j / splines.Count), 1.0f);
        //                Gizmos.DrawSphere(splines[i].EvaluatePosition(position), 0.2f);
        //            }
        //        }
        //    }
        //}

        for (int i = 0; i < pointInfos.Count; ++i)
        {
            if (PlayerSpline.gameObject.activeSelf)
            {
                foreach (float position in pointInfos[i].Positions)
                {
                    Random.InitState(((int)pointInfos[i].PositionType));
                    Gizmos.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    Gizmos.DrawSphere(PlayerSpline.EvaluatePosition(Mathf.Repeat(position + GetComponent<SplineManager>().PlayerSpline.Origin, 1.0f)), 0.4f);
                }
            }
        }


    }
#endif

    public List<float> GetPointsOfSpecificType(SplinePointPositionType positionInfoType)
    {
        List<float> points = new List<float>();
        foreach (SplinePointInfo positionInfo in pointInfos)
        {
            if (positionInfo.PositionType == positionInfoType)
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
