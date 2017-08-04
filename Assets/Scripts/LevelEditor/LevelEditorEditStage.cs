using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorEditStage : MonoBehaviour
{
    private string _initialStageName;
    private int _initialIndex;

    [SerializeField] private InputField _stageNameInput;
    [SerializeField] private Dropdown _orderNumberDropdown;

    public void ShowEditOptions(int stageIndex, string stageName, int stageQty)
    {
        _initialIndex = stageIndex;
        _initialStageName = stageName;
        _stageNameInput.text = _initialStageName;

        _orderNumberDropdown.options.Clear();
        for(int i = 0; i < stageQty; i++)
        {
            _orderNumberDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
        }
        _orderNumberDropdown.value = stageIndex;
        _orderNumberDropdown.captionText.text = stageIndex.ToString();

        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        LevelEditorProduction.Instance.ShowStageSelection();
    }

    public void ConfirmAndLoadLevels()
    {
        LevelEditorProduction.Instance.StageEdited(_stageNameInput.text, _orderNumberDropdown.value, _initialIndex);
    }
}
