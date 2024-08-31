using UnityEngine;

public class LevelSystemsHolder : MonoBehaviour
{
    private static LevelSystemsHolder _instance;

    [SerializeField] public SplineManager splineManager;
    [SerializeField] public OnSplineMovementController onSplineMovementController;
    [SerializeField] public ActivitiesSequenceGenerator obstacleManager;
    [SerializeField] public LevelScroller levelScroller;

    public void Awake()
    {
        _instance = this;
    }

    public static LevelSystemsHolder GetInstance()
    {
        return _instance;
    }
}
