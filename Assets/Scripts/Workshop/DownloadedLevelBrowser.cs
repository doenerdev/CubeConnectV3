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

public class DownloadedLevelBrowser : Singleton<DownloadedLevelBrowser> {

    public const int QTY_LEVELS_PER_PAGE = 2;
    public const string LAST_ALPHABETICAL_STRING = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
    public LevelBrowserSortCategory _currentSortCategory;
    public LevelBrowserSortType _currentSortType;


    [SerializeField] private Transform _parentCanvas;
    [SerializeField] private LevelBrowserScrollView _levelBrowserScrollView;
    [SerializeField] private DownloadedLevelBrowserPageSelection _levelBrowserPageSelection;
    [SerializeField] private Dropdown _sortCategoryDropdown;

    public Text errorText;

    public LevelBrowserSortCategory CurrentSortCategory
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
        foreach (LevelBrowserSortCategory sortCategory in Enum.GetValues(typeof(LevelBrowserSortCategory)))
        {
            _sortCategoryDropdown.options.Add(new Dropdown.OptionData(Enum.GetName(typeof(LevelBrowserSortCategory), enumCounter)));
            enumCounter++;
        }
        _sortCategoryDropdown.onValueChanged.AddListener(SortCategoryDropdownChanged);

        GatherLevelInfos();
    }

    public void GatherLevelInfos()
    {
        StartCoroutine(GatherLevelInfosAsync());
    }

    private IEnumerator GatherLevelInfosAsync()
    {
        errorText.text = "Gathering Infos...";
        List<KeyValuePair<string, UserGeneratedLevelInfo>> levelInfos = GetSortedLevelInfos();
        yield return null;

        errorText.text = "Count: " + levelInfos.Count;

        /*foreach (var keyValuePair in levelInfos)
        {
            Debug.Log(keyValuePair.Value.AuthorName);
            Debug.Log(keyValuePair.Value.LevelName);
            Debug.Log(keyValuePair.Value.Online);
            Debug.Log(keyValuePair.Value.DBNodeKey);
            Debug.Log(keyValuePair.Value.UserRating);
            Debug.Log("----");
        }*/

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
        switch (_currentSortCategory)
        {
            case LevelBrowserSortCategory.AuthorName:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderBy(d => d.Value.AuthorName).ToList();
                }
                return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.AuthorName).ToList();
            case LevelBrowserSortCategory.Date:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderBy(d => d.Value.Date).ToList();
                }
                return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.Date).ToList();
            case LevelBrowserSortCategory.LevelName:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderBy(d => d.Value.LevelName).ToList();
                }
                return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.LevelName).ToList();
            case LevelBrowserSortCategory.Rating:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderBy(d => d.Value.UserRating).ToList();
                }
                return StageAndLevelDataManager.Instance.GetDownloadedUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.UserRating).ToList();
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
        LevelBrowserSortCategory newSortCategory = (LevelBrowserSortCategory)value;
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

    /*public void DownloadingLevelInfosCompleteInteger(List<DataSnapshot> data, int newStartIndex, int pageIndex, string lastEntryKey = null)
    {
        if (pageIndex < 0) return;
        _alreadyDownloadedIndices.Add(pageIndex);

        if (data.Count < 1)
        {
            _levelBrowserPageSelection.RemovePage(pageIndex);
            return;
        }

        _levelBrowserPageSelection.Pages[pageIndex].CreateLevelTiles(data);
        _integerNumericStartIndex = newStartIndex;
        _lastPageIndex++;

        if (lastEntryKey != null)
        {
            _lastEntryKey = lastEntryKey;
        }

        if (pageIndex == 0)
        {
            DownloadLeveInfosForPage(_lastPageIndex + 1);
        }
    }

    private void Update()
    {
        if (Input.GetKey("d"))
        {
            // FirebaseManager.Instance.DummyLoad();
        }
    }



    public void DownloadLeveInfosForPage(int pageIndex)
    {
        if (pageIndex < 0 || _alreadyDownloadedIndices.Contains(pageIndex)) return;

        int qtyLevels = QTY_LEVELS_PER_PAGE;
        if (pageIndex > 0)
        {
            qtyLevels++;
        }
        switch (SortCategoryDataType(_currentSortCategory))
        {
            case LevelBrowserCategoryDataType.Integer:
                FirebaseManager.Instance.GetPaginatedLevelInfos(_currentSortCategory, _currentSortType, _integerNumericStartIndex, qtyLevels, pageIndex, _lastEntryKey, DownloadingLevelInfosCompleteInteger);
                break;
            case LevelBrowserCategoryDataType.Double:
                FirebaseManager.Instance.GetPaginatedLevelInfos(_currentSortCategory, _currentSortType, _doubleNumericStartIndex, qtyLevels, pageIndex, _lastEntryKey, DownloadingLevelInfosCompleteDouble);
                break;
            case LevelBrowserCategoryDataType.String:
                FirebaseManager.Instance.GetPaginatedLevelInfos(_currentSortCategory, _currentSortType, _aplhabeticStartIndex, qtyLevels, pageIndex, _lastEntryKey, DownloadingLevelInfosCompleteAlphabetical);
                break;
        }

        _levelBrowserPageSelection.AddPage();
    }


    public void DownloadingLevelInfosCompleteDouble(List<DataSnapshot> data, double newStartIndex, int pageIndex, string lastEntryKey = null)
    {
        if (pageIndex < 0) return;
        _alreadyDownloadedIndices.Add(pageIndex);

        if (data.Count < 1)
        {
            _levelBrowserPageSelection.RemovePage(pageIndex);
            return;
        }

        _levelBrowserPageSelection.Pages[pageIndex].CreateLevelTiles(data);
        _doubleNumericStartIndex = newStartIndex;
        _lastPageIndex++;

        if (lastEntryKey != null)
        {
            _lastEntryKey = lastEntryKey;
        }

        if (pageIndex == 0)
        {
            DownloadLeveInfosForPage(_lastPageIndex + 1);
        }
    }

    public void DownloadingLevelInfosCompleteAlphabetical(List<DataSnapshot> data, string newStartIndex, int pageIndex, string lastEntryKey = null)
    {
        if (pageIndex < 0) return;
        _alreadyDownloadedIndices.Add(pageIndex);

        if (data.Count < 1)
        {
            _levelBrowserPageSelection.RemovePage(pageIndex);
            return;
        }

        _levelBrowserPageSelection.Pages[pageIndex].CreateLevelTiles(data);
        _aplhabeticStartIndex = newStartIndex;
        _lastPageIndex++;

        if (lastEntryKey != null)
        {
            _lastEntryKey = lastEntryKey;
        }

        if (pageIndex == 0)
        {
            DownloadLeveInfosForPage(_lastPageIndex + 1);
        }
    }*/

    public void SetSortType(LevelBrowserSortType sortType)
    {
        _currentSortType = sortType;
    }

    public void SetSortCategory(LevelBrowserSortCategory sortCategory)
    {
        _currentSortCategory = sortCategory;
    }

    public void ShowDetailPage(LevelBrowserLevelTile tile)
    {
        LevelBrowserDetailView.Create(tile.LevelInfo);
    }
}