using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] SplineManager splineManager;
    [SerializeField] OldObstacleManager OldObstacleManager;
    [SerializeField] ObstacleManager ObstacleManager;

    [SerializeField] float ScrollSpeed;

    List<float> TempFloats = new List<float>();

    private void Start()
    {
        for(int i = 0; i < OldObstacleManager.obstacleInfos.Count; i++)
        {
            TempFloats.Insert(i, OldObstacleManager.obstacleInfos[i].locationOnSpline);
        }
    }

    private void Update()
    {
        for(int i = 0; i < ObstacleManager.InstantiatedTiles.Count; ++i)
        {
            GameObject obj = ObstacleManager.InstantiatedTiles[i];
            obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
        }
        for(int i = 0; i < OldObstacleManager.obstacleInfos.Count; ++i)
        {
            ObstacleInfo info = OldObstacleManager.obstacleInfos[i];
           /* GameObject obj = OldObstacleManager.instantiatedGOs[i];
            obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
            if (info.movesAlongSpline)
            {
                TempFloats[i] = Mathf.Repeat(TempFloats[i] + (info.moveAlongSplineSpeed * Time.deltaTime), 1.0f);

                Vector3 PosAlongSpline = new Vector3(splineManager.PlayerSpline.PlayerSpline.EvaluatePosition(TempFloats[i]).x,
                    splineManager.PlayerSpline.PlayerSpline.EvaluatePosition(TempFloats[i]).y,
                    obj.transform.position.z);

                obj.transform.position = PosAlongSpline;
            }*/
        }
    }
}
