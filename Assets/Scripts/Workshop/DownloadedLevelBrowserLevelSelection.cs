using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class DownloadedLevelBrowserLevelSelection : MonoBehaviour {

    protected int _pageIndex;
    protected List<DownloadedLevelBrowserLevelTile> _levelTiles;

    [SerializeField]
    protected RectTransform _rectTransform;
    [SerializeField]
    protected Text _nameText;
    [SerializeField]
    protected GameObject _levelTilesContainer;

    public int PageIndex
    {
        get { return _pageIndex; }
    }
    public virtual List<DownloadedLevelBrowserLevelTile> LevelTiles
    {
        get { return _levelTiles; }
    }

    public new static DownloadedLevelBrowserLevelSelection Create(int index, DownloadedLevelBrowserScrollView scrollView)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/DownloadedLevelBrowserLevelSelection")) as GameObject;
        DownloadedLevelBrowserLevelSelection levelSelection = go.GetComponent<DownloadedLevelBrowserLevelSelection>();
        levelSelection.transform.SetParent(scrollView.ContentContainer, false);
        levelSelection.name = "LevelSelectionPage " + index;
        levelSelection.SetName(index);
        levelSelection.SetStageIndexAndPosition(index);
        levelSelection._levelTiles = new List<DownloadedLevelBrowserLevelTile>();


        return levelSelection;
    }

    public void CreateLevelTiles(List<UserGeneratedLevelInfo> levelInfos)
    {
        foreach (var levelInfo in levelInfos)
        {
            var levelTile = DownloadedLevelBrowserLevelTile.Create(this, levelInfo);
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
