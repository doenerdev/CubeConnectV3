using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowserLevelTile : MonoBehaviour
{

    private LevelBrowserLevelSelection _parentLevelSelection;

    [SerializeField] private GameObject _ratingStarPrefab;
    [SerializeField] private GameObject _noRatingStarPrefab;
    [SerializeField] private Image _tileBackground;
    [SerializeField] private Color _unlockedTileColor;
    [SerializeField] private Color _lockedTileColor;
    [SerializeField] private GameObject _ratingStarsContainer;
    [SerializeField] private Text _levelName;
    [SerializeField] private Text _ratingText;

    private UserGeneratedLevelInfo _levelInfo;
    private string _authorID;
    private string _levelDataURL;

    public string AuthorID
    {
        get { return _authorID; }
    }
    public string LevelDataURL
    {
        get { return _levelDataURL; }
    }
    public UserGeneratedLevelInfo LevelInfo
    {
        get { return _levelInfo; }
    }

    public static LevelBrowserLevelTile Create(LevelBrowserLevelSelection levelSelection, DataSnapshot data)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/LevelBrowserTile")) as GameObject;
        LevelBrowserLevelTile levelTile = go.GetComponent<LevelBrowserLevelTile>();
        levelTile._parentLevelSelection = levelSelection;
        levelTile._levelName.text = data.Child("AuthorName").Value.ToString();
        levelTile._ratingText.text = data.Child("UserRating").Value.ToString();
        levelTile._authorID = data.Child("AuthorID").Value.ToString();
        levelTile._levelDataURL = data.Child("LevelDataURL").Value.ToString();
        levelTile._levelInfo = new UserGeneratedLevelInfo();
        levelTile.PopulateLevelInfo(data);
        return levelTile;
    }

    private void PopulateLevelInfo(DataSnapshot data)
    {
        _levelInfo.AuthorName = data.Child("AuthorName").Value.ToString();
        _levelInfo.AuthorID = data.Child("AuthorID").Value.ToString();
        _levelInfo.LevelID = data.Child("LevelID").Value.ToString();
        _levelInfo.LevelDataURL = data.Child("LevelDataURL").Value.ToString();
        _levelInfo.LevelCode = data.Child("LevelCode").Value.ToString();
        _levelInfo.QtyRatings = int.Parse(data.Child("QtyRatings").Value.ToString());
        _levelInfo.UserRating = float.Parse(data.Child("UserRating").Value.ToString());
        _levelInfo.Difficulty = int.Parse(data.Child("Difficulty").Value.ToString());
        _levelInfo.QtyDownloads = int.Parse(data.Child("QtyDownloads").Value.ToString());
        _levelInfo.Date = int.Parse(data.Child("Date").Value.ToString());
        _levelInfo.FileLocation = data.Child("FileLocation").Value.ToString();
        _levelInfo.LevelName = data.Child("LevelName").Value.ToString();
    }

    public void Clicked()
    {
        LevelBrowser.Instance.ShowDetailPage(this);
    }
}
