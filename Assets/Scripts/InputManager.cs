using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static CW.Common.CwInputManager;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerController _PlayerController;
    [SerializeField] private OnSplineMovementController _PlayerOnSplineController;

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUpdate += UpdateFinger;
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    public void HandleFingerTap(LeanFinger finger)
    {
        _PlayerOnSplineController.Launch(GetNormalisedXDirection(finger.ScreenPosition.x));
    }

    public void HandleFingerUp(LeanFinger finger)
    {
        _PlayerOnSplineController.StopAcceleration();
    }

    public void UpdateFinger(LeanFinger finger)
    {
        if(finger.Index == 0)
        {
            _PlayerOnSplineController.UpdateXDirection(GetNormalisedXDirection(finger.ScreenPosition.x));
        }
    }

    public void HandleFingerDown(LeanFinger finger)
    {
        _PlayerOnSplineController.StartAccelerating(GetNormalisedXDirection(finger.ScreenPosition.x));
    }
}
