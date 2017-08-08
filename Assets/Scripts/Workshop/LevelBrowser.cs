using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowser : Singleton<LevelBrowser>
{
    public const int QTY_LEVELS_PER_PAGE = 2;
    public const string LAST_ALPHABETICAL_STRING = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
    public LevelBrowserSortCategory _currentSortCategory;
    public LevelBrowserSortType _currentSortType;
    private int _lastPageIndex = -1;
    private string _aplhabeticStartIndex = null;
    private int _integerNumericStartIndex = Int32.MinValue;
    private double _doubleNumericStartIndex = Double.MinValue;
    public string _lastEntryKey = null;
    private List<int> _alreadyDownloadedIndices;


    [SerializeField] private Transform _parentCanvas;
    [SerializeField] private LevelBrowserScrollView _levelBrowserScrollView;
    [SerializeField] private LevelBrowserPageSelection _levelBrowserPageSelection;
    [SerializeField] private Dropdown _sortCategoryDropdown;

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
        _alreadyDownloadedIndices = new List<int>();
        _levelBrowserPageSelection.Initialize(_parentCanvas);


        _sortCategoryDropdown.options.Clear();
        int enumCounter = 0;
        foreach (LevelBrowserSortCategory sortCategory in Enum.GetValues(typeof(LevelBrowserSortCategory)))
        {
            _sortCategoryDropdown.options.Add(new Dropdown.OptionData(Enum.GetName(typeof(LevelBrowserSortCategory), enumCounter)));
            enumCounter++;
        }
        _sortCategoryDropdown.onValueChanged.AddListener(SortCategoryDropdownChanged); 

        DownloadLeveInfosForPage(_lastPageIndex + 1);
    }

    private void Update()
    {
        if (Input.GetKey("d"))
        {
           // FirebaseManager.Instance.DummyLoad();
        }
    }

    public void Refresh()
    {
        _levelBrowserPageSelection.RemoveAllPages();
        _lastPageIndex = -1;
        _aplhabeticStartIndex = null;
        _integerNumericStartIndex = Int32.MinValue;
        _doubleNumericStartIndex = Double.MinValue;
        _alreadyDownloadedIndices = new List<int>();

        DownloadLeveInfosForPage(_lastPageIndex + 1);
    }

    public void SortCategoryDropdownChanged(int value)
    {
        LevelBrowserSortCategory newSortCategory = (LevelBrowserSortCategory) value;
        Debug.Log(_currentSortCategory == newSortCategory);
        Debug.Log(newSortCategory);
        Debug.Log(_currentSortCategory);
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

    public void DownloadLeveInfosForPage(int pageIndex)
    {
        if (pageIndex < 0 || _alreadyDownloadedIndices.Contains(pageIndex)) return;

        int qtyLevels = QTY_LEVELS_PER_PAGE;
        if (pageIndex > 0)
        {
            qtyLevels++;
        }
        Debug.Log(_currentSortCategory);
        Debug.Log(SortCategoryDataType(_currentSortCategory));
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

    public void DownloadingLevelInfosCompleteInteger(List<DataSnapshot> data, int newStartIndex, int pageIndex, string lastEntryKey = null)
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

    public void DownloadingLevelInfosCompleteDouble(List<DataSnapshot> data, double newStartIndex, int pageIndex, string lastEntryKey = null)
    {
        Debug.Log("new startIndex:" + newStartIndex);
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
            Debug.Log(pageIndex);
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
            Debug.Log(pageIndex);
            DownloadLeveInfosForPage(_lastPageIndex + 1);
        }

        /*foreach (var dataSnapshot in data)
        {
            Debug.Log(dataSnapshot.Child("AuthorName").Value);
            Debug.Log("-----------------------------------");
        }*/
    }

    public LevelBrowserCategoryDataType SortCategoryDataType(LevelBrowserSortCategory sortCategory)
    {
        switch (sortCategory)
        {
            case LevelBrowserSortCategory.AuthorName:
            case LevelBrowserSortCategory.LevelName:
                return LevelBrowserCategoryDataType.String;
                break;
            case LevelBrowserSortCategory.Rating:
                return LevelBrowserCategoryDataType.Double;
                break;
            default:
                return LevelBrowserCategoryDataType.Integer;
        }
    }

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


public enum LevelBrowserCategoryDataType
{
    String,
    Integer,
    Double,
}
public enum LevelBrowserSortCategory
{
    AuthorName,
    LevelName,
    Rating,
    Date,
}
public enum LevelBrowserSortType
{
    Ascending,
    Descending,
}