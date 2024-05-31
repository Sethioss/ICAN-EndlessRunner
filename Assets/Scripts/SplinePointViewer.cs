using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePointViewer : MonoBehaviour
{
    [SerializeField]
    List<SplineContainer> splines = new List<SplineContainer>();

    [SerializeField]
    [Range(0, 1)]
    private List<float> Positions;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        SplineContainer spline = GetComponent<SplineContainer>();
        for(int i = 0; i < splines.Count; i++)
        {
            if (splines[i].gameObject.activeSelf)
            {
                foreach (float position in Positions)
                {
                    Gizmos.color = new Color(1.0f, ((float)i / splines.Count), 1.0f);
                    Gizmos.DrawSphere(splines[i].EvaluatePosition(position), 0.2f);
                }
            }
        }
    }
#endif
}
