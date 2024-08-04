using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ActivitySplineBoundsBasedInsertionProcessor
{
    [SerializeField] private List<ActivitiesTransitionInfo> ReplacementActivities;
    public List<SOActivity> Process(List<SOActivity> SelectedActivities, ActivityGeometry PreviousActivityGeometry)
    {
        List<ActivityGeometry> Geometries = new List<ActivityGeometry>();
        List<SOActivity> TempActivities = new List<SOActivity>(SelectedActivities);
        int AddedGeometriesCount = 0;

        if(PreviousActivityGeometry)
        {
            Geometries.Insert(0, PreviousActivityGeometry);
        }
        for(int i = SelectedActivities.Count - 1; i >= 0; --i)
        {
            Geometries.Insert(0, SelectedActivities[i]._Geometry);
        }

        if(Geometries.Count <= 1)
        {
           return SelectedActivities;
        }


        for(int i = Geometries.Count - 2; i >= 0; --i)
        {
            if (Geometries[i]._boundsArchetype != Geometries[i + 1]._boundsArchetype)
            {
                //Get activity transition
                ActivitiesTransitionInfo SelectedTransition = null;
                SelectedTransition = ReplacementActivities.FirstOrDefault(x => x._from == Geometries[i+1]._boundsArchetype && x._to == Geometries[i]._boundsArchetype);

                if (SelectedTransition == null || SelectedTransition._activity == null)
                {
                   Debug.LogError($"<color=#FFFF00>SplineBoundsBasedProcessor:: The selected transition or relevant activity was NULL. Please check the processor's configuration. Reverting to default</color>" +
                       $"<color=#CFB01F>\n Info: ActivityTransition supposed to trigger: {Geometries[i].name} at index <color=#d512be>{i}</color> to {Geometries[i + 1].name} at index <color=#d512be>{i + 1}</color> ({Geometries[i]._boundsArchetype} to {Geometries[i + 1]._boundsArchetype}</color>)");
                }
                else
                {
                    TempActivities.Insert((i + 1), SelectedTransition._activity);
                    AddedGeometriesCount++;
                }
            }
        }

        TempActivities.Reverse();
        return TempActivities;
    }
}
