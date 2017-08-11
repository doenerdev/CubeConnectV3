using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopPlayLevelBrowserLevelSelection : MonoBehaviour {

    protected int _pageIndex;
    protected List<WorkshopPlayLevelBrowserLevelTile> _levelTiles;

    [SerializeField] protected RectTransform _rectTransform;
    [SerializeField] protected Text _nameText;
    [SerializeField] protected GameObject _levelTilesContainer;

    public int PageIndex
    {
        get { return _pageIndex; }
    }
    public virtual List<WorkshopPlayLevelBrowserLevelTile> LevelTiles
    {
        get { return _levelTiles; }
    }

    public new static WorkshopPlayLevelBrowserLevelSelection Create(int index, WorkshopPlayLevelBrowserScrollView scrollView)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/WorkshopPlayLevelBrowserLevelSelection")) as GameObject;
        WorkshopPlayLevelBrowserLevelSelection levelSelection = go.GetComponent<WorkshopPlayLevelBrowserLevelSelection>();
        levelSelection.transform.SetParent(scrollView.ContentContainer, false);
        levelSelection.name = "LevelSelectionPage " + index;
        levelSelection.SetName(index);
        levelSelection.SetStageIndexAndPosition(index);
        levelSelection._levelTiles = new List<WorkshopPlayLevelBrowserLevelTile>();


        return levelSelection;
    }

    public void CreateLevelTiles(List<UserGeneratedLevelInfo> levelInfos)
    {
        foreach (var levelInfo in levelInfos)
        {
            var levelTile = WorkshopPlayLevelBrowserLevelTile.Create(this, levelInfo);
            _levelTiles.Add(levelTile);
            levelTile.transform.SetParent(_levelTilesContainer.transform);
        }
    }

    public void SetStageIndexAndPosition(int index)
    {
        _pageIndex = index;
        _rectTransform.anchoredPosition = new Vector2(index * _rectTransform.sizeDelta.x * _rectTransform.localScale.x, 0f);
    }

    public void SetName(int index)
    {
        _nameText.text = "Stage " + (index + 1);
    }
}
