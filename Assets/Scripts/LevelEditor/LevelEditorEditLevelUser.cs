using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorEditLevelUser : MonoBehaviour {

    private string _initialLevelName;

    [SerializeField] private InputField _levelNameInput;
    [SerializeField] Slider _gridSizeSlider;
    [SerializeField] Text _gridSizeSliderValueText;

    public void Start()
    {
        if (Cube.DefaultGridSize <= _gridSizeSlider.maxValue && Cube.DefaultGridSize >= _gridSizeSlider.minValue)
            SetSliderValue((int)Cube.DefaultGridSize);
        else

            SetSliderValue((int)_gridSizeSlider.maxValue / 2);

        _gridSizeSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    public void ShowEditOptions(string levelName)
    {
        _initialLevelName = levelName;
        _levelNameInput.text = _initialLevelName;

        gameObject.SetActive(true);
    }

    public void ConfirmAndLoadLevel()
    {
        LevelEditorUser.Instance.LevelEdited(_levelNameInput.text);
    }

    public void Cancel()
    {
        LevelEditorUser.Instance.ShowLevelSelection();
    }

    private void SetSliderValue(int value)
    {
        if (value >= _gridSizeSlider.minValue && value <= _gridSizeSlider.maxValue)
        {
            _gridSizeSlider.value = value;
            _gridSizeSliderValueText.text = _gridSizeSlider.value.ToString();
        }
    }

    public void OnSliderValueChanged()
    {
        _gridSizeSliderValueText.text = _gridSizeSlider.value.ToString();
        //LevelEditor.Instance.SetGridSize(_stageIndex, _initialIndex, (uint)_gridSizeSlider.value);
    }
}
