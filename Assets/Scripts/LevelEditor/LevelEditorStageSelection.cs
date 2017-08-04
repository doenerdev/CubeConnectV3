using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorStageSelection : MonoBehaviour
{

    private List<SelectStageButton> _selectStageButtons;

    [SerializeField] private Transform _stageSelectionScrollView;

    public void LoadStagesIntoScrollView(List<StageData> stages)
    {
        foreach (Transform child in _stageSelectionScrollView)
        {
                Destroy(child.gameObject);
        }

        _selectStageButtons = new List<SelectStageButton>();
        for (int i = 0; i < stages.Count; i++)
        {
            _selectStageButtons.Add(SelectStageButton.Create(i, stages[i].StageName));
            _selectStageButtons[_selectStageButtons.Count -1].transform.SetParent(_stageSelectionScrollView);
        }
    }

    public void NewStageClicked()
    {
        LevelEditorProduction.Instance.CreateNewStage();
    }

}
