using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] SplineManager splineManager;
    [SerializeField] ObstacleManager obstacleManager;
    [SerializeField] float ScrollSpeed;

    List<float> TempFloats = new List<float>();

    private void Start()
    {
        for(int i = 0; i < obstacleManager.obstacleInfos.Count; i++)
        {
            TempFloats.Insert(i, obstacleManager.obstacleInfos[i].locationOnSpline);
        }
    }

    private void Update()
    {
        for(int i = 0; i < obstacleManager.obstacleInfos.Count; ++i)
        {
            ObstacleInfo info = obstacleManager.obstacleInfos[i];
            GameObject obj = obstacleManager.instantiatedGOs[i];
            obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
            if (info.movesAlongSpline)
            {
                TempFloats[i] = Mathf.Repeat(TempFloats[i] + (info.moveAlongSplineSpeed * Time.deltaTime), 1.0f);
                Debug.Log(TempFloats[i]);

                Vector3 PosAlongSpline = new Vector3(splineManager.PlayerSpline.PlayerSpline.EvaluatePosition(TempFloats[i]).x,
                    splineManager.PlayerSpline.PlayerSpline.EvaluatePosition(TempFloats[i]).y,
                    obj.transform.position.z);

                obj.transform.position = PosAlongSpline;
            }
        }
    }
}
