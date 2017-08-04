using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorEditLevel : MonoBehaviour {

    private string _initialLevelName;
    private int _initialIndex;
    private int _stageIndex;

    [SerializeField] private InputField _levelNameInput;
    [SerializeField] private Dropdown _orderNumberDropdown;
    [SerializeField] Slider _gridSizeSlider;
    [SerializeField] Text _gridSizeSliderValueText;

    public void Start()
    {
        if(Cube.DefaultGridSize <= _gridSizeSlider.maxValue && Cube.DefaultGridSize >= _gridSizeSlider.minValue)
            SetSliderValue((int) Cube.DefaultGridSize);
        else

            SetSliderValue((int)_gridSizeSlider.maxValue / 2);

        _gridSizeSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    public void ShowEditOptions(int stageIndex, int levelIndex, string levelName, int gridSize, int levelQty)
    {
        _stageIndex = stageIndex;
        _initialIndex = levelIndex;
        _initialLevelName = levelName;
        _levelNameInput.text = _initialLevelName;

        _orderNumberDropdown.options.Clear();
        for (int i = 0; i < levelQty; i++)
        {
            _orderNumberDropdown.options.Add(new Dropdown.OptionData(i.ToString()));
        }
        _orderNumberDropdown.value = levelIndex;
        _orderNumberDropdown.captionText.text = levelIndex.ToString();

        

        gameObject.SetActive(true);
    }

    public void ConfirmAndLoadLevel()
    {
        LevelEditorProduction.Instance.LevelEdited(_levelNameInput.text, _stageIndex, (uint) _gridSizeSlider.value, _orderNumberDropdown.value, _initialIndex);
    }

    public void Cancel()
    {
        LevelEditorProduction.Instance.ShowLevelSelection(_stageIndex);
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
