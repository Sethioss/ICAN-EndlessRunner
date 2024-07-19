using UnityEngine;

public class LevelSystemsHolder : MonoBehaviour
{
    [SerializeField] public SplineManager splineManager;
    [SerializeField] public OnSplineMovementController onSplineMovementController;
    [SerializeField] public ActivitiesSequenceGenerator obstacleManager;
    [SerializeField] public LevelScroller levelScroller;

}
