using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public enum ESceneLoadingActionType : uint
{
    NONE = 0,
    ASYNCHRONOUSSINGLE = 1,
    ASYNCHRONOUSADDITIVE = 2,
    SINGLE = 3,
    ADDITIVE = 4
}

[System.Serializable]
public struct SceneLoadingAction
{
    public ESceneLoadingActionType SceneLoadingActionType;
    public string SceneToLoad;
    public bool LoadSceneOnStart;
    public bool ChangeActiveSceneOnLoad;
    public UnityEvent onSceneChanged;
}

public class SceneLoader : MonoBehaviour
{
    public UnityEvent<Scene, LoadSceneMode> onSceneLoadEvent;
    public SceneLoadingAction[] LoadActions;
    private string upcomingSceneName;
    private int upcomingSceneIndex;

    public void Start()
    {
        for (int i = 0; i < LoadActions.Length; ++i)
        {
            if (LoadActions[i].LoadSceneOnStart)
            {
                LoadSceneSpecific(i);
            }
        }
    }

    public void SetAsActiveScene(Scene scene1, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(upcomingSceneName));
        SceneManager.sceneLoaded -= SetAsActiveScene;
    }

    public void AddSceneChangeEventToDelegate(Scene scene1, LoadSceneMode mode)
    {
        LoadActions[upcomingSceneIndex].onSceneChanged?.Invoke();
        SceneManager.sceneLoaded -= AddSceneChangeEventToDelegate;
    }

    public void LoadSceneSpecific(int Index)
    {
        ESceneLoadingActionType CurrentType = LoadActions[Index].SceneLoadingActionType;
        string CurrentSceneName = LoadActions[Index].SceneToLoad;

        

        switch (CurrentType)
        {
            case ESceneLoadingActionType.ASYNCHRONOUSSINGLE:
                SceneManager.LoadSceneAsync(CurrentSceneName, LoadSceneMode.Single);
                break;
            case ESceneLoadingActionType.ASYNCHRONOUSADDITIVE:
                SceneManager.LoadSceneAsync(CurrentSceneName, LoadSceneMode.Additive);
                break;
            case ESceneLoadingActionType.SINGLE:
                SceneManager.LoadScene(CurrentSceneName, LoadSceneMode.Single);
                break;
            case ESceneLoadingActionType.ADDITIVE:
                SceneManager.LoadScene(CurrentSceneName, LoadSceneMode.Additive);
                break;
        }

        if (LoadActions[Index].ChangeActiveSceneOnLoad)
        {
            upcomingSceneName = CurrentSceneName;
            SceneManager.sceneLoaded += SetAsActiveScene;
        }

        upcomingSceneIndex = Index;
        SceneManager.sceneLoaded += AddSceneChangeEventToDelegate;
    }

    public void LoadScenes()
    {
        for (int i = 0; i < LoadActions.Length; ++i)
        {
            ESceneLoadingActionType CurrentType = LoadActions[i].SceneLoadingActionType;
            string CurrentSceneName = LoadActions[i].SceneToLoad;
            switch (CurrentType)
            {
                case ESceneLoadingActionType.ASYNCHRONOUSSINGLE:
                    if (i < LoadActions.Length - 1)
                    {
                        Debug.LogWarning("SCENE_LOADER::WARNING:: An asynchronous loading of single type has been thrown, but other scenes are supposed to be loaded afterwards, and so their loading will be interrupted. Make sure to put only one single loading at the end of the array");
                    }
                    SceneManager.LoadSceneAsync(CurrentSceneName, LoadSceneMode.Single);
                    break;
                case ESceneLoadingActionType.ASYNCHRONOUSADDITIVE:
                    SceneManager.LoadSceneAsync(CurrentSceneName, LoadSceneMode.Additive);
                    break;
                case ESceneLoadingActionType.SINGLE:
                    if (i < LoadActions.Length - 1)
                    {
                        Debug.LogWarning("SCENE_LOADER::WARNING:: A loading of single type has been thrown, but other scenes are supposed to be loaded afterwards, and so their loading will be interrupted. Make sure to put only one single loading at the end of the array");
                    }
                    SceneManager.LoadScene(CurrentSceneName, LoadSceneMode.Single);
                    break;
                case ESceneLoadingActionType.ADDITIVE:
                    SceneManager.LoadScene(CurrentSceneName, LoadSceneMode.Additive);
                    break;
            }

            if (LoadActions[i].ChangeActiveSceneOnLoad)
            {
                switch (CurrentType)
                {
                    case ESceneLoadingActionType.ASYNCHRONOUSADDITIVE:
                        Debug.LogWarning("SCENE_LOADER::WARNING::Can't set an asynchronous Additive scene as active, since it is not loaded yet");
                        LoadActions[i].onSceneChanged?.Invoke();
                        return;
                    case ESceneLoadingActionType.ASYNCHRONOUSSINGLE:
                        Debug.LogWarning("SCENE_LOADER::WARNING::Can't set an asynchronous Single scene as active, since it is not loaded yet");
                        LoadActions[i].onSceneChanged?.Invoke();
                        return;
                }
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(CurrentSceneName));
            }

            LoadActions[i].onSceneChanged?.Invoke();
        }
    }
}
