using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Environments;
using UnityEngine;

public class LevelEditorLevelSelection : MonoBehaviour {

    private List<SelectLevelButton> _selectLevelButtons;
    private int _stageIndex;

    [SerializeField] private Transform _levelSelectionScrollView;

    public void LoadLevelsIntoScrollView(int stageIndex, List<LevelData> levels)
    {
        _stageIndex = stageIndex;

        foreach (Transform child in _levelSelectionScrollView)
        {
            Destroy(child.gameObject);
        }

        _selectLevelButtons = new List<SelectLevelButton>();
        for (int i = 0; i < levels.Count; i++)
        {
            _selectLevelButtons.Add(SelectLevelButton.Create(stageIndex, i, levels[i].LevelName));
            _selectLevelButtons[_selectLevelButtons.Count - 1].transform.SetParent(_levelSelectionScrollView);
        }
    }

    public void NewLevelClicked()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.CreateNewLevel(_stageIndex);
                break;
            case GameState.LevelEditorUser:
                LevelEditorUser.Instance.CreateNewLevel();
                break;
        }
    }

    public void BackButtonClicked()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.ShowStageSelection();
                break;
            case GameState.LevelEditorUser:
                GameManager.Instance.ShowWorkshop();
                break;
        }
    }
}
