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

    [SerializeField] private LevelEditorUserLevelSelection _levelEditorLevelSelection;
    [SerializeField] private LevelEditorEditLevelUser _levelEditorEditLevelUser;

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
        StageAndLevelDataManager.Instance.LoadingUserGeneratedLevelDataHolderComplete += new EventHandler<EventTextArgs>(LoadedUserGeneratedLevelDataHolder);
        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelDataHolderAsync(GameManager.Instance.UserLevelsDataPath);
    }

    private void LoadedUserGeneratedLevelDataHolder(object sender, EventTextArgs args)
    {
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList());
        Debug.Log("Loaded User generated Levels");
        Debug.Log("Qty Levels:" + StageAndLevelDataManager.Instance.GetUserGeneratedLevels().Count);
    }

    public void CreateNewLevel()
    {
        _currentlySelectedLevelInfo = new UserGeneratedLevelInfo();
        _levelEditorLevelSelection.gameObject.SetActive(false);
        _levelEditorEditLevelUser.ShowEditOptions("Unnamed");
    }

    public void RemoveLevel(int stageIndex, int levelIndex, SelectLevelButton button)
    {
       
    }

    public void LevelSelected(string levelCode)
    {
        foreach (var userGeneratedLevelInfo in StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList())
        {
            Debug.Log(userGeneratedLevelInfo.Key);
        }
        Debug.Log("-----");
        Debug.Log(_currentlySelectedLevelInfo);
        Debug.Log(StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList()[levelCode]);
        _currentlySelectedLevelInfo = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfoByLevelcode(levelCode);
        _levelEditorLevelSelection.gameObject.SetActive(false);
        _levelEditorEditLevelUser.ShowEditOptions(_currentlySelectedLevelInfo.LevelName);
    }

    public void LevelEdited(string newLevelName)
    {
        //TODO show loading screen
        ShowLoadingOverlay();

        Dictionary<string, UserGeneratedLevelInfo> levelInfos = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList();
        _currentlySelectedLevelInfo.LevelName = newLevelName;

        UserGeneratedLevelData level = null;
        if (String.IsNullOrEmpty(_currentlySelectedLevelInfo.LevelCode) == false)
        {
            StartCoroutine(LoadAndShowCurrentlySelectedCubeLevel(HideLoadingOverlay));
        }
        else
        {
            _currentlySelectedLevelInfo.LevelName = newLevelName;

            if (Application.isEditor == false)
            {
                _currentlySelectedLevelInfo.AuthorName = FirebaseAuthentication.Instance.CurrentUserInfo.Username;
                _currentlySelectedLevelInfo.AuthorID = FirebaseAuthentication.Instance.CurrentUserInfo.UserID;
            }
            _currentlySelectedLevelInfo.AuthorName = "Namez"; //TODO REMOVE LATER. JUST FOR TESTING

            StartCoroutine(CreateAndShowCurrentlySelectedCubeLevel());
        }
    }

    private IEnumerator LoadAndShowCurrentlySelectedCubeLevel(Action callback)
    {
        Debug.Log(_currentlySelectedLevelInfo.LevelDataURL);
        WWW file = new WWW(_currentlySelectedLevelInfo.LevelDataURL);
        yield return file;
        byte[] fileBytes = file.bytes;
        Task.Run(() =>
        {
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                _currentlySelectedLevelData = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelData>(new BufferedStream(ms));
                ms.Close();
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    _levelEditorEditLevelUser.gameObject.SetActive(false);
                    callback();
                    Debug.Log(_currentlySelectedLevelData.AuthorName);
                    Debug.Log(_currentlySelectedLevelData.LevelDataURL);
                    Debug.Log(_currentlySelectedLevelData.CubeMap);
                    ShowCubeLevel(_currentlySelectedLevelData);
                });

            }
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
        Dictionary<string, UserGeneratedLevelInfo> levelInfos = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList();
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(levelInfos);

        _levelEditorEditLevelUser.gameObject.SetActive(false);
        _levelDetailControls.SetActive(false);
        HideCubeLevel();
        _levelEditorLevelSelection.gameObject.SetActive(true);
    }

    public override void ShowCubeLevel(LevelData levelData)
    {
        _levelEditorLevelSelection.gameObject.SetActive(false);
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
                _currentlySelectedLevelInfo.LevelDataURL = dataPath;
                StageAndLevelDataManager.Instance.SaveUserGeneratedLevelInfoHolder();
            });
    }

    public void UploadLevel(UserGeneratedLevelData levelData)
    {
        //FirebaseManager.Instance.UploadUserGeneratedLevel(levelData, level);
    }
}
