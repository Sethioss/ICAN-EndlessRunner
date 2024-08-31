using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] Canvas Canva;
    [SerializeField] List<UIMenuCanva> MenuCanvaList;

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
            MenuCanvaList[i].gameObject.SetActive(false);
        }
    }

    public void ActivateAllMenus()
    {
        for (int i = 0; i < Canva.transform.childCount; ++i)
        {
            MenuCanvaList[i].gameObject.SetActive(true);
        }
    }

    public void ActivateMenu(int index)
    {
        HideAllMenus();
                
        MenuCanvaList[index].gameObject.SetActive(true);
    }

    public UIManager GetInstance()
    {
        return _instance;
    }

    public void SendSceneChangeCommandSpecific(int index)
    {
        GameManager.GetInstance().GetComponent<SceneLoader>().LoadSceneSpecific(index);
    }

    public void SendSceneChangeCommand()
    {
        GameManager.GetInstance().GetComponent<SceneLoader>().LoadScenes();
    }
}
