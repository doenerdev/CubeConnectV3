using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowserDetailView : MonoBehaviour
{

    [SerializeField] private Text _levelNameText;
    [SerializeField] private Text _authorNameText;
    [SerializeField] private Text _difficultyText;
    [SerializeField] private Text _ratinText;
    [SerializeField] private Text _qtyRatingsText;
    [SerializeField] private Text _qtyDownloadsText;

    private UserGeneratedLevelInfo _levelInfo;

    public static LevelBrowserDetailView Create(UserGeneratedLevelInfo levelInfo)
    {
        GameObject detailViewGO = Instantiate(Resources.Load("LevelBrowser/LevelBrowserDetailView")) as GameObject;
        LevelBrowserDetailView detailView = detailViewGO.GetComponent<LevelBrowserDetailView>();
        detailView._levelNameText.text = levelInfo.LevelName;
        detailView._authorNameText.text = levelInfo.AuthorName;
        detailView._difficultyText.text = levelInfo.Difficulty.ToString();
        detailView._ratinText.text = levelInfo.UserRating.ToString();
        detailView._qtyRatingsText.text = levelInfo.QtyRatings.ToString();
        detailView._qtyDownloadsText.text = levelInfo.QtyDownloads.ToString();
        detailView._levelInfo = levelInfo;

        return detailView;
    }

    public void DownloadClicked()
    {
        Debug.Log("Download Level");
        FirebaseManager.Instance.DownloadUserGeneratedLevel(_levelInfo);
    }

    public void CloseButtonClicked()
    {
        Destroy(this.gameObject);   
    }
}
