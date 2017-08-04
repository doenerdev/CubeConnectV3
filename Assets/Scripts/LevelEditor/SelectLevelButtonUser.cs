using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectLevelButtonUser : MonoBehaviour {

    private int _levelIndex;
    private string _levelName;

    [SerializeField] private Text _levelIndexText;
    [SerializeField] private Text _levelNameText;

    public static SelectLevelButtonUser Create(int levelIndex, string levelName)
    {
        GameObject selectButtonGO = Instantiate(Resources.Load("LevelSelectButtonUser")) as GameObject;
        SelectLevelButtonUser selectLevelButton = selectButtonGO.GetComponent<SelectLevelButtonUser>();
        selectLevelButton.SetLevelName(levelName);
        selectLevelButton._levelIndex = levelIndex;
        return selectLevelButton;
    }

    public void SelectClick()
    {
        LevelEditorUser.Instance.LevelSelected(_levelIndex);
    }

    public void RemoveClicked()
    {
        //LevelEditorProduction.Instance.RemoveLevel(_stageIndex, _levelIndex, this);
    }

    public void SetLevelName(string name)
    {
        _levelName = name;
        _levelNameText.text = _levelName;
    }
}
