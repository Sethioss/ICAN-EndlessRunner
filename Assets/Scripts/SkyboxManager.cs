using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material skyBoxMaterial;
    public float b = 1;
    public float s = 0.010f;
    public int var = 1;
    // Start is called before the first frame update
    void Awake()
    {
        skyBoxMaterial = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        //gère la thickness de l'atmosphère ce qui donne la teinte rose au ciel
        if (b > 1.30)
        {
            var = -1;
        }
        if (b <0.50){
            var = 1;
        }
        b = b + (0.00001f * var);
        skyBoxMaterial.SetFloat("_AtmosphereThickness", b);

        //gère la taille du soleil
        if (s < 0.50)
        {
        s = s + 0.0000000001f;
        skyBoxMaterial.SetFloat("_SunSize", s);
        }

    }
}
