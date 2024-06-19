using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TileDifficulty
{
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}

public class ObstacleTile : MonoBehaviour
{
    [SerializeField] public TileDifficulty tileDifficulty;
    [SerializeField] public Transform tail;
}
