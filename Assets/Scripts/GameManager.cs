using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TagFrenzy;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool Nutzertest = false; //TODO remove later

    private UnityEngine.AsyncOperation _asyncLevelLoader;
    private bool _applicationBootstrappingComplete = false;
    private string _userLevelsDataPath;
    private string _downloadedUserLevelsDataPath;
    private string _userLevelsInfoPath;
    private string _userLevelsInfoFolderPath;
    private string _productionLevelsDataPath;
    private MainMenu _mainMenu;

    [SerializeField] private GameState _gameState;

    public GameState GameState
    {
        get { return _gameState; }
    }
    public string ProductionLevelsDataPath
    {
        get { return _productionLevelsDataPath; }   
    }
    public string UserLevelsDataPath
    {
        get { return _userLevelsDataPath; }
    }
    public string UserLevelsInfoPath
    {
        get { return _userLevelsInfoPath; }
    }
    public string UserLevelsInfoFolderPath
    {
        get { return _userLevelsInfoFolderPath; }
    }
    public string DownloadedUserLevelsDataPath
    {
        get { return _downloadedUserLevelsDataPath; }
    }

    protected void Awake()
    {
        base.Awake();
        if (PersistentSceneData.BootstrappedApplication == true)
            return;

        if (Application.platform == RuntimePlatform.Android)
        {
            _productionLevelsDataPath = "jar:file://" + Application.dataPath + "!/assets/Levels/level.lvl";
            //_userLevelsDataPath = "jar:file://" + Application.dataPath + "!/assets/Levels/UserLevels/Own";
            //_downloadedUserLevelsDataPath = "jar:file://" + Application.dataPath + "!/assets/Levels/UserLevels/Downloaded";
            //_userLevelsInfoPath = "jar:file://" + Application.dataPath + "!/assets/Levels/userLevels.info";
            _userLevelsDataPath = Application.persistentDataPath + "/Levels/UserLevels/Own";
            _downloadedUserLevelsDataPath = Application.persistentDataPath + "/Levels/UserLevels/Downloaded";
            _userLevelsInfoFolderPath = Application.persistentDataPath + "/Levels/";
            _userLevelsInfoPath = _userLevelsInfoFolderPath + "userLevels.info";
        }
        else
        {
            _productionLevelsDataPath = System.IO.Path.Combine("file:///" + Application.streamingAssetsPath, "Levels/level.lvl");
            //_userLevelsDataPath = System.IO.Path.Combine("file:///" + Application.streamingAssetsPath, "Levels/UserLevels/Own");
            //_downloadedUserLevelsDataPath = System.IO.Path.Combine("file:///" + Application.streamingAssetsPath, "Levels/UserLevels/Downloaded");
            //_userLevelsInfoPath = System.IO.Path.Combine("file:///" + Application.streamingAssetsPath, "Levels/userLevels.info");
            _userLevelsDataPath = System.IO.Path.Combine("file:///" + Application.persistentDataPath, "Levels/UserLevels/Own");
            _downloadedUserLevelsDataPath = System.IO.Path.Combine("file:///" + Application.persistentDataPath, "Levels/UserLevels/Downloaded");
            _userLevelsInfoFolderPath = System.IO.Path.Combine("file:///" + Application.persistentDataPath, "Levels/");
            _userLevelsInfoPath = System.IO.Path.Combine(_userLevelsInfoFolderPath, "userLevels.info");
            //_productionLevelsDataPath = Application.dataPath + "/Levels/level.lvl";
            // _userLevelsDataPath = Application.dataPath + "/Levels/userLevels.lvl";
        }
        DontDestroyOnLoad(this);

        switch (_gameState)
        {
            case GameState.InitialLoading:
                BootstrapApplication();
                break;
            case GameState.LevelEditorUser:

                break;
            case GameState.LevelEditor:
                BootstrapLevelEditorProduction();
                break;
        }
    }

    private void LoadedNeccessaryInitialLevelData(object sender, EventTextArgs args)
    {
        if (Nutzertest == false && _applicationBootstrappingComplete == false && _gameState == GameState.InitialLoading) //security check so the stage and level selection will only be created once
        {
            CreateMainMenu();
        }
        else if (_applicationBootstrappingComplete == false && Nutzertest) //TODO remove later
        {
            PersistentSceneData.MainMenuSceneContainer = MultiTag.FindGameObjectsWithTags(Tags.SceneContainer, Tags.MainMenu)[0];
            LoadCubeGameplayLevel(0,0);
        }
    }

    private void CreateMainMenu()
    {
        GameObject mainMenuContainer = Instantiate(Resources.Load("MainMenu/MainMenuCanvas")) as GameObject;
        PersistentSceneData.MainMenuSceneContainer = MultiTag.FindGameObjectsWithTags(Tags.SceneContainer, Tags.MainMenu)[0];
        mainMenuContainer.transform.SetParent(PersistentSceneData.MainMenuSceneContainer.transform, false);
        MainMenu menu = MainMenu.Create(mainMenuContainer);
        menu.CreatingMainMenuComplete += new EventHandler<EventTextArgs>(CreatedStageAndLevelDataSelection);
        PersistentSceneData.MainMenu = menu;
    }

    private void CreatedStageAndLevelDataSelection(object sender, EventTextArgs args)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            _asyncLevelLoader.allowSceneActivation = true;
        }
        ShowMainMenu();
    }

    public void LoadCubeGameplayLevel(int stageIndex, int levelIndex)
    {
        if (_gameState != GameState.CubeGameplay)
        {
            _gameState = GameState.CubeGameplay;
            CubeSceneManager.Instance.LoadCubeGameplayLevel(stageIndex, levelIndex);
        }
        else
        {
            PersistentSceneData.CurrentStageIndex = stageIndex;
            PersistentSceneData.CurrentLevelIndex = levelIndex;
            CubeGameplay.Instance.NextLevel();
        }
    }

    public void LoadWorkshopCubeGameplayLevel(UserGeneratedLevelData levelData)
    {
        _gameState = GameState.WorkshopCubeGameplay;
        PersistentSceneData.CurrentUserGeneratedLevelData = levelData;
        CubeSceneManager.Instance.LoadWorkshopCubeGameplayLevel(levelData);
    }

    public void ShowMainMenu()
    {
        _gameState = GameState.MainMenu;
        CubeSceneManager.Instance.ShowMainMenu();
    }

    public void ShowWorkshop()
    {
        _gameState = GameState.WorkshopStartPage;
        CubeSceneManager.Instance.ShowWorkshopStartpage();
    }

    public void ShowWorkshopDownloadedLevelBrowser()
    {
        _gameState = GameState.WorkshopDownloadedLevelBrowser;
        CubeSceneManager.Instance.ShowWorkshopDownloadedLevelBrowser();
    }

    public void ShowWorkshopLevelBrowser()
    {
        _gameState = GameState.WorkshopLevelBrowser;
        CubeSceneManager.Instance.ShowWorkshopLevelBrowser();
    }

    public void ShowWorkshopPlayLevelBrowser()
    {
        _gameState = GameState.WorkshopPlayLevelBrowser;
        CubeSceneManager.Instance.ShowWorkshopPlayLevelBrowser();
    }

    public void ShowWorkshopLevelEditor()
    {
        _gameState = GameState.LevelEditorUser;
        CubeSceneManager.Instance.ShowWorkshopLevelEditor();
    }

    public void ShowStageAndLevelSelection()
    {
        _gameState = GameState.MainMenuStageAndLevelSelection;
        CubeSceneManager.Instance.ShowStageAndLevelSelection();
    }

    public void ShowStageAndLevelSelectionByIndex(int index)
    {
        _gameState = GameState.MainMenuStageAndLevelSelection;
        CubeSceneManager.Instance.ShowStageAndLevelSelectionByIndex(index);
    }

    private void SetMainMenuSlideActive(GameObject slide, bool active)
    {
        CanvasGroup canvasGroup = slide.GetComponent<CanvasGroup>();
        canvasGroup.alpha = active == false ? 0 : 1;
        canvasGroup.blocksRaycasts = active;
    }

    private void BootstrapApplication()
    {
        Debug.Log("Bootstrap Application");
        //Instantiate all the needed manager and singleton instances
        StageAndLevelDataManager.Create();
        UnityMainThreadDispatcher.Create();
        MobileInputManager.Create();
        CubeSceneManager.Create();
        FirebaseManager.Create();
        FirebaseAuthentication.Create();

        StageAndLevelDataManager.Instance.LoadingNeccessaryInitialLevelDataFromStreamingComplete += new EventHandler<EventTextArgs>(LoadedNeccessaryInitialLevelData);
        StageAndLevelDataManager.Instance.LoadNeccessaryInitialLevelData();

        PersistentSceneData.BootstrappedApplication = true;
    }

    private void BootstrapLevelEditorProduction()
    {
        Debug.Log("Bootstrap LevelEditor Production");
        StageAndLevelDataManager.Create();
        MobileInputManager.Create();
        UnityMainThreadDispatcher.Create();
        Instantiate(Resources.Load("LevelEditorProduction"));
    }
}

public enum GameState
{
    InitialLoading,
    MainMenu,
    MainMenuStageAndLevelSelection,
    LevelMenu,
    CubeGameplay,
    WorkshopCubeGameplay,
    CubePauseMenu,
    LevelEditor,
    LevelEditorUser,
    WorkshopStartPage,
    WorkshopLevelBrowser,
    WorkshopDownloadedLevelBrowser,
    WorkshopPlayLevelBrowser,
}
