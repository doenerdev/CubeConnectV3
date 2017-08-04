using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorProduction : LevelEditor
{
    private static LevelEditorProduction _instance;

    [SerializeField] private LevelEditorStageSelection _levelEditorStageSelection;
    [SerializeField] private LevelEditorLevelSelection _levelEditorLevelSelection;
    [SerializeField] private LevelEditorEditStage _levelEditorEditStage;
    [SerializeField] private LevelEditorEditLevel _levelEditorEditLevel;

    public static LevelEditorProduction Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (LevelEditorProduction)FindObjectOfType(typeof(LevelEditorProduction));

                if (_instance == null)
                {
                    Debug.LogError("An instance of " + typeof(LevelEditorProduction) +
                       " is needed in the scene, but there is none.");
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        StageAndLevelDataManager.Instance.LoadingStageAndLevelDataComplete += new EventHandler<EventTextArgs>(LoadedStageAndLevelData);
        Debug.Log(GameManager.Instance.ProductionLevelsDataPath);
        StageAndLevelDataManager.Instance.LoadLevelAndStageDataAsync(GameManager.Instance.ProductionLevelsDataPath);   
    }

    private void LoadedStageAndLevelData(object sender, EventTextArgs args)
    {
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        _levelEditorStageSelection.LoadStagesIntoScrollView(stages);
    }

    public void StageSelected(int stageIndex)
    {
        _levelEditorStageSelection.gameObject.SetActive(false);
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        _levelEditorEditStage.ShowEditOptions(stageIndex, stages[stageIndex].StageName, stages.Count);
    }

    public void CreateNewStage()
    {
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        _levelEditorStageSelection.gameObject.SetActive(false);
        _levelEditorEditStage.ShowEditOptions(stages.Count, "Unnamed", stages.Count + 1);
    }

    public void RemoveStage(int stageIndex, SelectStageButton button)
    {
        Destroy(button.gameObject);
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        stages.RemoveAt(stageIndex);
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();
    }

    public void StageEdited(string newStageName, int newStageIndex, int oldStageIndex)
    {
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        StageData stage = null;

        if (oldStageIndex >= stages.Count)
        {
            stage = new StageData(newStageIndex);
            stage.SetStageName(newStageName);
        }
        else
        {
            stage = stages[oldStageIndex];
            stage.SetStageName(newStageName);
            StageAndLevelDataManager.Instance.RemoveStageAt(oldStageIndex);
            if (newStageIndex > oldStageIndex) newStageIndex--;
        }

        StageAndLevelDataManager.Instance.AddStageAt(newStageIndex, stage);
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();

        _levelEditorEditStage.gameObject.SetActive(false);
        _levelEditorLevelSelection.gameObject.SetActive(true);
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(newStageIndex, StageAndLevelDataManager.Instance.GetStages()[oldStageIndex].Levels);
    }

    public void CreateNewLevel(int stageIndex)
    {
        int qtyLevels = StageAndLevelDataManager.Instance.GetStages()[stageIndex].Levels.Count;
        _levelEditorLevelSelection.gameObject.SetActive(false);
        _levelEditorEditLevel.ShowEditOptions(stageIndex, qtyLevels, "Unnamed", (int)Cube.DefaultGridSize, qtyLevels + 1);
    }

    public void RemoveLevel(int stageIndex, int levelIndex, SelectLevelButton button)
    {
        Destroy(button.gameObject);
        StageData stages = StageAndLevelDataManager.Instance.GetStages()[stageIndex];
        stages.Levels.RemoveAt(stageIndex);
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();
    }

    public void LevelSelected(int stageIndex, int levelIndex)
    {
        _levelEditorLevelSelection.gameObject.SetActive(false);
        List<LevelData> levels = StageAndLevelDataManager.Instance.GetStages()[stageIndex].Levels;
        _levelEditorEditLevel.ShowEditOptions(stageIndex, levelIndex, levels[levelIndex].LevelName, (int)levels[levelIndex].GridSize, levels.Count);
    }

    public void LevelEdited(string newLevelName, int stageIndex, uint gridSize, int newLevelIndex, int oldLevelIndex)
    {
        List<LevelData> levels = StageAndLevelDataManager.Instance.GetStages()[stageIndex].Levels;
        LevelData level = null;

        if (oldLevelIndex >= levels.Count)
        {
            level = new LevelData(gridSize);
            level.SetLevelName(newLevelName);
        }
        else
        {
            level = levels[oldLevelIndex];
            level.SetLevelName(newLevelName);
            StageAndLevelDataManager.Instance.RemoveLevelAt(stageIndex, oldLevelIndex);
            if (newLevelIndex > oldLevelIndex) newLevelIndex--;
        }

        StageAndLevelDataManager.Instance.AddLevelAt(stageIndex, newLevelIndex, level);
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();
        _levelEditorEditLevel.gameObject.SetActive(false);
        ShowCubeLevel(levels[newLevelIndex]);
    }

    public void ShowStageSelection()
    {
        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        _levelEditorStageSelection.LoadStagesIntoScrollView(stages);

        _levelEditorEditStage.gameObject.SetActive(false);
        _levelEditorEditLevel.gameObject.SetActive(false);
        _levelEditorLevelSelection.gameObject.SetActive(false);
        _levelDetailControls.SetActive(false);
        HideCubeLevel();
        _levelEditorStageSelection.gameObject.SetActive(true);
    }

    public void ShowLevelSelection(int stageIndex)
    {
        StageData stage = StageAndLevelDataManager.Instance.GetStages()[stageIndex];
        _levelEditorLevelSelection.LoadLevelsIntoScrollView(stageIndex, stage.Levels);

        _levelEditorEditStage.gameObject.SetActive(false);
        _levelEditorEditLevel.gameObject.SetActive(false);
        _levelEditorStageSelection.gameObject.SetActive(false);
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
        StageAndLevelDataManager.Instance.SaveLevelAndStageData();
    }
   
}

