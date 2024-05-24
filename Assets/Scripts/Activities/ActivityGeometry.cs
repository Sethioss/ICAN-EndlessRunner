using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivityGeometry : MonoBehaviour
{
    [Header("Tail should be the \"Ending\" object")]
    [SerializeField] public GeometryTail _tail;
}
