using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorLevelBrowserLevelTile : MonoBehaviour {

    [SerializeField] private Text _levelName;
    [SerializeField] private Text _ratingText;
    [SerializeField] private GameObject _editOptions;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _uploadButton;

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
        levelTile._editOptions.SetActive(false);
        levelTile._editButton.gameObject.SetActive(true);
        levelTile._deleteButton.gameObject.SetActive(true);   

        if (levelInfo.Online == true)
        {
            levelTile._uploadButton.gameObject.SetActive(false);
            levelTile._editButton.onClick.AddListener(() => { LevelEditorUser.Instance.LevelSelected(levelTile._levelInfo.LevelCode); });
            levelTile._deleteButton.onClick.AddListener(() => { });
        }
        else
        {
            levelTile._uploadButton.gameObject.SetActive(true);
            levelTile._editButton.onClick.AddListener(() => { LevelEditorUser.Instance.LevelSelected(levelTile._levelInfo.LevelCode); });
            levelTile._deleteButton.onClick.AddListener(() => { });
            levelTile._uploadButton.onClick.AddListener(() => { LevelEditorUser.Instance.UploadLevel(levelTile._levelInfo); });
        }     

        return levelTile;
    }

    public void Clicked()
    {
        _editOptions.SetActive(true);
    }
}
