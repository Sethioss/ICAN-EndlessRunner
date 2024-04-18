using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController _PlayerController;

    [SerializeField] private GameObject[] UnderFingerObjects;
    [SerializeField] private GameObject LocationMeanObject;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private LineRenderer SwipeLine;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float SideDashSpeed;
    [SerializeField] private float ShortTapCooldown;
    
    private float TempTapCooldown;
    
    private Camera m_Cam;
    private float DeltaSpeed;
    private Rigidbody _rb;
    private Vector2 InitialPos = Vector2.zero;

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUpdate += UpdateFinger;
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnGesture += HandleGesture;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerTap += HandleFingerTap;
    }
    
    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }
    
    public void Start()
    {
        //m_Cam = Camera.main;
        //_rb = PlayerObject.GetComponent<Rigidbody>();
        //ShortTapAvailable = true;
    }
    
    public void HandleFingerTap(LeanFinger finger)
    {
        _PlayerController.Dash(GetNormalisedXDirection(finger.ScreenPosition.x));
        //if (ShortTapAvailable)
        //{
        //    float Dir = GetNormalisedXDirection(finger.ScreenPosition.x);
        //    if (Mathf.Sign(_rb.velocity.x) != Dir)
        //    {
        //        _rb.velocity = Vector3.zero;
        //        _rb.AddForce(GetNormalisedXDirection(finger.ScreenPosition.x) * SideDashSpeed * Time.deltaTime, 0, 0);
        //    }
        //    _rb.AddForce(GetNormalisedXDirection(finger.ScreenPosition.x) * SideDashSpeed * 2 * Time.deltaTime, 0, 0);
        //    SetShortTapCooldown();
        //}
    }
    
    public void HandleFingerUp(LeanFinger finger)
    {
        _PlayerController.StopAcceleration();
        //DeltaSpeed = 0;
    }
    
    public void UpdateFinger(LeanFinger finger)
    {
        if (finger.Index == 0)
        {
            _PlayerController.UpdateXDirection(GetNormalisedXDirection(finger.ScreenPosition.x));
        }
        //if(finger.Index >= 0)
        //{
        //    UnderFingerObjects[finger.Index % UnderFingerObjects.Length].transform.position = m_Cam.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, 10.0f));
        //}
    
        //if (finger.Index >= 0)
        //{
        //    _rb.AddForce(GetNormalisedXDirection(finger.ScreenPosition.x) * PlayerSpeed * Time.deltaTime, 0.0f, 0.0f);
        //    //_rb.AddForce(1 * PlayerSpeed * Time.deltaTime, 0.0f, 0.0f);
        //}
        //
        //if(!ShortTapAvailable)
        //{
        //    TempTapCooldown -= Time.deltaTime;
        //    ShortTapAvailable = TempTapCooldown < 0;
        //}
    
        //if (PlayerObject.transform.position.x >= m_Cam.ViewportToWorldPoint(new Vector3(2, 0, 0)).x)
        //{
        //    PlayerObject.transform.position = new Vector3(m_Cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x, PlayerObject.transform.position.y,  PlayerObject.transform.position.z);
        //}
    
        //if (finger.Index >= 0)
        //{
        //    DeltaSpeed = finger.ScreenPosition.x - InitialPos.x;
        //    _rb.AddForce(DeltaSpeed * PlayerSpeed * Time.deltaTime, 0, 0);
        //}
    }
    
    public void HandleGesture(List<LeanFinger> Fingers)
    {
        //if (Fingers.Count > 1 && Fingers[1].Index >= 0)
        //{
        //    Vector2 MeanPosition = Vector2.zero;
        //
        //    float XMean = 0.0f;
        //    float YMean = 0.0f;
        //
        //    foreach (LeanFinger finger in Fingers)
        //    {
        //        XMean += finger.ScreenPosition.x;
        //        YMean += finger.ScreenPosition.y;
        //    }
        //
        //    XMean /= Fingers.Count;
        //    YMean /= Fingers.Count;
        //
        //    MeanPosition = new Vector2(XMean, YMean);
        //
        //    LocationMeanObject.transform.position = m_Cam.ScreenToWorldPoint(new Vector3(MeanPosition.x, MeanPosition.y, 10.0f));
        //}       
    }
    
    public void HandleFingerDown(LeanFinger finger)
    {
        _PlayerController.UpdateXDirection(GetNormalisedXDirection(finger.ScreenPosition.x));
        //InitialPos = finger.StartScreenPosition;
    }
}
