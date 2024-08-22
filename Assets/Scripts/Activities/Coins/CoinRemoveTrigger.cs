using UnityEngine;

public class CoinRemoveTrigger : MonoBehaviour
{
    [SerializeField] private CoinStreakManager _coinStreakManager;
    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonus"))
        {
            _coinStreakManager.DestroyCoin(other.GetComponent<Coin>());
        }
    }
}
