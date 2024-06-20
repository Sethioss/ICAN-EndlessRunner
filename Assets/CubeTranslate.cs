using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CubeTranslate : MonoBehaviour
{
    public float duration = 1f;
    private Material mat;
    //public AnimationCurve curve;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            mat = other.GetComponent<Renderer>().material;
            StartCoroutine(FadeMyShit());
        }
    }

    IEnumerator FadeMyShit()
    {
        float interpolator = 0;
        while(interpolator < 1)
        {
            interpolator += Time.deltaTime / duration;
            mat.SetFloat("_Transparent",interpolator);
            yield return new WaitForEndOfFrame();
        }
        mat.SetFloat("_Transparent", 1);
    }


}
