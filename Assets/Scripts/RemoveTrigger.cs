using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTrigger : MonoBehaviour
{
    [SerializeField] public ObstacleManager om;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile"))
        {
            om.CreateNewObstacle(other.gameObject);
        }
    }
}
