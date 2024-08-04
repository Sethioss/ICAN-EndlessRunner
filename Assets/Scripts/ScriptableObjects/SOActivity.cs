using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Activity", menuName = "Level Elements / Activity", order = 1)]
public class SOActivity : ScriptableObject
{
    [SerializeField, FormerlySerializedAs("ActivityGeometry")] GameObject ActivityGeometryGO;
    [HideInInspector] public ActivityGeometry _Geometry => ActivityGeometryGO.GetComponent<ActivityGeometry>();
}
