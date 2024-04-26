using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas Canva;

    public void DisableCanvas(Scene arg0, Scene arg1)
    {
        Canva.gameObject.SetActive(false);
        Debug.Log("Disabling Canvas");
    }
    public void EnableCanvas(Scene arg0, Scene arg1)
    {
        Canva.gameObject.SetActive(true);
        Debug.Log("Enabling Canvas");
    }
}
