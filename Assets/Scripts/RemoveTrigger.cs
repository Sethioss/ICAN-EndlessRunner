using UnityEngine;

public class RemoveTrigger : MonoBehaviour
{
    [SerializeField] public ActivitiesSequenceGenerator om;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile"))
        {
            //om.Temp++;
            om.DeleteActivity(other.gameObject);
            //if(om.Temp >= om.NumberOfActivitiesSpawnedOnStart / 2)
            //{
            //    Debug.Log("Spawn new");
            //
            //    om.MakeNewIncomingActivitiesList(om.NumberOfActivitiesSpawnedOnStart / 2);
            //    om.SpawnActivitiesGeometry();
            //
            //    om.Temp = 0;
            //}
        }
    }
}
