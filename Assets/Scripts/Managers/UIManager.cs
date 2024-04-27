using System.ComponentModel;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] Canvas Canva;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HideAllMenus()
    {
        for (int i = 0; i < Canva.transform.childCount; ++i)
        {
            Canva.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public UIManager GetInstance()
    {
        return _instance;
    }
}
