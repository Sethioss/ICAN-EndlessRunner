using UnityEngine;

public class LevelSystemsHolder : MonoBehaviour
{
    [SerializeField] SplineManager splineManager;
    [SerializeField] OnSplineMovementController onSplineMovementController;
    [SerializeField] public ActivitiesSequenceGenerator obstacleManager;
    [SerializeField] public LevelScroller levelScroller;

}
