using UnityEngine;

[System.Serializable]
public class ActivityData
{
    [SerializeField] public AnimationCurve weightCurve;
    [SerializeField] public SOActivity activitySO;

    public float GetFinalWeightFromCurve(float t)
    {
        return weightCurve.Evaluate(t);
    }
}
