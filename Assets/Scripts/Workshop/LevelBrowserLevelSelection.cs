using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowserLevelSelection : MonoBehaviour {


    protected int _pageIndex;
    protected List<LevelBrowserLevelTile> _levelTiles;

    [SerializeField] protected RectTransform _rectTransform;
    [SerializeField] protected Text _nameText;
    [SerializeField] protected GameObject _levelTilesContainer;

    public int PageIndex
    {
        get { return _pageIndex; }
    }
    public virtual List<LevelBrowserLevelTile> LevelTiles
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

    public virtual void CreateLevelTiles(List<DataSnapshot> data)
    {
        foreach (var dataSnapshot in data)
        {
            var levelTile = LevelBrowserLevelTile.Create(this, dataSnapshot);
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
