using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum InsertionType
{
    Before,
    Middle,
    After,
}

[System.Serializable]
public class InsertionObstacleProcessor
{
    [SerializeField] private SOActivity _activitySOToCheck;
    [SerializeField] private SOActivity _activitySOToInsert;
    [SerializeField] private int _replacingActivitiesNumber;
    [SerializeField] private int _MaxRepetitions = 1;
    [SerializeField] private InsertionType _InsertType;

    public SOActivity ProcessSingle(SOActivity CurrentActivity, List<SOActivity> SelectedActivities)
    {
        int Repetition = _MaxRepetitions;

        for (int i = SelectedActivities.Count - 1; i < 0; --i)
        {
            if (SelectedActivities[i] != _activitySOToCheck)
            {
                Repetition = _MaxRepetitions;
                continue;
            }

            if (Repetition <= 0)
            {
                Debug.Log("Found repetition");
                return _activitySOToInsert;
            }
            else
            {
                Repetition--;
            }
        }
        return CurrentActivity;
    }

    public List<SOActivity> Process(List<SOActivity> SelectedActivities)
    {
        int Repetition = _MaxRepetitions;
        for (int i = 0; i < SelectedActivities.Count; ++i)
        {
            if (SelectedActivities[i] != _activitySOToCheck)
            {
                Repetition = _MaxRepetitions;
                continue;
            }

            if (Repetition <= 0)
            {
                switch (_InsertType)
                {
                    case InsertionType.Before:
                        for (int j = 0; j < _replacingActivitiesNumber; ++j)
                        {
                            SelectedActivities.Insert(i - _MaxRepetitions, _activitySOToInsert);
                            i++;
                        }
                        break;

                    case InsertionType.Middle:
                        for (int j = 0; j < _replacingActivitiesNumber; ++j)
                        {
                            SelectedActivities.Insert(i - (_MaxRepetitions / 2), _activitySOToInsert);
                            i++;
                        }
                        break;

                    case InsertionType.After:
                        for (int j = 0; j < _replacingActivitiesNumber; ++j)
                        {
                            SelectedActivities.Insert(i, _activitySOToInsert);
                            i++;
                        }
                        break;
                }

                Repetition = _MaxRepetitions;
            }
            else
            {
                Repetition--;
            }
        }
        return SelectedActivities;
    }
}
