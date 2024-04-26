using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSingleton : MonoBehaviour
{
    private static ManagerSingleton _instance;

    [SerializeField] InputManager inputManager;
    [SerializeField] SplineManager splineManager;
    [SerializeField] ObstacleManager obstacleManager;
    [SerializeField] OnSplineMovementController onSplineMovementController;
    [SerializeField] LevelScroller levelScroller;

    public void Start()
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
    public ManagerSingleton GetInstance()
    {
        return _instance;
    }

}
