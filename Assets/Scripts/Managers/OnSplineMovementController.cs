using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Splines;

public enum PlayerMoveState
{
    ACCELERATING = 0,
    DECELERATING = 1,
    LAUNCHED = 2,
    STOPPED = 3
}

public class OnSplineMovementController : MonoBehaviour
{
    [SerializeField] private GameObject _playerObject;
    //[SerializeField] private PlayerMoveState _playerMoveState;
    private Rigidbody _rb;

    [SerializeField] private SplineManager _splineManager;
    private SplineContainer _spline;

    [SerializeField] private bool _loopsBack;
    //
    //[Header("Acceleration")]
    //[SerializeField] private float _accelerationTime = 0;
    //[SerializeField] private float _tempAcceleration = 0;
    //
    //private float _finalAcceleration = 0;
    //
    //[Header("Deceleration")]
    //[SerializeField] private float _decelerationTime = 0;
    //[SerializeField] private float _tempDeceleration = 0;
    //
    //
    //[Header("Side launch")]
    //[SerializeField] private float _sideLaunchSpeed;
    //[SerializeField] private float _sideLaunchCooldown = 0;
    //private float _tempSideLaunchCooldown = 0;
    //private bool _isShortTapAvailable = false;
    //
    //
    //[Header("Debug")]
    //[SerializeField] private float _accelerationRatio = 0;
    //[SerializeField] private float _decelerationRatio = 0;

    private float _velocity = 0;
    [Header("Controls - Higher values = More drag")]
    [SerializeField] private float _maxSpeed;
    [Tooltip("Acceleration on Start")]
    [SerializeField] private float normalAccel;
    [Tooltip("Deceleration in Direction change")]
    [SerializeField] private float changeSideDeceleration;
    [Tooltip("Deceleration when being stopped")]
    [SerializeField] private float deceleration;


    private Camera _mainCam;
    private float _direction = 0;

    private float _positionOnSpline = 0;


    // Start is called before the first frame update
    void Start()
    {
        _mainCam = Camera.main;

        _spline = _splineManager.PlayerSpline.PlayerSpline;

        _rb = _playerObject.GetComponent<Rigidbody>();

        _positionOnSpline = _splineManager.PlayerSpline.Origin;
        _playerObject.transform.position = _spline.EvaluatePosition(_positionOnSpline);
    }

    #region Misc
    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    #endregion

    #region Movement

    private void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        float accel = normalAccel;
        if (Mathf.Sign(_velocity) != Mathf.Sign(_direction))
        {
            accel = changeSideDeceleration;
        }
        _velocity = Mathf.Clamp(_velocity + accel * _direction * Time.deltaTime, -_maxSpeed, _maxSpeed);

        if (_direction == 0f)
        {
            if (_velocity > 0.001f || _velocity < -0.001f)
            {
                accel = deceleration;
                _velocity = Mathf.Clamp(_velocity + accel * -(Mathf.Sign(_velocity)) * Time.deltaTime, -_maxSpeed, _maxSpeed);
            }
            else
            {
                {
                    _velocity = 0;
                }
            }

        }


        AddSplinePositionOffset(_velocity);

        //Set player on corresponding spline position
        _playerObject.transform.position = _spline.EvaluatePosition(_positionOnSpline);
        _playerObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _spline.EvaluateUpVector(_positionOnSpline));

    }
    public void UpdateXDirection(float XDirection)
    {
        _direction = XDirection;
    }
    public void AddSplinePositionOffset(float value)
    {
        //Spline is reversed
        _positionOnSpline -= value;

        if (_loopsBack)
        {
            _positionOnSpline = Mathf.Repeat(_positionOnSpline, 1.0f);
        }
        else
        {
            _positionOnSpline = Mathf.Clamp(_positionOnSpline, 0.0f, 1.0f);
        }

    }
    public void StopAcceleration()
    {
        _direction = 0;
    }
    
    #endregion
}
