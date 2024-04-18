using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObstacleInfo
{
    public GameObject obstacle;
    [Header("Location takes user defined spline origin in consideration")]
    public float locationOnSpline;
    public float depth;

    public bool movesAlongSpline;
    public float moveAlongSplineSpeed;
}

public class OldObstacleManager : MonoBehaviour
{
    [SerializeField] private SplineManager splineManager;
    [SerializeField] public List<ObstacleInfo> obstacleInfos = new List<ObstacleInfo>();
    [HideInInspector] public List<GameObject> instantiatedGOs = new List<GameObject>();
    private void Start()
    {
        SplineInfo cachedSplineInfo = splineManager.PlayerSpline;
        foreach(ObstacleInfo info in obstacleInfos)
        {
            Vector3 posOnSpline = splineManager.PlayerSpline.PlayerSpline.EvaluatePosition(Mathf.Repeat(info.locationOnSpline + splineManager.PlayerSpline.Origin, 1.0f));
            instantiatedGOs.Add(Instantiate(info.obstacle, new Vector3(posOnSpline.x, posOnSpline.y, info.depth), Quaternion.LookRotation(info.obstacle.transform.forward, cachedSplineInfo.PlayerSpline.EvaluateUpVector(info.locationOnSpline))));
        }
    }
}
