using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
