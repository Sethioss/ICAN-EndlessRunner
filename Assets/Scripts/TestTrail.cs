using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrail : MonoBehaviour
{
    LineRenderer lineRenderer;
    float res;
    [SerializeField] int _Resolution;
    [SerializeField] int _End;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = _Resolution;
        float dist = transform.position.z - _End;
        res = dist / _Resolution;
        /* for (int i = 0; i < _Resolution; i++)
         {
             lineRenderer.SetPosition(i, new Vector3(transform.position.x, transform.position.y, transform.position.z + res * i));
         }*/
        InvokeRepeating("UpdateLine", 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
        

    }
    void UpdateLine()
    {
        /*for (int i = 1; i < _Resolution; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i - 1);
            pos.z = transform.position.z + res * i;
            lineRenderer.SetPosition(i, pos);
        }
        lineRenderer.SetPosition(0, transform.position);*/
    }
}
