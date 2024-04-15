using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SkierController : MonoBehaviour
{

    [SerializeField] private GameObject ControlledPawn;
    [SerializeField] private LayerMask m_LayerMask;

    Transform ControlledPawnTransform;
    Camera MainCam;
    Vector3 NextPos;

    // Start is called before the first frame update
    void Start()
    {
        MainCam = Camera.main;
        ControlledPawnTransform = ControlledPawn.transform;
        NextPos = ControlledPawn.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touches.Length > 0)
        {
            Ray ray = MainCam.ScreenPointToRay(Input.GetTouch(0).position);

            // Create a particle if hit
            if (!Physics.Raycast(ray, 10.0f, ~m_LayerMask))
            {
                NextPos = MainCam.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 10.0f));
                Debug.Log(NextPos.ToString());

                ControlledPawnTransform.position = NextPos;
            }

            
        }
    }
}
