using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationManager : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public OnSplineMovementController onSplineMovementController;
    float ratioAnimation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
            
        
        ratioAnimation = onSplineMovementController._OnGroundVelocityRatio * 3000;

        ratioAnimation=Mathf.Clamp(ratioAnimation, -100, 100);

        if (ratioAnimation > 0)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(1, 0);

            skinnedMeshRenderer.SetBlendShapeWeight(0, ratioAnimation);
        }
        else if(ratioAnimation < 0)
        {
            ratioAnimation *= -1;
            skinnedMeshRenderer.SetBlendShapeWeight(0, 0);

            skinnedMeshRenderer.SetBlendShapeWeight(1, ratioAnimation);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight(0, 0);
            skinnedMeshRenderer.SetBlendShapeWeight(1, 0);

        }
    }
}
