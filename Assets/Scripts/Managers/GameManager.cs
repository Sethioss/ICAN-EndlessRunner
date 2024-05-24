using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] public SplineManager splineManager;
    [SerializeField] public ActivitiesSequenceGenerator obstacleManager;
    [SerializeField] public OnSplineMovementController onSplineMovementController;
    [SerializeField] public LevelScroller levelScroller;

    [SerializeField] public UnityEvent onGameSceneLoaded;

    public void Awake()
    {
        if(_instance == null)
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
