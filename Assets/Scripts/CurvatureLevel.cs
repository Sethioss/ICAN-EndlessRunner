using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CurvatureLevel : MonoBehaviour
{
    public Material curvPipe;
    public Material curvObstacle;
    public float b;
    private float c;
    private float t;
    private float upDown = 0f;
    private float side = 0f;
    private float dc = 1;
    private void Start()
    {   
        b = Random.Range(200, 2000);
    }
    private void Update()
    {
        b--;
        if (b < 0)
        {
            if (dc > 0) 
            {
                upDown = Mathf.Lerp(0f, 0.004f, t);
                curvPipe.SetFloat("_BackwardStrenght", upDown);
                upDown = Mathf.Lerp(0f, 0.004f, t);
                curvPipe.SetFloat("_SidewayStrenght", upDown);

                upDown = Mathf.Lerp(0f, 0.004f, t);
                curvObstacle.SetFloat("_BackwardStrenght", upDown);
                upDown = Mathf.Lerp(0f, 0.004f, t);
                curvObstacle.SetFloat("_SidewayStrenght", upDown);

                // .. and increase the t interpolater
                t += 0.05f * Time.deltaTime;

            }
            if (dc < 0)
            {
                upDown = Mathf.Lerp(0.004f, 0, t);
                curvObstacle.SetFloat("_BackwardStrenght", upDown);
                side = Mathf.Lerp(0.004f, 0, t);
                curvObstacle.SetFloat("_SidewayStrenght", side);

                upDown = Mathf.Lerp(0.004f, 0, t);
                curvObstacle.SetFloat("_BackwardStrenght", upDown);
                side = Mathf.Lerp(0.004f, 0, t);
                curvObstacle.SetFloat("_SidewayStrenght", side);
                // .. and increase the t interpolater
                t += 0.05f * Time.deltaTime;

            }

            if (t > 1.0f)
            {
                t = 0f;
                dc *= -1;
            }
        }
    }

}
