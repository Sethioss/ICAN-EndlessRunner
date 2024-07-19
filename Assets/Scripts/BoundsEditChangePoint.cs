using UnityEngine;

public class BoundsEditChangePoint : MonoBehaviour
{
    [SerializeField] private SplineManager splineManager;
    [SerializeField] private OnSplineMovementController movementController;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BoundsEditPlane"))
        {
            SplineBoundsEditPlane CurPlane = other.GetComponent<SplineBoundsEditPlane>();
            if (splineManager.CurrentBounds != CurPlane.pointInfo)
            {
                splineManager.ChangeCurrentBounds(CurPlane.pointInfo, CurPlane);
                if(!movementController._Airborne)
                {
                    movementController.CorrectPlayerBackOnSpline();
                }
            }
        }
    }
}
