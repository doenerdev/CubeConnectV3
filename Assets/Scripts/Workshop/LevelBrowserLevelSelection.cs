using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowserLevelSelection : MonoBehaviour {


    private int _pageIndex;
    private List<LevelBrowserLevelTile> _levelTiles;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _nameText;
    [SerializeField] private GameObject _levelTilesContainer;

    public int PageIndex
    {
        get { return _pageIndex; }
    }
    public List<LevelBrowserLevelTile> LevelTiles
    {
        get { return _levelTiles; }
    }

    public static LevelBrowserLevelSelection Create(int index, LevelBrowserScrollView scrollView)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/LevelBrowserLevelSelection")) as GameObject;
        LevelBrowserLevelSelection levelSelection = go.GetComponent<LevelBrowserLevelSelection>();
        levelSelection.transform.SetParent(scrollView.ContentContainer, false);
        levelSelection.name = "LevelSelectionPage " + index;
        levelSelection.SetName(index);
        levelSelection.SetStageIndexAndPosition(index);
        levelSelection._levelTiles = new List<LevelBrowserLevelTile>();
  

        return levelSelection;
    }

    public void CreateLevelTiles(List<DataSnapshot> data)
    {
        foreach (var dataSnapshot in data)
        {
            _levelTiles.Add(LevelBrowserLevelTile.Create(this, dataSnapshot));
            _levelTiles[_levelTiles.Count - 1].transform.SetParent(_levelTilesContainer.transform);
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
