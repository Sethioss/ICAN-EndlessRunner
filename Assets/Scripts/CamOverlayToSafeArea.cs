using UnityEngine;

public class CamOverlayToSafeArea : MonoBehaviour
{
    [SerializeField] public ScreenManager _ScrManager;
    [SerializeField] private Camera _Cam;

    // Start is called before the first frame update
    void Awake()
    {
        _Cam.rect = ScreenManager.GetSafeRect();
    }
}
