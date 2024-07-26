using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ActivityRepetitionConstraints
{
    [SerializeField] private SOActivity activitySO;
    [SerializeField] private int MaxRepetitions = 1;
    [SerializeField] private int ConstraintHistoryDepth = 1;
    [SerializeField] private int ForcedCooldown = 1;

    public SOActivity _ActivitySO => activitySO;
    public int _ForcedCooldown => ForcedCooldown;

    public bool DoesConstraintApply(List<SOActivity> ActivityList)
    {
        int skip = Mathf.Max(0, ActivityList.Count - ConstraintHistoryDepth);
        int sameActivityCount = ActivityList.Skip(skip).Count(x => x == activitySO);

        return sameActivityCount > MaxRepetitions;
    }
}
