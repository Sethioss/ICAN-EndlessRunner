using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] UnityEvent _onGameSceneLoaded;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameManager GetInstance()
    {
        return _instance;
    }
}
