using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUserLevelSelection : MonoBehaviour {

    private List<SelectLevelButtonUser> _selectLevelButtons;
    private int _stageIndex;

    [SerializeField] private Transform _levelSelectionScrollView;

    public void LoadLevelsIntoScrollView(Dictionary<string, UserGeneratedLevelInfo> levelInfos)
    {
        foreach (Transform child in _levelSelectionScrollView)
        {
            Destroy(child.gameObject);
        }

        _selectLevelButtons = new List<SelectLevelButtonUser>();
        foreach (KeyValuePair<string, UserGeneratedLevelInfo> levelInfo in levelInfos)
        {
            _selectLevelButtons.Add(SelectLevelButtonUser.Create(levelInfo.Value.LevelCode, levelInfo.Value.LevelName));
            _selectLevelButtons[_selectLevelButtons.Count - 1].transform.SetParent(_levelSelectionScrollView);
        }
    }

    public void NewLevelClicked()
    {
        LevelEditorProduction.Instance.CreateNewLevel(_stageIndex);
    }

    public void BackButtonClicked()
    {
        LevelEditorProduction.Instance.ShowStageSelection();
    }
}
