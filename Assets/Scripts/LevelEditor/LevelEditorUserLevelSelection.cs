using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUserLevelSelection : MonoBehaviour {

    private List<SelectLevelButtonUser> _selectLevelButtons;
    private int _stageIndex;

    [SerializeField] private Transform _levelSelectionScrollView;

    public void LoadLevelsIntoScrollView(List<UserGeneratedLevelData> levels)
    {
        foreach (Transform child in _levelSelectionScrollView)
        {
            Destroy(child.gameObject);
        }

        _selectLevelButtons = new List<SelectLevelButtonUser>();
        for (int i = 0; i < levels.Count; i++)
        {
            _selectLevelButtons.Add(SelectLevelButtonUser.Create(i, levels[i].LevelName));
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
