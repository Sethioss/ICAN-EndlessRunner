using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[InlineProperty(LabelWidth = 50)]
public class ActivityData
{
    [HorizontalGroup(100)]
    [SerializeField] public AnimationCurve weightCurve;
    [HorizontalGroup(200)]
    [SerializeField] public SOActivity activitySO;

    public float GetFinalWeightFromCurve(float t)
    {
        return weightCurve.Evaluate(t);
    }
}
