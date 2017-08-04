using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelButton : MonoBehaviour
{
    private int _stageIndex;
    private int _levelIndex;
    private string _levelName;

    [SerializeField] private Text _levelIndexText;
    [SerializeField] private Text _levelNameText;

    public static SelectLevelButton Create(int stageIndex, int levelIndex, string levelName)
    {
        GameObject selectButtonGO = Instantiate(Resources.Load("LevelSelectButton")) as GameObject;
        SelectLevelButton selectLevelButton = selectButtonGO.GetComponent<SelectLevelButton>();
        selectLevelButton.SetLevelIndex(levelIndex);
        selectLevelButton.SetLevelName(levelName);
        selectLevelButton._stageIndex = stageIndex;
        return selectLevelButton;
    }

    public void SelectClick()
    {
        LevelEditorProduction.Instance.LevelSelected(_stageIndex, _levelIndex);
    }

    public void RemoveClicked()
    {
        LevelEditorProduction.Instance.RemoveLevel(_stageIndex, _levelIndex, this);
    }

    public void SetLevelIndex(int index)
    {
        _levelIndex = index;
        _levelIndexText.text = _levelIndex.ToString();
    }

    public void SetLevelName(string name)
    {
        _levelName = name;
        _levelNameText.text = _levelName;
    }
}
