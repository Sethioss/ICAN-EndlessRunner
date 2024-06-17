using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] private OnSplineMovementController _PlayerOnSplineController;
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
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    public float GetNormalisedXDirection(float FingerXPos)
    {
        return FingerXPos < ((ScreenManager.GetSafeRect().width * Screen.width) / 2.0f) ? -1 : 1;
    }

    public void HandleFingerTap(LeanFinger finger)
    {
       // _PlayerOnSplineController.Launch(GetNormalisedXDirection(finger.ScreenPosition.x));
    }

    public void HandleFingerDebug(List<LeanFinger> Fingers)
    {
        if(Fingers.Count > 2)
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
        if (finger.Index == 0)
        {
            float Dir = GetNormalisedXDirection(finger.ScreenPosition.x);
            if(Dir < 0)
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

        //_PlayerOnSplineController.StartAccelerating(GetNormalisedXDirection(finger.ScreenPosition.x));
    }
}
