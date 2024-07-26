using UnityEngine;

public enum BoundsArchetype
{
    FULL_PIPE = 0,
    HALF_PIPE = 1,
    THREE_QUARTERS_HOLE_LEFT = 2,
    THREE_QUARTERS_HOLE_RIGHT = 3
}

public class ActivityGeometry : MonoBehaviour
{
    [Header("Tail should be the \"Ending\" object")]
    [SerializeField] public GeometryTail _tail;
    [SerializeField] public SplineBoundsEditPlane _firstBoundsEditPlane;
    [SerializeField] public BoundsArchetype _boundsArchetype;
}
