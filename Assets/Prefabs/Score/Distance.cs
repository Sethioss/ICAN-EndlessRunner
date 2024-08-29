using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Distance : MonoBehaviour
{
    public float distance;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        distance = 0f; 
    }

    // Update is called once per frame
    void Update()
    {
        distance += Time.deltaTime;
        Mathf.Round(distance);
        text.text = distance.ToString("F1");
    }
}
