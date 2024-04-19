using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] SplineManager splineManager;
    [SerializeField] ObstacleManager ObstacleManager;

    [SerializeField] float ScrollSpeed;

    List<float> TempFloats = new List<float>();

    private void Update()
    {
        if(ObstacleManager.LeadingTileObject != null)
        {
            GameObject obj = ObstacleManager.LeadingTileObject;
            obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
        }

        for (int i = 0; i < ObstacleManager.InstantiatedTiles.Count; ++i)
        {
            GameObject obj = ObstacleManager.InstantiatedTiles[i];
            obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
        }
    }
}
