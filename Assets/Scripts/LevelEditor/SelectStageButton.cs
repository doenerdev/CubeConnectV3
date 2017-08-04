using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageButton : MonoBehaviour
{
    private static string _prefabReosurcePath = "StageSelectButton";

    private int _stageIndex;
    private string _stageName;

    [SerializeField] private Text _stageIndexText;
    [SerializeField] private Text _stageNameText;

    public static SelectStageButton Create(int stageIndex, string stageName)
    {
        GameObject selectButtonGO = Instantiate(Resources.Load(_prefabReosurcePath)) as GameObject;
        SelectStageButton selectStageButton = selectButtonGO.GetComponent<SelectStageButton>();
        selectStageButton.SetStageIndex(stageIndex);
        selectStageButton.SetStageName(stageName);
        return selectStageButton;
    }

    public void SelectClicked()
    {
        LevelEditorProduction.Instance.StageSelected(_stageIndex);
    }

    public void RemoveClicked()
    {
        LevelEditorProduction.Instance.RemoveStage(_stageIndex, this);
    }

    public void SetStageIndex(int index)
    {
        _stageIndex = index;
        _stageIndexText.text = _stageIndex.ToString();
    }

    public void SetStageName(string name)
    {
        _stageName = name;
        _stageNameText.text = _stageName;
    }
}
