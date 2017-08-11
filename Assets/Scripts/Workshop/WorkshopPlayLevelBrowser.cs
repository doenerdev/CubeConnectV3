using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopPlayLevelBrowser : Singleton<WorkshopPlayLevelBrowser> {
    public const int QTY_LEVELS_PER_PAGE = 2;
    public LevelBrowserPlaySortCategory _currentSortCategory;
    public LevelBrowserSortType _currentSortType;


    [SerializeField] private Transform _parentCanvas;
    [SerializeField] private WorkshopPlayLevelBrowserScrollView _levelBrowserScrollView;
    [SerializeField] private WorkshopPlayLevelBrowserPageSelection _levelBrowserPageSelection;
    [SerializeField] private Dropdown _sortCategoryDropdown;

    public Text errorText;

    public LevelBrowserPlaySortCategory CurrentSortCategory
    {
        get { return _currentSortCategory; }
    }
    public LevelBrowserSortType CurrentSortType
    {
        get { return _currentSortType; }
    }

    private void Start()
    {
        _levelBrowserPageSelection.Initialize(_parentCanvas);


        _sortCategoryDropdown.options.Clear();
        int enumCounter = 0;
        foreach (LevelBrowserPlaySortCategory sortCategory in Enum.GetValues(typeof(LevelBrowserPlaySortCategory)))
        {
            _sortCategoryDropdown.options.Add(new Dropdown.OptionData(Enum.GetName(typeof(LevelBrowserPlaySortCategory), enumCounter)));
            enumCounter++;
        }
        _sortCategoryDropdown.onValueChanged.AddListener(SortCategoryDropdownChanged);

        GatherLevelInfos();
    }

    public void GatherLevelInfos()
    {
        Debug.Log("Gathering Infos...");
        StartCoroutine(GatherLevelInfosAsync());
    }

    private IEnumerator GatherLevelInfosAsync()
    {
        errorText.text = "Gathering Infos...";
        List<KeyValuePair<string, UserGeneratedLevelInfo>> levelInfos = GetSortedLevelInfos();
        yield return null;

        errorText.text = "Count: " + levelInfos.Count;

        for (int i = 0; i < levelInfos.Count; i += QTY_LEVELS_PER_PAGE)
        {
            _levelBrowserPageSelection.AddPage();
            _levelBrowserPageSelection.Pages[_levelBrowserPageSelection.Pages.Count - 1].CreateLevelTiles(levelInfos.GetRange(i, Math.Min(QTY_LEVELS_PER_PAGE, levelInfos.Count - i)).Select(x => x.Value).ToList());
            errorText.text = "Added Page " + i;
            yield return null;
        }
    }

    private List<KeyValuePair<string, UserGeneratedLevelInfo>> GetSortedLevelInfos()
    {
        Debug.Log("Gettings Sorted Infos...");
        Dictionary<string, UserGeneratedLevelInfo> ownlevels = StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList();
        Dictionary<string, UserGeneratedLevelInfo> downloadedLevels = StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList();

        Debug.Log("Own:" + ownlevels);
        Debug.Log("Own Count:" + ownlevels.Count);
        Debug.Log("Downlaoded:" + downloadedLevels);
        Debug.Log("Downlaoded Count:" + downloadedLevels.Count);
        Debug.Log("Overall Count:" + StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList().Count);

        switch (_currentSortCategory)
        {
            case LevelBrowserPlaySortCategory.AuthorName:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                    return ownlevels.OrderBy(d => d.Value.AuthorName).ToList();
                }
                downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                return ownlevels.OrderByDescending(d => d.Value.AuthorName).ToList();
            case LevelBrowserPlaySortCategory.Date:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                    return ownlevels.OrderBy(d => d.Value.Date).ToList();
                }
                downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                return ownlevels.OrderByDescending(d => d.Value.Date).ToList();
            case LevelBrowserPlaySortCategory.LevelName:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                    return ownlevels.OrderBy(d => d.Value.LevelName).ToList();
                }
                downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                return ownlevels.OrderByDescending(d => d.Value.LevelName).ToList();
            case LevelBrowserPlaySortCategory.OwnDownloaded:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                    return ownlevels.ToList();
                }
                ownlevels.ToList().ForEach(x => downloadedLevels[x.Key] = x.Value);
                return downloadedLevels.ToList();
            case LevelBrowserPlaySortCategory.Played:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                    return ownlevels.OrderBy(d => d.Value.Played).ToList();
                }
                downloadedLevels.ToList().ForEach(x => ownlevels[x.Key] = x.Value);
                return ownlevels.OrderByDescending(d => d.Value.Played).ToList();
        }
        return null;
    }

    public void Refresh()
    {
        _levelBrowserPageSelection.RemoveAllPages();
        GatherLevelInfos();
    }

    public void SortCategoryDropdownChanged(int value)
    {
        LevelBrowserPlaySortCategory newSortCategory = (LevelBrowserPlaySortCategory)value;
        if (_currentSortCategory == newSortCategory)
        {
            _currentSortType = _currentSortType == LevelBrowserSortType.Ascending ? LevelBrowserSortType.Descending : LevelBrowserSortType.Ascending;
        }
        else
        {
            _currentSortCategory = newSortCategory;
            _currentSortType = LevelBrowserSortType.Ascending;
        }

        Refresh();
    }

    public void BackToWorkshop()
    {
        GameManager.Instance.ShowWorkshop();
    }

    public void SetSortType(LevelBrowserSortType sortType)
    {
        _currentSortType = sortType;
    }

    public void SetSortCategory(LevelBrowserPlaySortCategory sortCategory)
    {
        _currentSortCategory = sortCategory;
    }

    public void PlayLevel(WorkshopPlayLevelBrowserLevelTile tile)
    {
        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelAsync(tile.LevelInfo.FileLocation, (levelData) =>
        {
            GameManager.Instance.LoadWorkshopCubeGameplayLevel(levelData);
        });
    }
}


public enum LevelBrowserPlaySortCategory
{
    AuthorName,
    LevelName,
    Played,
    Date,
    OwnDownloaded,
}//Datum, OwnDownloaded, Gespielt