using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static Rect GetSafeRect()
    {
        Rect SafeArea = Screen.safeArea;

        float xOffset = SafeArea.xMin / Screen.width;
        float yOffset = SafeArea.yMin / Screen.height;
        Vector2 Offset = new Vector2(xOffset, yOffset);

        float xPercentage = (SafeArea.xMax - SafeArea.xMin) / Screen.currentResolution.width;
        float yPercentage = (SafeArea.yMax - SafeArea.yMin) / Screen.currentResolution.height;
        Vector2 Percentage = new Vector2(xPercentage, yPercentage);

        return new Rect(Offset, Percentage);
    }
}
