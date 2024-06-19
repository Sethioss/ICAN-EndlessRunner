using Lean.Touch;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField][Range(0, 1)] private float _startPosition;
    [SerializeField] private GameObject _playerObject;
    //[SerializeField] private PlayerMoveState _playerMoveState;
    private Rigidbody _rb;

    [SerializeField] private SplineManager _splineManager;
    private SplineContainer _spline;

#if UNITY_EDITOR
    [Header("Visualisation")]
    [SerializeField] bool _previewPlayerStartPos = false;
#endif

    [Header("Controls - Higher values = More drag")]
    [SerializeField] private float _velocity = 0;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed;
    [Tooltip("Acceleration on Start")]
    [SerializeField] private float normalAccel;
    [Tooltip("Deceleration in Direction change")]
    [SerializeField] private float changeSideDeceleration;
    [Tooltip("Deceleration when being stopped")]
    [SerializeField] private float deceleration;
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve decelCurve;

    private float _movementLerpValue;

    [Header("Ratios - Used for animation")]
    [SerializeField] [Range(-1, 1)] private float _OnGroundVelocityRatio;


    private Camera _mainCam;
    private float _direction = 0;
    private float _lastDir = 0;

    private float _positionOnSpline = 0;

    [Header("Particles Test")]
    [SerializeField] private ParticleSystem _ParticleRight;
    [SerializeField] private ParticleSystem _ParticleLeft;

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnGesture += HandleFingerDebug;
        LeanTouch.OnFingerUpdate += UpdateFinger;
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    // Start is called before the first frame update
    void Start()
    {
        _mainCam = Camera.main;

        _spline = _splineManager.PlayerSpline.PlayerSpline;

        _rb = _playerObject.GetComponent<Rigidbody>();

        _positionOnSpline = _startPosition;
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

        _velocity = Mathf.Clamp(_velocity + _direction * Time.deltaTime * accelCurve.Evaluate(_movementLerpValue) , -_maxSpeed, _maxSpeed);
        _movementLerpValue += Time.deltaTime;

        if (_direction == 0f)
        {
            if (_velocity > 0.001f || _velocity < -0.001f)
            {
                accel = deceleration;
                float _calculatedVelocity = _velocity + decelCurve.Evaluate(_movementLerpValue) * -(Mathf.Sign(_velocity)) * Time.deltaTime;
                _velocity = Mathf.Clamp(_calculatedVelocity, -_maxSpeed, _maxSpeed);

                //Never fully stop after having pressed a direction at least once
                if(_velocity <= _minSpeed && _velocity >= -_minSpeed && _velocity != 0.0f)
                {
                    float _negativeVelocity = _velocity >= 0.0f ? 1 : -1;
                    _velocity = _minSpeed * _negativeVelocity;
                }
                _movementLerpValue += Time.deltaTime;
            }
            else
            {
                {
                    _velocity = 0;
                }
            }
        }

        _OnGroundVelocityRatio = (_velocity / _maxSpeed);
        AddSplinePositionOffset(_velocity);

        //Set player on corresponding spline position
        _playerObject.transform.position = _spline.EvaluatePosition(_positionOnSpline);
        _playerObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _spline.EvaluateUpVector(_positionOnSpline));
    }
    public void UpdateXDirection(float XDirection)
    {
        if(_lastDir == 0)
        {
            _direction = XDirection;
        }
        else
        {
            if (_lastDir == XDirection)
            {
                _direction = XDirection;
            }
        }
        _lastDir = _direction;
    }
    public void AddSplinePositionOffset(float value)
    {
        //Spline is reversed
        float _finalPosOnSpline = _positionOnSpline - value;

        float[] Positions = _splineManager.GetCurrentBoundsPositions();
        bool ValidPosition = false;

        if (!(Mathf.Abs(Positions[0] - Positions[1]) == 1.0f) || (Mathf.Abs(Positions[0] - Positions[1]) == 0.0f))
        {
            if (_splineManager.CurrentBoundPassesByOrigin(_splineManager.CurrentBounds))
            {
                ValidPosition = _finalPosOnSpline >= Positions[0] || _finalPosOnSpline <= Positions[1];
            }
            else
            {
                ValidPosition = _finalPosOnSpline >= Positions[0] && _finalPosOnSpline <= Positions[1];
            }

            if(ValidPosition )
            {
                _positionOnSpline = Mathf.Repeat(_finalPosOnSpline, 1.0f);
            }
            else
            {
                _velocity = 0;
            }
        }
        else
        {
            _positionOnSpline = Mathf.Repeat(_finalPosOnSpline, 1.0f);
        }

    }
    public void StopAcceleration()
    {
        _direction = 0;
    }

    #endregion

    public void HandleFingerDebug(List<LeanFinger> Fingers)
    {
        if (Fingers.Count > 2)
        {
            if (Fingers[1].Age > 2.0f)
            {
                Fingers[1].Age = 0.0f;
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }

    public void HandleFingerUp(LeanFinger finger)
    {
        StopAcceleration();
        _movementLerpValue = 0;
    }

    public void UpdateFinger(LeanFinger finger)
    {
        if (finger.Index == 0)
        {
            UpdateXDirection(GetNormalisedXDirection(finger.ScreenPosition.x));
        }
    }

    public void HandleFingerDown(LeanFinger finger)
    {
        if (finger.Index == 0)
        {
            float Dir = GetNormalisedXDirection(finger.ScreenPosition.x);
            if (Dir < 0)
            {
                //LancerSonGauche
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/SOUND-GlisseTurn");
                //ParticuleAGauche
                _ParticleLeft.Play();
                _ParticleRight.Stop();
            }
            else if (Dir != 0)
            {
                //LancerSonDroite
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/SOUND-GlisseTurn");
                //ParticuleAGauche
                _ParticleLeft.Stop();
                _ParticleRight.Play();
            }
        }
        _movementLerpValue = 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_previewPlayerStartPos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_splineManager.GetComponent<SplinePointManager>().PlayerSpline.EvaluatePosition(_startPosition), new Vector3(0.8f, 0.8f, 0.8f));
        }
    }
#endif
}