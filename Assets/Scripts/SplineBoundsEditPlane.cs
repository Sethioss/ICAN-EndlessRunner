using UnityEngine;

#if UNITY_EDITOR
[System.Serializable, 
RequireComponent(typeof(SplineBoundsEditPlanePreviewer))]
#endif
public class SplineBoundsEditPlane : MonoBehaviour
{
    [SerializeField] public bool PreviewEditPlane = false;

    [SerializeField] public SplinePointInfo pointInfo;
    [SerializeField] public ActivityGeometry relatedGeometry;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(PreviewEditPlane)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
#endif
}
