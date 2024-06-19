using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIOverlayToSafeArea : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(Screen.safeArea.xMin, Screen.safeArea.yMin);
        rectTransform.sizeDelta = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
    }
}
