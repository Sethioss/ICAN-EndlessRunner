using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    private Material skyBoxMaterial;
    public float b ;
    public float s ;
    [HideInInspector] public int var = 1;
    // Start is called before the first frame update
    void Awake()
    {
        skyBoxMaterial = RenderSettings.skybox;
        b = 1;
        s = 0.10f;
}

    // Update is called once per frame
    void Update()
    {
        //gère la thickness de l'atmosphère ce qui donne la teinte rose au ciel
        if (b > 1.30)
        {
            var = -1;
        }
        if (b < 0.70){
            var = 1;
        }
        b = b + (0.00002f * var);
        skyBoxMaterial.SetFloat("_AtmosphereThickness", b);

        //gère la taille du soleil
        if (s < 0.80)
        {
        s = s + 0.00003f;
        skyBoxMaterial.SetFloat("_SunSize", s);
        }

    }
}
