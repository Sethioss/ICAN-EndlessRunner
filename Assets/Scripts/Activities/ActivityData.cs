using UnityEngine;

[System.Serializable]
public class ActivityData
{
    [SerializeField] public AnimationCurve weightCurve;
    [SerializeField, Range(0, 1)] public float weight = 1;
    [SerializeField] public SOActivity activitySO;
    [SerializeField] public int _cooldown;

    private int Cooldown;

    public void SetCooldown(int newCoolDown)
    {
        Cooldown = newCoolDown;
    }
    public void SetCooldown()
    {
        Cooldown = _cooldown;
    }
    public void DecreaseCooldown()
    {
        Cooldown--;
    }
    public float GetFinalWeightAt(float t, bool FirstSpawn)
    {
        float mul = 1.0f;
        if(!FirstSpawn)
        {
            mul = Cooldown < 0 ? 1 : 0;
        }

        return weightCurve.Evaluate(t) * weight * mul;
    }
}
