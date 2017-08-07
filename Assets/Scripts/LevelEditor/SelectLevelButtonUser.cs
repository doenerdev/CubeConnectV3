using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectLevelButtonUser : MonoBehaviour {

    private string _levelCode;
    private string _levelName;

    [SerializeField] private Text _levelIndexText;
    [SerializeField] private Text _levelNameText;

    public static SelectLevelButtonUser Create(string levelCode, string levelName)
    {
        GameObject selectButtonGO = Instantiate(Resources.Load("LevelSelectButtonUser")) as GameObject;
        SelectLevelButtonUser selectLevelButton = selectButtonGO.GetComponent<SelectLevelButtonUser>();
        selectLevelButton.SetLevelName(levelName);
        selectLevelButton._levelCode = levelCode;
        return selectLevelButton;
    }

    public void SelectClick()
    {
        LevelEditorUser.Instance.LevelSelected(_levelCode);
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
