using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class CoinStreakManager : MonoBehaviour
{

    [SerializeField] private Coin NextExpectedCoin = null;

    [SerializeField] private UnityEvent OnCoinPickup;
    [SerializeField] private UnityEvent OnCoinFailed;
    [SerializeField] private UnityEvent OnStreakStarted;
    [SerializeField] private UnityEvent OnStreakSucceeded;
    [SerializeField] private UnityEvent OnStreakFailed;

    public void LogPickup()
    {
        Debug.Log($"<color=#FFFF00>CoinStreakManager.cs - Picked up coin</color>");
    }

    public void LogStreakLost()
    {
        Debug.Log($"<color=#FF0000>CoinStreakManager.cs - Lost streak</color>");
    }

    public void LogStreakStarted()
    {
        Debug.Log($"<color=#FFFF00>CoinStreakManager.cs - Started streak</color>");
    }
    public void LogStreakSuccess()
    {
        Debug.Log($"<color=#00FF00>CoinStreakManager.cs - Success streak</color>");
    }

    public void DestroyCoin(Coin coin)
    {
        OnCoinFailed?.Invoke();
        FailStreak(coin);
    }

    public bool IsCoinValid(Coin coin)
    {
        return (coin.IsFirstCoin || coin == NextExpectedCoin) && coin.IsValid;
    }

    public void PickupCoin(Coin coin)
    {
        if(IsCoinValid(coin))
        {
            OnCoinPickup?.Invoke();
            if(NextExpectedCoin == null)
            {
                StartStreak();
            }
        
            if (coin.NextCoin != null)
            {
                NextExpectedCoin = coin.NextCoin;
            }
            else
            {
                SucceedStreak();
            }
        }
        else
        {
            FailStreak(coin);
        }
    }

    public void StartStreak()
    {
        OnStreakStarted?.Invoke();
    }

    public void FailStreak(Coin currentCoin)
    {
        while(currentCoin.NextCoin != null)
        {
            currentCoin.IsValid = false;
            currentCoin = currentCoin.NextCoin;
        }

        OnStreakFailed?.Invoke();
        NextExpectedCoin = null;
    }

    public void SucceedStreak()
    {
        OnStreakSucceeded?.Invoke();
        NextExpectedCoin = null;
    }
}
