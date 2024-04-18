using UnityEditor;
using UnityEngine;

public class ScreenDebugging : EditorWindow
{
    private static EditorWindow window;

    [MenuItem("Window/Android Screen Safe Area Debugging")]
    private static void ShowWindow()
    {
        window = GetWindow<ScreenDebugging>();
        window.titleContent = new GUIContent("Screen Debugging");
        window.Show();
    }

    private void OnGUI()
    {
        //On Android, min is Bottom left !  
        EditorGUILayout.LabelField("Safe Area width : " + Screen.safeArea.width);
        EditorGUILayout.LabelField("Safe Area height : " + Screen.safeArea.height);
        EditorGUILayout.LabelField("Safe Area y : " + Screen.safeArea.y);
        EditorGUILayout.LabelField("Safe Area y min : " + Screen.safeArea.yMin);
        EditorGUILayout.LabelField("Safe Area y max : " + Screen.safeArea.yMax);
        EditorGUILayout.LabelField("Safe Area x : " + Screen.safeArea.x);
        EditorGUILayout.LabelField("Safe Area x min : " + Screen.safeArea.xMin);
        EditorGUILayout.LabelField("Safe Area x max : " + Screen.safeArea.xMax);
    }
}