using UnityEngine;

public class RemoveTrigger : MonoBehaviour
{
    [SerializeField] public ActivitiesSequenceGenerator om;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile"))
        {
            om.DeleteActivity(other.gameObject);
        }
    }
}
