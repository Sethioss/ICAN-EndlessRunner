using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] public bool IsFirstCoin;
    [SerializeField] public Coin NextCoin;
    [HideInInspector] public bool IsValid = true;
}
