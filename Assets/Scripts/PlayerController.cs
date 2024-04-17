using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static CW.Common.CwInputManager;
using static UnityEngine.ParticleSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float SideDashSpeed;
    [SerializeField] private float ShortTapCooldown;
    [SerializeField] private SplineContainer _Spline;

    private bool ShortTapAvailable = false;
    private float TempTapCooldown;

    private Camera m_Cam;
    private float xDirection = 0;
    private Rigidbody _rb;
    private Vector2 InitialPos = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_Cam = Camera.main;
        _rb = PlayerObject.GetComponent<Rigidbody>();
        ShortTapAvailable = true;
        //_Spline = GetComponent<SplineComponent>();
    }

    #region Misc
    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    #endregion

    #region Dash
    public void Dash(float FingerXPos)
    {
        if (ShortTapAvailable)
        {
            if (Mathf.Sign(_rb.velocity.x) != FingerXPos)
            {
                _rb.velocity = Vector3.zero;
                _rb.AddForce(FingerXPos * SideDashSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                _rb.AddForce(FingerXPos * SideDashSpeed * 2 * Time.deltaTime, 0, 0);
            }
            SetShortTapCooldown();
        }
    }

    public void SetShortTapCooldown()
    {
        TempTapCooldown = ShortTapCooldown;
        ShortTapAvailable = false;
    }
    #endregion

    #region Movement-Hold

    private void Update()
    {
        UpdateMovement(xDirection);
    }

    public void UpdateXDirection(float XDirection)
    {
        this.xDirection = XDirection;
    }

    // Update is called once per frame
    void UpdateMovement(float XDirection)
    {
        if(XDirection != 0)
        {
            _rb.AddForce(XDirection * PlayerSpeed * Time.deltaTime, 0.0f, 0.0f);
        }
        
        if (!ShortTapAvailable)
        {
            TempTapCooldown -= Time.deltaTime;
            ShortTapAvailable = TempTapCooldown < 0;
        }
    }
    #endregion

    #region Stop

    public void StopAcceleration()
    {
        xDirection = 0;
    }
    #endregion
}
