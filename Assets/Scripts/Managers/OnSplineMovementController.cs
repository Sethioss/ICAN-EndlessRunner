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
    [HideInInspector] public bool _Airborne = false;

    [SerializeField] private float _sideJumpImpulseForce = 400;
    [SerializeField] private float _lastVelocity;

    private Vector3 _LastValidPointPosition;
    [SerializeField] private float _postSideLaunchCooldownBeforeGroundCheck = 0.2f;
    private float _tempPostSideLaunchCooldownBeforeGroundCheck = 0.0f;

    [SerializeField] private float _postSideLaunchCooldownAfterGroundCheck = 1.0f;
    private float _tempPostSideLaunchCooldownAfterGroundCheck = 0.0f;
    [SerializeField] bool _CanAirAgain = true;

    bool PostSideJump = false;

    [SerializeField] private GameObject _playerObject;
    //[SerializeField] private PlayerMoveState _playerMoveState;
    private Rigidbody _rb;

    [SerializeField] private SplineManager _splineManager;
    private SplineContainer _spline;

    //Whether the loop is closed (Loops the player's position on spline between 0 and 1) or not
    [SerializeField] private bool _loopsBack;

    private float _velocity = 0;
    [Header("Controls - Higher values = More drag")]
    [SerializeField] private float _maxSpeed;
    [Tooltip("Acceleration on Start")]
    [SerializeField] private float normalAccel;
    [Tooltip("Deceleration in Direction change")]
    [SerializeField] private float changeSideDeceleration;
    [Tooltip("Deceleration when being stopped")]
    [SerializeField] private float deceleration;
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve decelCurve;

    [SerializeField] private float t;


    private Camera _mainCam;
    private float _direction = 0;

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

        //_positionOnSpline = _splineManager.PlayerSpline.Origin;
        //_playerObject.transform.position = _spline.EvaluatePosition(_positionOnSpline);
    }

    #region Misc
    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    #endregion

    public void SwapPhysicsToRB()
    {
        _Airborne = true;
        Debug.Log($"<color=#00FF00>Begin Airborne</color>");
        _LastValidPointPosition = _spline.EvaluatePosition(_positionOnSpline);
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.AddForce(new Vector3(0.0f, (Mathf.Abs(_velocity) * -1) * (_sideJumpImpulseForce * -1), 0.0f), ForceMode.Impulse);
        _tempPostSideLaunchCooldownBeforeGroundCheck = _postSideLaunchCooldownBeforeGroundCheck;
    }

    public void ResumePositionOnSpline()
    {
        _Airborne = false;
        _CanAirAgain = false;
        Debug.Log($"<color=#00FF00>End Airborne</color>");
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _playerObject.transform.position = _spline.EvaluatePosition(0.5f);
        _playerObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _spline.EvaluateUpVector(_lastVelocity));
        _tempPostSideLaunchCooldownAfterGroundCheck = _postSideLaunchCooldownAfterGroundCheck;
    }

    #region Movement

    private void Update()
    {
        if(!_Airborne)
        {
            if(_tempPostSideLaunchCooldownAfterGroundCheck > 0)
            {
                _tempPostSideLaunchCooldownAfterGroundCheck -= Time.deltaTime;
            }
            else
            {
                _CanAirAgain = true;
            }
            UpdateMove();
        }
        else
        {
            CheckForLanding();
        }
    }

    private void CheckForLanding()
    {
        if(_tempPostSideLaunchCooldownBeforeGroundCheck < 0 )
        {
            if (Mathf.Abs(Vector3.Distance(_playerObject.transform.position, _LastValidPointPosition)) < 0.02f)
            {
                ResumePositionOnSpline();
                _tempPostSideLaunchCooldownBeforeGroundCheck = _postSideLaunchCooldownBeforeGroundCheck;
            }
        }
        else
        {
            _tempPostSideLaunchCooldownBeforeGroundCheck -= Time.deltaTime;
        }


    }

    private void UpdateMove()
    {
        float accel = normalAccel;
        if (Mathf.Sign(_velocity) != Mathf.Sign(_direction))
        {
            accel = changeSideDeceleration;
        }
        _velocity = Mathf.Clamp(_velocity + _direction * Time.deltaTime* accelCurve.Evaluate(t) , -_maxSpeed, _maxSpeed);
        t += Time.deltaTime;

        if (_direction == 0f)
        {
            if (_velocity > 0.001f || _velocity < -0.001f)
            {
                accel = deceleration;
                _velocity = Mathf.Clamp(_velocity + decelCurve.Evaluate(t) * -(Mathf.Sign(_velocity)) * Time.deltaTime, -_maxSpeed, _maxSpeed);
                t += Time.deltaTime;
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
            //Debug.Log($"<color=#FF0000>Position on spline: {_positionOnSpline}</color>");

            if (_positionOnSpline > 0.15f && _positionOnSpline < 0.8f)
            {
                if(_CanAirAgain) 
                {
                    _lastVelocity = Mathf.Sign(_velocity);
                    SwapPhysicsToRB();
                }

            }
            else
            {
                _lastVelocity = _positionOnSpline;
            }
        }
        else
        {
            //Standard behaviour
            _positionOnSpline = Mathf.Clamp(_positionOnSpline, 0.0f, 1.0f);
            Debug.Log($"<color=#FF0000>Position on spline: {_positionOnSpline}</color>");

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
        t = 0;
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
        t=0;

        //_PlayerOnSplineController.StartAccelerating(GetNormalisedXDirection(finger.ScreenPosition.x));
    }
}
