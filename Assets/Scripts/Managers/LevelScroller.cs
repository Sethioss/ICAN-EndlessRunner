using System.Collections.Generic;
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] float ScrollSpeed;
    [SerializeField] public float DistanceTraveled = 0.0f;

    [Header("Level scrolling slowing down when player hits an obstacle")]
    [SerializeField] float OnDamageTargetScroll = 0.0f;
    [SerializeField][Min(0.1f)] float ScrollResumeTime;
    float MaxScrollSpeed;
    bool slowed = false;
    [SerializeField] AnimationCurve ScrollResumeCurve;
    float temp;

    [Header("Managers")]
    [SerializeField] SplineManager splineManager;
    [SerializeField] ActivitiesSequenceGenerator ActivitiesSequenceGenerator;

    List<float> TempFloats = new List<float>();

    //TODO: Put this in a component, interface or system that can easily be placed in multiple scripts ("GameStateDependantExecution?")
    private bool bAllowedToProceed = true;

    protected virtual void OnEnable()
    {
        GameManager.GetInstance()._onDeath.AddListener(() => bAllowedToProceed = false);
        GameManager.GetInstance()._onPause.AddListener(() => bAllowedToProceed = false);
        GameManager.GetInstance()._onResume.AddListener(() => bAllowedToProceed = true);
    }

    void OnDisable()
    {
        GameManager.GetInstance()._onDeath.RemoveListener(() => bAllowedToProceed = false);
        GameManager.GetInstance()._onPause.RemoveListener(() => bAllowedToProceed = false);
        GameManager.GetInstance()._onResume.RemoveListener(() => bAllowedToProceed = true);
    }

    private void Start()
    {
#if UNITY_EDITOR
        //ScrollSpeed *= (10.0f/25.0f);
#endif
        MaxScrollSpeed = ScrollSpeed;
    }

    private void Update()
    {
        if(bAllowedToProceed)
        {
            DistanceTraveled += ScrollSpeed * Time.deltaTime;

            if (slowed)
            {
                temp = Mathf.Max(temp - Time.deltaTime, 0.0f);
                if (ScrollResumeTime > 0.0f)
                {
                    ScrollSpeed = Mathf.Lerp(MaxScrollSpeed, OnDamageTargetScroll, ScrollResumeCurve.Evaluate(temp / ScrollResumeTime));
                }
                else
                {
                    ScrollSpeed = MaxScrollSpeed;
                }

                if (temp < 0.0f)
                {
                    slowed = false;
                }
            }

            for (int i = 0; i < ActivitiesSequenceGenerator.InstantiatedActivitiesGO.Count; ++i)
            {
                GameObject obj = ActivitiesSequenceGenerator.InstantiatedActivitiesGO[i].gameObject;
                obj.transform.position += ((-obj.transform.forward) * ScrollSpeed) * Time.deltaTime;
            }
        }     
    }

    public void SlowLevelBecauseOfHit()
    {
        slowed = true;
        ScrollSpeed = OnDamageTargetScroll;
        temp = ScrollResumeTime;
    }
}
