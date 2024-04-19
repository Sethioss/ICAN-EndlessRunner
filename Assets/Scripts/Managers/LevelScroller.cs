using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] float ScrollSpeed;

    [Header("Level scrolling slowing down when player hits an obstacle")]
    [SerializeField] float OnDamageTargetScroll = 0.0f;
    [SerializeField] float ScroolResumeTime;
    float MaxScrollSpeed;
    bool slowed = false;
    [SerializeField] AnimationCurve ScrollResumeCurve;
    float temp;

    [Header("Managers")]
    [SerializeField] SplineManager splineManager;
    [SerializeField] ObstacleManager ObstacleManager;

    List<float> TempFloats = new List<float>();

    private void Start()
    {
        MaxScrollSpeed = ScrollSpeed;
    }

    private void Update()
    {
        if(slowed)
        {
            temp = Mathf.Max(temp - Time.deltaTime, 0.0f);
            ScrollSpeed = Mathf.Lerp(MaxScrollSpeed, OnDamageTargetScroll, ScrollResumeCurve.Evaluate(temp / ScroolResumeTime));
            if(temp < 0.0f )
            { 
                slowed = false;
            }
        }

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

    public void SlowLevelBecauseOfHit()
    {
        slowed = true;
        ScrollSpeed = OnDamageTargetScroll;
        temp = ScroolResumeTime;
    }
}
