using System.Collections;
using System.Collections.Generic;
using TagFrenzy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeSceneManager : Singleton<CubeSceneManager> {

    private UnityEngine.AsyncOperation _asyncOperation;
    private UnityEngine.AsyncOperation _asyncUnloadOperation;

    private void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ShowMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            MultiTag.FindGameObjectsWithTags(Tags.InitialLoadingCanvas).ForEach((element) => { SetMainMenuSlideActive(element, false); });
            MultiTag.FindGameObjectsWithTags(Tags.MainMenuScreenSlide).ForEach((element) =>
            {
                if (element.tags().Contains(Tags.MainMenuScreen.ToString()) == false) SetMainMenuSlideActive(element, false);
            });
            MultiTag.FindGameObjectsWithTags(Tags.MainMenuScreen)
                .ForEach((element) => { SetMainMenuSlideActive(element, true); });
        }
        else
        {
            StartCoroutine(LoadMainMenuAsync());
        }
    }

    public void ShowWorkshopStartpage()
    {
        StartCoroutine(LoadLevelAsync("Workshop"));
    }

    public void ShowWorkshopLevelEditor()
    {
        StartCoroutine(LoadLevelAsync("LevelEditorUser"));
    }

    public void ShowWorkshopLevelBrowser()
    {
        StartCoroutine(LoadLevelAsync("LevelBrowser"));
    }

    public void ShowWorkshopDownloadedLevelBrowser()
    {
        StartCoroutine(LoadLevelAsync("DownloadedLevelBrowser"));
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        _asyncUnloadOperation = SceneManager.UnloadSceneAsync(sceneName);
        while (_asyncUnloadOperation.isDone == false)
        {
            yield return null;
        }
        _asyncUnloadOperation = null; //reset the asyncOperation
    }

    private IEnumerator LoadMainMenuAsync()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") //Unload the active scene (CubeGameplay, Workshop etc.) first, before switching to the main menu
        {
            yield return StartCoroutine(UnloadSceneAsync(SceneManager.GetActiveScene().name));
        }
        PersistentSceneData.MainMenuSceneContainer.SetActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
    }

    public void LoadCubeGameplayLevel(int stageIndex, int levelIndex)
    {
        PersistentSceneData.CurrentStageIndex = stageIndex;
        PersistentSceneData.CurrentLevelIndex = levelIndex;
        StartCoroutine(LoadLevelAsync("CubeGameplay"));
    }

    private IEnumerator LoadLevelAsync(string levelname)
    {
        if (SceneManager.GetActiveScene().name == levelname)
        {
            yield return StartCoroutine(UnloadSceneAsync(SceneManager.GetActiveScene().name));
        }

        _asyncOperation = SceneManager.LoadSceneAsync(levelname, LoadSceneMode.Additive);
        _asyncOperation.allowSceneActivation = false;

        while (_asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            StartCoroutine(UnloadSceneAsync(SceneManager.GetActiveScene().name));
        }
        _asyncOperation.allowSceneActivation = true;
        yield return null; //wait one frame for the scene activation to the take place

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelname));
        PersistentSceneData.MainMenuSceneContainer.SetActive(false);
    }

    public void ShowStageAndLevelSelection()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            MultiTag.FindGameObjectsWithTags(Tags.InitialLoadingCanvas)
                .ForEach((element) => { SetMainMenuSlideActive(element, false); });
            MultiTag.FindGameObjectsWithTags(Tags.MainMenuScreenSlide).ForEach((element) =>
            {
                if (element.tags().Contains(Tags.MainMenuStageAndLevelSelection.ToString()) == false)
                    SetMainMenuSlideActive(element, false);
                ;
            });
            MultiTag.FindGameObjectsWithTags(Tags.MainMenuStageAndLevelSelection)
                .ForEach((element) => { SetMainMenuSlideActive(element, true); });
        }
        else
        {
            StartCoroutine(ShowStageAndLevelSelectionAsync());
        }
    }

    public void ShowStageAndLevelSelectionByIndex(int index)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            MultiTag.FindGameObjectsWithTags("InitialLoadingCanvas")
                .ForEach((element) => { SetMainMenuSlideActive(element, false); });
            MultiTag.FindGameObjectsWithTags("MainMenuScreenSlide").ForEach((element) =>
            {
                if (element.tags().Contains("LevelBrowserPageSelection") == false)
                    SetMainMenuSlideActive(element, false);
                ;
            });
            MultiTag.FindGameObjectsWithTags("LevelBrowserPageSelection")
                .ForEach((element) => { SetMainMenuSlideActive(element, true); });
        }
        else
        {
            StartCoroutine(ShowStageAndLevelSelectionByIndexAsync(index));
        }
    }

    private IEnumerator ShowStageAndLevelSelectionAsync()
    {
        yield return StartCoroutine(LoadMainMenuAsync());
        MultiTag.FindGameObjectsWithTags(Tags.MainMenuStageAndLevelSelectionScrollView)[0].GetComponent<MainMenuStageScrollView>().SlideToIndex(0);
    }

    private IEnumerator ShowStageAndLevelSelectionByIndexAsync(int index)
    {
        yield return StartCoroutine(LoadMainMenuAsync());
        MultiTag.FindGameObjectsWithTags(Tags.MainMenuStageAndLevelSelectionScrollView)[0].GetComponent<MainMenuStageScrollView>().SlideToIndex(index);
    }

    private void SetMainMenuSlideActive(GameObject slide, bool active)
    {
        CanvasGroup canvasGroup = slide.GetComponent<CanvasGroup>();
        canvasGroup.alpha = active == false ? 0 : 1;
        canvasGroup.blocksRaycasts = active;
    }
}
