using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] UnityEvent _onGameSceneLoaded;
    [SerializeField] public UnityEvent _onPause;
    [SerializeField] public UnityEvent _onResume;
    [SerializeField] public UnityEvent _onDeath;

    [HideInInspector] public bool GamePaused = false;
    [HideInInspector] public bool GameLost = false;

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

    public void ResetGameState()
    {
        ResumeGame();
        GameLost = false;
    }

    public static GameManager GetInstance()
    {
        return _instance;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        GamePaused = true;
        _onPause?.Invoke();
    }

    public void ResumeGame()
    {
        GamePaused = false;
        _onResume?.Invoke();
    }

    public void KillPlayer()
    {
        GameLost = true;
        _onDeath?.Invoke();
    }
}
