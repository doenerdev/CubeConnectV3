using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUser : LevelEditor {

    private static LevelEditorUser _instance;
    private int _currentLevelIndex = -1;

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

    private void Start() //TODO Change back to Awake later
    {
        StageAndLevelDataManager.Instance.LoadingUserGeneratedLevelDataHolderComplete += new EventHandler<EventTextArgs>(LoadedUserGeneratedLevelDataHolder);
        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelDataHolder(GameManager.Instance.UserLevelsDataPath);
    }

    private void LoadedUserGeneratedLevelDataHolder(object sender, EventTextArgs args)
    {
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(StageAndLevelDataManager.Instance.GetUserGeneratedLevels());
        Debug.Log("Loaded User generated Levels");
        Debug.Log("Qty Levels:" + StageAndLevelDataManager.Instance.GetUserGeneratedLevels().Count);
    }

    public void CreateNewLevel()
    {
        int qtyLevels = StageAndLevelDataManager.Instance.GetUserGeneratedLevels().Count;
        _levelEditorLevelSelection.gameObject.SetActive(false);
        _levelEditorEditLevelUser.ShowEditOptions(qtyLevels, "Unnamed", (int)Cube.DefaultGridSize);
    }

    public void RemoveLevel(int stageIndex, int levelIndex, SelectLevelButton button)
    {
        Destroy(button.gameObject);
        StageData stages = StageAndLevelDataManager.Instance.GetStages()[stageIndex];
        stages.Levels.RemoveAt(stageIndex);
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();
    }

    public void LevelSelected(int levelIndex)
    {
        _levelEditorLevelSelection.gameObject.SetActive(false);
        UserGeneratedLevelData level = StageAndLevelDataManager.Instance.GetUserGeneratedLevels()[levelIndex];
        _levelEditorEditLevelUser.ShowEditOptions(levelIndex, level.LevelName, (int)level.GridSize);
    }

    public void LevelEdited(string newLevelName, uint gridSize, int levelIndex)
    {
        List<UserGeneratedLevelData> levels = StageAndLevelDataManager.Instance.GetUserGeneratedLevels();
        UserGeneratedLevelData level = null;

        if (levelIndex >= levels.Count)
        {
            level = new UserGeneratedLevelData(gridSize);
            level.SetLevelName(newLevelName);
            levels.Add(level);
        }
        else
        {
            levels[levelIndex].SetLevelName(newLevelName);

            if(levels[levelIndex].GridSize != gridSize) //Reinitialize the cube map (sets all grid fields to empty grid fields
                levels[levelIndex].SetGridSize(gridSize);
        }

        StageAndLevelDataManager.Instance.SaveUserGeneratedLevelDataHolder();
        _levelEditorEditLevelUser.gameObject.SetActive(false);

        Debug.Log("Levels Count:" + StageAndLevelDataManager.Instance.GetUserGeneratedLevels().Count);

        _currentLevelIndex = levelIndex;
        ShowCubeLevel(levels[levelIndex]);
    }

    public void ShowLevelSelection()
    {
        List<UserGeneratedLevelData> levels = StageAndLevelDataManager.Instance.GetUserGeneratedLevels();
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(levels);
        _currentLevelIndex = -1;

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

    public override void SaveLevel()
    {
        StageAndLevelDataManager.Instance.GetUserGeneratedLevels()[_currentLevelIndex].CubeMap = _currentLevelData.CubeMap;
        StageAndLevelDataManager.Instance.SaveUserGeneratedLevelDataHolder();/*
        UploadLevel(StageAndLevelDataManager.Instance.GetUserGeneratedLevels()[_currentLevelIndex]);
        FirebaseAuthentication.Instance.RequireAuthenticationForm(x => {Debug.Log(x);});*/
    }

    public void UploadLevel(UserGeneratedLevelData levelData)
    {
        FirebaseManager.Instance.UploadUserGeneratedLevel(levelData);
    }
}
