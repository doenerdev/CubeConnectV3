using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStageSelection : MonoBehaviour
{

    private int _stageIndex;
    private List<MainMenuLevelTile> _levelTiles;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _nameText;
    [SerializeField] private GameObject _levelTilesContainer;

    public int StageIndex 
    {
        get { return _stageIndex; }
    }
    public List<MainMenuLevelTile> LevelTiles
    {
        get { return _levelTiles; }
    }

    public static MainMenuStageSelection Create(int index, MainMenuStageScrollView scrollView)
    {
        GameObject go = Instantiate(Resources.Load("MainMenu/LevelSelection")) as GameObject;
        MainMenuStageSelection stageSelection = go.GetComponent<MainMenuStageSelection>();
        stageSelection.transform.SetParent(scrollView.ContentContainer, false);
        stageSelection.name = "StageSelection " + index;
        stageSelection.SetName(index);
        stageSelection.SetStageIndexAndPosition(index);
        stageSelection._levelTiles = new List<MainMenuLevelTile>();

        stageSelection.CreateLevelTiles();

        return stageSelection;
    }

    private void CreateLevelTiles()
    {
        StageData stageData = StageAndLevelDataManager.Instance.GetStageDataByIndex(_stageIndex);
        if (stageData != null && stageData.Levels != null)
        {
            for (int i = 0; i < stageData.Levels.Count; i++)
            {
                _levelTiles.Add(MainMenuLevelTile.Create(i, this));
                _levelTiles[i].transform.SetParent(_levelTilesContainer.transform);
            }
        }
    }

    public void SetStageIndexAndPosition(int index)
    {
        _stageIndex = index;
        _rectTransform.anchoredPosition = new Vector2(index * _rectTransform.sizeDelta.x * _rectTransform.localScale.x, 0f);
    }

    public void SetName(int index)
    {
        _nameText.text = "Stage " + (index + 1);
    }

}
