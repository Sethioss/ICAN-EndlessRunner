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
    [Header("Side jump")]
    [SerializeField] private bool _SideJumpEnabled = false;
    [HideInInspector] public bool _Airborne = false;
    [SerializeField] private float _sideJumpImpulseForce = 400.0f;
    [SerializeField] private float _sideJumpMaxHeight = 3.0f;

    [Range(0.9985f, 1.0030f)]
    [SerializeField] private float _LandingVelocityBoostMultiplier = 1.0015f;

    private float _cooldownBeforeGroundCheck = 0.2f;
    private float _tempcooldownBeforeGroundCheck = 0.0f;

    private float _cooldownAfterGroundCheck = 0.2f;
    private float _tempCooldownAfterGroundCheck = 0.0f;

    [Tooltip("Deceleration when landing from a side jump")]
    [SerializeField] private float _landingDeceleration = 0.1f;

    [Tooltip("Time taken to go from post jump deceleration to standard deceleration")]
    [SerializeField] private float _postLandingDecelerationTransitionTime = 0.5f;
    private float _tempPostLandingDecelerationTransitionTime = 0.0f;

    [Tooltip("VelocityBasedRotationMultiplier")]
    [SerializeField] private float _AngularRotationMultiplierOnAir = 2.0f;

    private Vector3 _pointPositionBeforeLaunch;
    private float _splinePositionToGoBackTo;
    bool _CanAirAgain = true;

    [SerializeField] private GameObject _playerObject;
    //[SerializeField] private PlayerMoveState _playerMoveState;
    private Rigidbody _rb;

    [SerializeField] private SplineManager _splineManager;
    private SplineContainer _spline;

    //Whether the loop is closed (Loops the player's position on spline between 0 and 1) or not
    [SerializeField] private bool _loopsBack;

    private float _velocity = 0;
    [Header("Controls - Higher values = More drag")]
    [Tooltip("VelocityBasedRotationMultiplier")]
    [SerializeField] private float _AngularRotationMultiplier = 10000.0f;
    [SerializeField] private float _maxSpeed;
    [Tooltip("Acceleration on Start")]
    [SerializeField] private float normalAccel;
    [Tooltip("Deceleration in Direction change")]
    [SerializeField] private float changeSideDeceleration;
    [Tooltip("Deceleration when being stopped")]
    [SerializeField] private float deceleration;
    private float tempDeceleration;
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve decelCurve;

    [SerializeField] private float t;

    private List<float> JumpPointsPosition = new List<float>();
    private float LeftJumpPoint = 0.25f;
    private float RightJumpPoint = 0.75f;


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

        tempDeceleration = deceleration;
        _CanAirAgain = true;

        JumpPointsPosition = _splineManager.GetComponent<SplinePointManager>().GetPointsOfSpecificType(SplinePointPositionType.JUMP_POINTS);
        if(JumpPointsPosition.Count != 2 )
        {
            Debug.LogError($"<color=#FF0000>There are fewer or more than 2 jump points on the spline. Make sure that the SplinePointManager has exactly two points serving as jump points. Using default values...</color>");
        }
        else
        {
            LeftJumpPoint = Mathf.Min(JumpPointsPosition[0], JumpPointsPosition[1]);
            RightJumpPoint = Mathf.Max(JumpPointsPosition[0], JumpPointsPosition[1]);
        }
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
        float _JumpForce = Mathf.Min((Mathf.Abs(_velocity) * -1) * (_sideJumpImpulseForce * -1), _sideJumpMaxHeight);

        _Airborne = true;
        //Debug.Log($"<color=#00FF00>Begin Airborne</color>");
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.AddForce(new Vector3(0.0f, _JumpForce, 0.0f), ForceMode.Impulse);

        if(_JumpForce < 1)
        {
            _tempcooldownBeforeGroundCheck = 0.0f;
        }
        else
        {
            _tempcooldownBeforeGroundCheck = _cooldownBeforeGroundCheck;
        }
    }

    public void ResumePositionOnSpline()
    {
        _Airborne = false;
        _CanAirAgain = false;
        float _landingVelocity = _rb.velocity.y * (_LandingVelocityBoostMultiplier - 1);
        //Debug.Log($"<color=#00FF00>End Airborne</color>");
        _tempCooldownAfterGroundCheck = _cooldownAfterGroundCheck;

        _tempPostLandingDecelerationTransitionTime = _postLandingDecelerationTransitionTime;

        _velocity = _landingVelocity * GetPositionSideBasedOnSplineValue(_positionOnSpline);
        deceleration = _landingDeceleration;

        _rb.useGravity = false;
        _rb.isKinematic = true;

        SetSplinePositionOffset(_splinePositionToGoBackTo);
        _playerObject.transform.position = _spline.EvaluatePosition(_splinePositionToGoBackTo);
        _playerObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _spline.EvaluateUpVector(_splinePositionToGoBackTo));
    }

    private int GetPositionSideBasedOnSplineValue(float SplineValue)
    {
        return _positionOnSpline < 0.5f ? -1 : 1;
    }

    private void RotatePlayerBasedOnVelocity(bool isAirborne)
    {
        Transform playerMeshTransform = _playerObject.GetComponent<Player>().PlayerMesh.gameObject.transform;
        if(isAirborne)
        {
            float zRotation = ((_rb.velocity.y * _AngularRotationMultiplierOnAir) * GetPositionSideBasedOnSplineValue(_positionOnSpline));
            playerMeshTransform.localRotation = Quaternion.Euler(-90.0f, 0, Mathf.Clamp(zRotation, -80f, 80f));
        }
        else
        {
            float zRotation = _velocity * _AngularRotationMultiplier;
            playerMeshTransform.localRotation = Quaternion.Euler(-90.0f, 0, Mathf.Clamp(zRotation, -80f, 80f));
        }
    }

    #region Movement

    private void Update()
    {
        if(_Airborne)
        {
            CheckForLanding();
        }
        else
        {
            //if(_tempPostLandingDecelerationTransitionTime > 0)
            //{
            //    _tempPostLandingDecelerationTransitionTime -= Time.deltaTime;
            //    deceleration = Mathf.Lerp(tempDeceleration, _landingDeceleration, (float)_tempPostLandingDecelerationTransitionTime / (float)_postLandingDecelerationTransitionTime);
            //}
            if(_tempCooldownAfterGroundCheck > 0)
            {
                _tempCooldownAfterGroundCheck -= Time.deltaTime;
            }
            else
            {
                _CanAirAgain = true;
            }
            UpdateMove();
        }
        RotatePlayerBasedOnVelocity(_Airborne);
    }

    private void CheckForLanding()
    {
        if(_tempcooldownBeforeGroundCheck < 0 )
        {
            if (Mathf.Abs(Vector3.Distance(_playerObject.transform.position, _pointPositionBeforeLaunch)) < 0.1f)
            {
                ResumePositionOnSpline();
                _tempcooldownBeforeGroundCheck = _cooldownBeforeGroundCheck;
            }
        }
        else
        {
            _tempcooldownBeforeGroundCheck -= Time.deltaTime;
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
        _playerObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _spline.EvaluateUpVector(_positionOnSpline)) /* _playerObject.transform.rotation*/;
    }
    public void UpdateXDirection(float XDirection)
    {
        _direction = XDirection;
    }

    public void SetSplinePositionOffset(float value)
    {
        _positionOnSpline = value;
    }
    public void AddSplinePositionOffset(float value)
    {
        //Spline is reversed
        _positionOnSpline -= value;

        if (_loopsBack)
        {
            _positionOnSpline = Mathf.Repeat(_positionOnSpline, 1.0f);
            //Debug.Log($"<color=#FF0000>Position on spline: {_positionOnSpline}</color>");

            if(_SideJumpEnabled)
            {
                // > 0.17f = left; < 0.833f = right
                if (_positionOnSpline > LeftJumpPoint && _positionOnSpline < RightJumpPoint)
                {
                    if (_CanAirAgain)
                    {
                        _splinePositionToGoBackTo = _positionOnSpline < 0.5f ? LeftJumpPoint - 0.004f : RightJumpPoint + 0.004f;
                        _pointPositionBeforeLaunch = _spline.EvaluatePosition(_positionOnSpline);
                        SwapPhysicsToRB();
                    }

                }
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
