using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ActivityData
{
    [SerializeField] public AnimationCurve weightCurve;
    [SerializeField] public SOActivity activitySO;
    [SerializeField, Range(0, 1)] public float Weight = 1;
    [SerializeField] public int CooldownDuration = 1;

    private int Cooldown;

    public float GetFinalWeightAt(float t)
    {
        float cdWeight = Cooldown < 0 ? 1f : 0f;
        return weightCurve.Evaluate(t) * cdWeight * Weight;
    }

    public int GetCooldown()
    {
        return Cooldown;
    }

    public void SetCooldown()
    {
        Cooldown = CooldownDuration;
    }
    public void SetCooldown(int cd)
    {
        Cooldown = cd;
    }

    public void DecreaseCooldown()
    {
        Cooldown--;
    }
}
