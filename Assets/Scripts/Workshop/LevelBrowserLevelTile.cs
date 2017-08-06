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

    public static LevelBrowserLevelTile Create(LevelBrowserLevelSelection levelSelection, DataSnapshot data)
    {
        GameObject go = Instantiate(Resources.Load("LevelBrowser/LevelBrowserTile")) as GameObject;
        LevelBrowserLevelTile levelTile = go.GetComponent<LevelBrowserLevelTile>();
        levelTile._parentLevelSelection = levelSelection;
        levelTile._levelName.text = data.Child("AuthorName").Value.ToString();
        levelTile._ratingText.text = data.Child("UserRating").Value.ToString();
        levelTile._authorID = data.Child("AuthorID").Value.ToString();
        levelTile._levelDataURL = data.Child("LevelDataURL").Value.ToString();
        return levelTile;
    }

    public void Clicked()
    {
        LevelBrowser.Instance.ShowDetailPage(this);
    }
}
