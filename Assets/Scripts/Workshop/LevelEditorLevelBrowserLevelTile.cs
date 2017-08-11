using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorLevelBrowserLevelTile : MonoBehaviour {

    [SerializeField] private GameObject _ratingStarPrefab;
    [SerializeField] private GameObject _noRatingStarPrefab;
    [SerializeField] private GameObject _ratingStarsContainer;
    [SerializeField] private GameObject _levelDownloadedIndicator;
    [SerializeField] private GameObject _levelPlayedIndicator;
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

    public static LevelEditorLevelBrowserLevelTile Create(LevelEditorLevelBrowserLevelSelection levelSelection, UserGeneratedLevelInfo levelInfo)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/LevelEditorLevelBrowserLevelTile")) as GameObject;
        LevelEditorLevelBrowserLevelTile levelTile = go.GetComponent<LevelEditorLevelBrowserLevelTile>();
        Debug.Log(levelTile);
        Debug.Log(levelInfo);
        levelTile._levelName.text = levelInfo.AuthorName;
        levelTile._ratingText.text = levelInfo.UserRating.ToString();
        levelTile._authorID = levelInfo.AuthorID;
        levelTile._levelDataURL = levelInfo.LevelDataURL;
        levelTile._levelInfo = levelInfo;

        if (levelInfo.Online == true && levelInfo.AuthorID != FirebaseAuthentication.Instance.CurrentUserInfo.UserID)
        {
            levelTile._levelDownloadedIndicator.SetActive(true);
        }
        if (levelInfo.Played == true)
        {
            levelTile._levelPlayedIndicator.SetActive(true);
        }

        return levelTile;
    }

    public void Clicked()
    {
        
    }
}
