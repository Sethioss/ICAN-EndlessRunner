using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Activity", menuName = "Level Elements / Activity", order = 1)]
public class SOActivity : ScriptableObject
{
    [SerializeField] GameObject ActivityGeometry;
    [HideInInspector] public ActivityGeometry _Geometry => ActivityGeometry.GetComponent<ActivityGeometry>();
}
