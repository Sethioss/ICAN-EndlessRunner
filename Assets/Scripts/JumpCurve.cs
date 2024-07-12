using UnityEngine;

[System.Serializable]
public class JumpCurve
{
    [SerializeField, Min(0f)] public float MinimumRequiredVelocity;
    [SerializeField, Min(0f)] public float SideJumpImpulseForce;
    [SerializeField, Min(0f)] public float MinJumpHeight;
    [SerializeField, Min(0f)] public float MaxJumpHeight;
    [SerializeField] public AnimationCurve Curve;
}
