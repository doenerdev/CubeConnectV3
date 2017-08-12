using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LevelEditorUser : LevelEditor {

    private static LevelEditorUser _instance;
    private UserGeneratedLevelData _currentlySelectedLevelData;
    private UserGeneratedLevelInfo _currentlySelectedLevelInfo;

    [SerializeField] private LevelEditorEditLevelUser _levelEditorEditLevelUser;
    [SerializeField] private LeveEditorLevelBrowser _levelBrowser;

    public static LevelEditorUser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (LevelEditorUser)FindObjectOfType(typeof(LevelEditorUser));

                if (_instance == null)
                {
                    Debug.LogError("An instance of " + typeof(LevelEditorUser) +
                       " is needed in the scene, but there is none.");
                }
            }

            return _instance;
        }
    }

    private void Awake() //TODO Change back to Awake later
    {
        //StageAndLevelDataManager.Instance.LoadingUserGeneratedLevelDataHolderComplete += new EventHandler<EventTextArgs>(LoadedUserGeneratedLevelDataHolder);
        //StageAndLevelDataManager.Instance.LoadUserGeneratedLevelDataHolderAsync(GameManager.Instance.UserLevelsDataPath);
    }


    public void CreateNewLevel()
    {
        _currentlySelectedLevelInfo = new UserGeneratedLevelInfo();
        _levelEditorEditLevelUser.ShowEditOptions("Unnamed");
    }

    public void RemoveLevel(int stageIndex, int levelIndex, SelectLevelButton button)
    {
       
    }

    public void LevelSelected(string levelCode)
    {
        _currentlySelectedLevelInfo = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfoByLevelcode(levelCode);
        ShowLoadingOverlay();
        _levelBrowser.Canvas.enabled = false;

        UserGeneratedLevelData level = null;
        if (String.IsNullOrEmpty(_currentlySelectedLevelInfo.LevelCode) == false)
        {
            if (_currentlySelectedLevelInfo.Online == true) //if the level is already online, create a copy of it
            {
                StartCoroutine(CopyAndShowCurrentlySelectedCubeLevel());
            }
            else
            {
                LoadAndShowCurrentlySelectedCubeLevel();
            }
        }
        else
        {
            _currentlySelectedLevelInfo.LevelName = "Unnamed";
            if (Application.isEditor == false)
            {
                _currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.DESKTOP_USER_ID;
                _currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.DESKTOP_USER_USERNAME;
                //_currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.Instance.CurrentUserInfo.Username; //TODO CHANGE THIS LATER
                //_currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.Instance.CurrentUserInfo.UserID;
            }
            else
            {
                _currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.DESKTOP_USER_ID;
                _currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.DESKTOP_USER_USERNAME;
            }

            StartCoroutine(CreateAndShowCurrentlySelectedCubeLevel());
        }
    }

    public void LevelEdited(string newLevelName)
    {
        //TODO show loading screen
        ShowLoadingOverlay();
        _currentlySelectedLevelInfo.LevelName = newLevelName;

        UserGeneratedLevelData level = null;
        if (String.IsNullOrEmpty(_currentlySelectedLevelInfo.LevelCode) == false)
        {
            LoadAndShowCurrentlySelectedCubeLevel();
        }
        else
        {
            _currentlySelectedLevelInfo.LevelName = newLevelName;

            if (Application.isEditor == false)
            {
                _currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.DESKTOP_USER_ID;
                _currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.DESKTOP_USER_USERNAME;
                //_currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.Instance.CurrentUserInfo.Username; //TODO CHANGE THIS LATER
                //_currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.Instance.CurrentUserInfo.UserID;
            }
            else
            {
                _currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.DESKTOP_USER_ID;
                _currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.DESKTOP_USER_USERNAME;
            }

            StartCoroutine(CreateAndShowCurrentlySelectedCubeLevel());
        }
    }

    private void LoadAndShowCurrentlySelectedCubeLevel()
    {
        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelAsync(_currentlySelectedLevelInfo.FileLocation, (levelData) =>
        {
            _currentlySelectedLevelData = levelData;
            _levelEditorEditLevelUser.gameObject.SetActive(false);
            HideLoadingOverlay();
            ShowCubeLevel(_currentlySelectedLevelData);
        });
    }

    private IEnumerator CreateAndShowCurrentlySelectedCubeLevel()
    {
        _currentlySelectedLevelData = new UserGeneratedLevelData(4);
        yield return StartCoroutine(GenerateTemporaryLocalLevelcode(_currentlySelectedLevelInfo));
        _currentlySelectedLevelData.SyncWithLevelInfo(_currentlySelectedLevelInfo);
        _levelEditorEditLevelUser.gameObject.SetActive(false);
        StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList().Add(_currentlySelectedLevelInfo.LevelCode, _currentlySelectedLevelInfo);
        HideLoadingOverlay();
        ShowCubeLevel(_currentlySelectedLevelData);
    }

    private IEnumerator CopyAndShowCurrentlySelectedCubeLevel()
    {
        UserGeneratedLevelInfo copyLevelInfo = new UserGeneratedLevelInfo(_currentlySelectedLevelInfo.AuthorName + "_Copy", _currentlySelectedLevelInfo.AuthorID, _currentlySelectedLevelInfo.LevelName + "_Copy");
        yield return StartCoroutine(GenerateTemporaryLocalLevelcode(copyLevelInfo));

        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelAsync(_currentlySelectedLevelInfo.FileLocation, (levelData) =>
        {
            _currentlySelectedLevelInfo = copyLevelInfo;
            _currentlySelectedLevelData = new UserGeneratedLevelData(levelData.GridSize);
            _currentlySelectedLevelData.CubeMap = levelData.CreateCubeMapCopy();
            _currentlySelectedLevelData.SyncWithLevelInfo(copyLevelInfo);
            _levelEditorEditLevelUser.gameObject.SetActive(false);
            StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList().Add(_currentlySelectedLevelInfo.LevelCode, _currentlySelectedLevelInfo);
            HideLoadingOverlay();
            ShowCubeLevel(_currentlySelectedLevelData);
        });
    }

    private IEnumerator GenerateTemporaryLocalLevelcode(UserGeneratedLevelInfo levelInfo)
    {
        bool generatedUID = false;

        do
        {
            string uid = "local";
            uid += Guid.NewGuid().ToString().GetHashCode().ToString("x");

            WWW file = new WWW(GameManager.Instance.UserLevelsDataPath + "/" + uid + ".lvl");
            yield return file;
            if (string.IsNullOrEmpty(file.error) == false)
            {
                levelInfo.LevelCode = uid;
                generatedUID = true;
            }
        } while (generatedUID == false);

    }

    public void ShowLoadingOverlay()
    {
        
    }

    public void HideLoadingOverlay()
    {
        
    }

    public void ShowLevelSelection()
    {
        _levelBrowser.Canvas.enabled = true;
        _levelBrowser.Refresh();
        Dictionary<string, UserGeneratedLevelInfo> levelInfos = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList();

        _levelEditorEditLevelUser.gameObject.SetActive(false);
        _levelDetailControls.SetActive(false);
        HideCubeLevel();
    }

    public override void ShowCubeLevel(LevelData levelData)
    {
        base.ShowCubeLevel(levelData);
    }

    public void SaveAndUploadLevel(string levelName = "Unnamed")
    {
        _currentlySelectedLevelData.CubeMap = _currentLevelData.CubeMap;
        FirebaseManager.Instance.UploadUserGeneratedLevel(_currentlySelectedLevelData, _currentlySelectedLevelInfo);
    }

    public override void SaveLevel()
    {
        _currentlySelectedLevelData.CubeMap = _currentLevelData.CubeMap;
        StageAndLevelDataManager.Instance.SaveUserGeneratedLevel(_currentlySelectedLevelData, _currentlySelectedLevelInfo.LevelCode,
            dataPath =>
            {
                _currentlySelectedLevelInfo.Played = false;
                _currentlySelectedLevelInfo.LocalDate = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                _currentlySelectedLevelInfo.LocalDate = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                _currentlySelectedLevelInfo.FileLocation = dataPath;
                StageAndLevelDataManager.Instance.SaveUserGeneratedLevelInfoHolder();
            });
    }

    public void UploadLevel(UserGeneratedLevelInfo levelInfo)
    {
        FirebaseManager.Instance.UploadUserGeneratedLevel(levelInfo);
    }

    public void BackToWorkshop()
    {
        GameManager.Instance.ShowWorkshop();
    }
}
