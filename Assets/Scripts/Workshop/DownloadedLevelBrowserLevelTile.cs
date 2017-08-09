using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class DownloadedLevelBrowserLevelTile : MonoBehaviour
{

    [SerializeField] private GameObject _levelTileOptions;
    [SerializeField] private Button _deleteLevelButton;
    [SerializeField] private Button _rateLevelButton;
    [SerializeField] private GameObject _ratingStarPrefab;
    [SerializeField] private GameObject _noRatingStarPrefab;
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

    public static DownloadedLevelBrowserLevelTile Create(DownloadedLevelBrowserLevelSelection levelSelection, UserGeneratedLevelInfo levelInfo)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/DownloadedLevelBrowserLevelTile")) as GameObject;
        DownloadedLevelBrowserLevelTile levelTile = go.GetComponent<DownloadedLevelBrowserLevelTile>();
        levelTile._levelName.text = levelInfo.AuthorName;
        levelTile._ratingText.text = levelInfo.UserRating.ToString();
        levelTile._authorID = levelInfo.AuthorID;
        levelTile._levelDataURL = levelInfo.LevelDataURL;
        levelTile._levelInfo = levelInfo;
        return levelTile;
    }

    public void Clicked()
    {
        //LevelBrowser.Instance.ShowDetailPage(this);
    }

    public void RateLevel(int rating = 1)
    {
        Debug.Log("Rate Level:" + _levelInfo.DBNodeKey);
        FirebaseManager.Instance.RateUserGeneratedLevel(_levelInfo, 1);
    }

    public void DeleteLevel()
    {
        //TODO show prompt before deleting
        StageAndLevelDataManager.Instance.RemoveUserGeneratedLevel(_levelInfo.LevelCode);
        DownloadedLevelBrowser.Instance.Refresh();
    }

    public void ShowLevelTileOptions()
    {
        _levelTileOptions.SetActive(true);
    }

    public void HideLevelTileOptions()
    {
        _levelTileOptions.SetActive(false);
    }
}
