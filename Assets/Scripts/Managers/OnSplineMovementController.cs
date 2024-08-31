using DG.Tweening;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class OnSplineMovementController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private SplineManager _splineManager;

#if UNITY_EDITOR
    [Header("Visualisation")]
    [SerializeField] bool _previewPlayerStartPos = false;
#endif

    private SplineContainer _spline;

    [SerializeField][Range(0, 1)] private float _startPosition;
    [SerializeField] private GameObject _playerObject;
    private Rigidbody _rb;

    [Header("Side jump")]
    [SerializeField] private float _minimumVelocityForJump = 1.5f;
    [SerializeField] private float _sideJumpImpulseForce = 400.0f;
    [SerializeField] private float MinJumpAmplitude;
    [SerializeField] private float MaxJumpAmplitude;

    [HideInInspector] public bool _Airborne = false;
    [SerializeField] private AnimationCurve _StandardJumpCurve;
    [SerializeField] private JumpCurve[] JumpCurves;
    private float JumpAmplitude;
    private float JumpDuration;
    private float PlayerOgYPosition;
    private Quaternion PlayerOgRotation;
    private float PreviousYPosition;

    [Range(0.9985f, 1.0030f)]
    [SerializeField] private float _LandingVelocityBoostMultiplier = 1.0015f;
    [SerializeField] private float _LandingMaxVelocity = 0.15f;

    [SerializeField][Tooltip("How much time passes before the player can side jump again")] private float _cooldownAfterGroundCheck = 2.0f;
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




    [Header("Controls - Higher values = More drag")]
    [SerializeField] private float _velocity = 0;
    [Tooltip("VelocityBasedRotationMultiplier")]
    [SerializeField] private float _AngularRotationMultiplier = 10000.0f;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed;
    [Tooltip("Acceleration on Start")]
    [SerializeField] private float normalAccel;
    [Tooltip("Deceleration in Direction change")]
    [SerializeField] private float changeSideDeceleration;
    [Tooltip("Deceleration when being stopped")]
    [SerializeField] private float deceleration;
    private float tempDeceleration;

    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve decelCurve;

    private float AccelerationCurveT;
    private float estimatedAirTime = 0;
    private float tempTimeInAir = 0;
    private float _AirJumpPhysicsDelay = 0;
    private bool TimeInAirSet = false;
    [HideInInspector] public float _TimeInAirRatio = 0;

    private Camera _mainCam;
    private float _direction = 0;
    private float _movementLerpValue;

    [Header("Ratios - Used for animation")]
    [SerializeField][Range(-1, 1)] public float _OnGroundVelocityRatio;
    private float _lastDir = 0;

    private float _positionOnSpline = 0;

    [Header("Particles Test")]
    [SerializeField] private ParticleSystem _ParticleRight;
    [SerializeField] private ParticleSystem _ParticleLeft;


    //Used when bounds change
    public void CorrectPlayerBackOnSpline()
    {
        bool IsOutOfBounds = false;
        bool SplinePassesByOrigin = _splineManager.CurrentBoundPassesByOrigin(_splineManager.CurrentBounds);
        if (SplinePassesByOrigin)
        {
            IsOutOfBounds = _positionOnSpline >= _splineManager.CurrentBounds.Positions[0].Position && _positionOnSpline <= _splineManager.CurrentBounds.Positions[1].Position;
            if (IsOutOfBounds)
            {
                if (Mathf.Abs(_positionOnSpline - _splineManager.CurrentBounds.Positions[0].Position) > Mathf.Abs(_positionOnSpline - _splineManager.CurrentBounds.Positions[1].Position))
                {
                    SetSplinePositionOffset(_splineManager.CurrentBounds.Positions[0].Position);
                }
                else
                {
                    SetSplinePositionOffset(_splineManager.CurrentBounds.Positions[1].Position);
                }
            }
        }
        else
        {
            IsOutOfBounds = !(_positionOnSpline >= _splineManager.CurrentBounds.Positions[0].Position && _positionOnSpline <= _splineManager.CurrentBounds.Positions[1].Position);
            if (IsOutOfBounds)
            {
                if (Mathf.Abs(_positionOnSpline - _splineManager.CurrentBounds.Positions[0].Position) > Mathf.Abs(_positionOnSpline - _splineManager.CurrentBounds.Positions[1].Position))
                {
                    SetSplinePositionOffset(_splineManager.CurrentBounds.Positions[1].Position);
                }
                else
                {
                    SetSplinePositionOffset(_splineManager.CurrentBounds.Positions[0].Position);
                }
            }
        }
    }

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
        tempDeceleration = deceleration;
        _CanAirAgain = true;
    }

    private void Update()
    {
        if (_Airborne)
        {
            CheckForLanding();
        }
        else
        {
            if(_tempPostLandingDecelerationTransitionTime > 0)
            {
                _tempPostLandingDecelerationTransitionTime -= Time.deltaTime;
                deceleration = Mathf.Lerp(tempDeceleration, _landingDeceleration, (float)_tempPostLandingDecelerationTransitionTime / (float)_postLandingDecelerationTransitionTime);
            }
            if(_tempPostLandingDecelerationTransitionTime > 0)
            {
                _tempPostLandingDecelerationTransitionTime = Time.deltaTime;
            }

            if (_tempCooldownAfterGroundCheck > 0)
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

    #region Misc
    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    private int GetPositionSideBasedOnSplineValue(float SplineValue)
    {
        return _positionOnSpline < 0.5f ? -1 : 1;
    }

    #endregion

    #region Side Jump
    private void CheckForLanding()
    {
        if (!TimeInAirSet)
        {
            _AirJumpPhysicsDelay += Time.deltaTime;

            //Debug.Log("<color=00FFFF>_AirJumpPhysicsDelay: </color>" + _AirJumpPhysicsDelay);
            JumpDuration = Mathf.Max(0, ((2 * JumpAmplitude) / Physics.gravity.magnitude) - _AirJumpPhysicsDelay);
            //Debug.Log($"Total time before fall: {JumpDuration}");
            PlayerOgYPosition = _playerObject.transform.position.y;
            PlayerOgRotation = _playerObject.transform.rotation;
            //Debug.Log($"Time player will spend in air: {Mathf.Max(0, AirTime - _AirJumpPhysicsDelay)}");
            _AirJumpPhysicsDelay = 0;
            TimeInAirSet = true;
            estimatedAirTime = JumpDuration;

            tempTimeInAir = estimatedAirTime;

        }
        else
        {
            PreviousYPosition = _playerObject.transform.position.y;
            if (tempTimeInAir < 0)
            {
                tempTimeInAir = 0.0f;
                TimeInAirSet = false;
                ResumePositionOnSpline();
            }
            else
            {
                tempTimeInAir -= Time.deltaTime;
                _TimeInAirRatio = Mathf.Clamp(1 - (tempTimeInAir / JumpDuration), 0.0f, 1.0f);
                _playerObject.transform.position = new Vector3(_playerObject.transform.position.x, PlayerOgYPosition + (_StandardJumpCurve.Evaluate(_TimeInAirRatio)) * JumpAmplitude, _playerObject.transform.position.z);
                GameObject _playerMeshObject = _playerObject.transform.GetChild(0).gameObject;
            }
        }
    }

    private AnimationCurve SetJumpCurve(float inAbsoluteVelocity)
    {
        if(JumpCurves.Length < 1)
        {
            return _StandardJumpCurve;
        }

        AnimationCurve SelectedCurve = _StandardJumpCurve;
        float HigherMinVelocityValue = _minimumVelocityForJump;
        for(int i = 0; i < JumpCurves.Length; i++)
        {
            if (JumpCurves[i].MinimumRequiredVelocity < inAbsoluteVelocity)
            {
                if (JumpCurves[i].MinimumRequiredVelocity >= HigherMinVelocityValue)
                {
                    SelectedCurve = JumpCurves[i].Curve;
                    HigherMinVelocityValue = JumpCurves[i].MinimumRequiredVelocity;
                    ApplyCurveParameters(JumpCurves[i]);
                }

            }
        }
        return SelectedCurve;
    }

    private void ApplyCurveParameters(JumpCurve InCurve)
    {
        _sideJumpImpulseForce = InCurve.SideJumpImpulseForce;
        MinJumpAmplitude = InCurve.MinJumpHeight;
        MaxJumpAmplitude = InCurve.MaxJumpHeight;
    }

    public void ResumePositionOnSpline()
    {
        _Airborne = false;
        _CanAirAgain = false;

        float _landingVelocity = Mathf.Min(_LandingMaxVelocity, JumpAmplitude * (_LandingVelocityBoostMultiplier - 1));

        _tempCooldownAfterGroundCheck = _cooldownAfterGroundCheck;
        _tempPostLandingDecelerationTransitionTime = _postLandingDecelerationTransitionTime;

        _velocity = _landingVelocity * GetPositionSideBasedOnSplineValue(_positionOnSpline);

        deceleration = _landingDeceleration;

        _rb.useGravity = false;
        _rb.isKinematic = true;

        SetSplinePositionOffset(_splinePositionToGoBackTo);
        _playerObject.transform.position = _spline.EvaluatePosition(_splinePositionToGoBackTo);
    }
    #endregion

    #region Rotation
    private void RotatePlayerBasedOnVelocity(bool isAirborne)
    {
        Transform playerMeshTransform = _playerObject.GetComponent<Player>().PlayerMesh.gameObject.transform;
        if (isAirborne)
        {
            float zRotation = 1 - (((_rb.velocity.y * _AngularRotationMultiplierOnAir) * GetPositionSideBasedOnSplineValue(_positionOnSpline)));
            playerMeshTransform.localRotation = Quaternion.Euler(-90.0f, 0, Mathf.Clamp(zRotation, -70f, 70f));
        }
        else
        {
            float zRotation = _velocity * _AngularRotationMultiplier;
            playerMeshTransform.localRotation = Quaternion.Euler(-90.0f, 0, Mathf.Clamp(zRotation, -70f, 70f));
        }
    }
    #endregion

    #region Movement

    private void UpdateMove()
    {
        float accel = normalAccel;
        if (Mathf.Sign(_velocity) != Mathf.Sign(_direction))
        {
            accel = changeSideDeceleration;
        }

        _velocity = Mathf.Clamp(_velocity + _direction * Time.deltaTime * accelCurve.Evaluate(_movementLerpValue), -_maxSpeed, _maxSpeed);
        _movementLerpValue += Time.deltaTime;

        if (_direction == 0f)
        {
            if (_velocity > 0.00001f || _velocity < -0.00001f)
            {
                accel = deceleration;
                float _calculatedVelocity = _velocity + decelCurve.Evaluate(_movementLerpValue) * -(Mathf.Sign(_velocity)) * Time.deltaTime;
                _velocity = Mathf.Clamp(_calculatedVelocity, -_maxSpeed, _maxSpeed);

                //Never fully stop after having pressed a direction at least once
                if (_velocity <= _minSpeed && _velocity >= -_minSpeed && _velocity != 0.0f)
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
        if (_lastDir == 0)
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

    public void SetSplinePositionOffset(float value)
    {
        _positionOnSpline = value;
    }
    public void AddSplinePositionOffset(float value)
    {
        //Spline is reversed
        float _finalPosOnSpline = _positionOnSpline - value;

        float[] Positions = _splineManager.GetCurrentBoundsPositions();
        bool ValidPosition = false;

        //Bound points don't go from 0.01 to 0.99 to avoid spline origin related issues
        if (!(Mathf.Abs(Positions[0] - Positions[1]) >= 0.98f) || (Mathf.Abs(Positions[0] - Positions[1]) == 0.0f))
        {
            bool SplinePassesByOrigin = _splineManager.CurrentBoundPassesByOrigin(_splineManager.CurrentBounds);
            if (SplinePassesByOrigin)
            {
                ValidPosition = _finalPosOnSpline >= Positions[0] || _finalPosOnSpline <= Positions[1];
            }
            else
            {
                ValidPosition = _finalPosOnSpline >= Positions[0] && _finalPosOnSpline <= Positions[1];
            }

            if (ValidPosition)
            {
                _positionOnSpline = Mathf.Repeat(_finalPosOnSpline, 1.0f);
            }
            else
            {
                if (SplinePassesByOrigin)
                {
                    //TODO: Add logic for jump points passing by the origin (Should never happen so does nothing for now)
                }
                else
                {
                    if (_CanAirAgain)
                    {
                        if (_positionOnSpline < 0.5f && _splineManager.CurrentBounds.Positions[0].PositionPointType == SplinePointPositionType.JUMP_POINT)
                        {
                            if(_velocity > _minimumVelocityForJump)
                            {
                                _StandardJumpCurve = SetJumpCurve(Mathf.Abs(_velocity));
                                JumpAmplitude = Mathf.Clamp((Mathf.Abs(_velocity) * -1) * (_sideJumpImpulseForce * -1), MinJumpAmplitude, MaxJumpAmplitude);
                                _velocity = 0.0f;
                                _Airborne = true;
                                _splinePositionToGoBackTo = _splineManager.CurrentBounds.Positions[0].Position + 0.007f;
                            }
                        }
                        if (_positionOnSpline >= 0.5f && _splineManager.CurrentBounds.Positions[1].PositionPointType == SplinePointPositionType.JUMP_POINT)
                        {
                            if (-_velocity > _minimumVelocityForJump)
                            {
                                _StandardJumpCurve = SetJumpCurve(Mathf.Abs(_velocity));
                                JumpAmplitude = Mathf.Clamp((Mathf.Abs(_velocity) * -1) * (_sideJumpImpulseForce * -1), MinJumpAmplitude, MaxJumpAmplitude);
                                _velocity = 0.0f;
                                _Airborne = true;
                                _splinePositionToGoBackTo = _splineManager.CurrentBounds.Positions[1].Position - 0.007f;
                            }

                        }
                    }
                }

                _velocity = 0;
            }
        }
        else
        {
            _positionOnSpline = Mathf.Repeat(_finalPosOnSpline, 1.0f);
        }

        //HandleJumpingLogic();

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
            if (_previewPlayerStartPos)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(_splineManager.GetComponent<SplinePointManager>().PlayerSpline.EvaluatePosition(_startPosition), new Vector3(0.8f, 0.8f, 0.8f));
            }
        }
#endif
}