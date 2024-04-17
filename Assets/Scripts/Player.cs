using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MeshRenderer PlayerMesh;
    [SerializeField] Material StandardMat;
    [SerializeField] Material HurtMat;
    [SerializeField] Material RichMat;

    bool isSpecialStatus = false;
    [SerializeField] float specialStatusTime;
    float tempSpecialStatusTime;

    private void Start()
    {
        PlayerMesh.material = StandardMat;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
        {
            isSpecialStatus = true;
            other.gameObject.SetActive(false);
            PlayerMesh.material = HurtMat;
        }

        if (other.gameObject.CompareTag("Bonus"))
        {
            isSpecialStatus = true;
            other.gameObject.SetActive(false);
            PlayerMesh.material = RichMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMesh.material = StandardMat;
    }

    private void Update()
    {
         if(isSpecialStatus) 
        {
            tempSpecialStatusTime += Time.deltaTime;
            if(tempSpecialStatusTime > specialStatusTime)
            {
                tempSpecialStatusTime = 0;
                isSpecialStatus = false;
                PlayerMesh.material = StandardMat;
            }
        }
    }
}
