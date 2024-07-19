using UnityEngine;

public class ActivityGeometry : MonoBehaviour
{
    [Header("Tail should be the \"Ending\" object")]
    [SerializeField] public GeometryTail _tail;
    [SerializeField] public SplineBoundsEditPlane _firstBoundsEditPlane;
}
