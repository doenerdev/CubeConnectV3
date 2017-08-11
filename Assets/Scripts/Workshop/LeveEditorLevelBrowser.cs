using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeveEditorLevelBrowser : MonoBehaviour {

    public const int QTY_LEVELS_PER_PAGE = 2;
    public LevelBrowserEditorSortCategory _currentSortCategory;
    public LevelBrowserSortType _currentSortType;

    [SerializeField] private Transform _parentCanvas;
    [SerializeField] private LevelEditorLevelBrowserScrollView _levelBrowserScrollView;
    [SerializeField] private LevelEditorLevelBrowserPageSelection _levelBrowserPageSelection;
    [SerializeField] private Dropdown _sortCategoryDropdown;

    public Text errorText;

    public LevelBrowserEditorSortCategory CurrentSortCategory
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
        switch (_currentSortCategory)
        {        
            case LevelBrowserEditorSortCategory.Date:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderBy(d => d.Value.Date).ToList();
                }
                return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.Date).ToList();
            case LevelBrowserEditorSortCategory.LevelName:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderBy(d => d.Value.LevelName).ToList();
                }
                return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.LevelName).ToList();
            case LevelBrowserEditorSortCategory.Online:
                if (_currentSortType == LevelBrowserSortType.Ascending)
                {
                    return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderBy(d => d.Value.Online).ToList();
                }
                return StageAndLevelDataManager.Instance.GetOwnUserGeneratedLevelInfosList().OrderByDescending(d => d.Value.Online).ToList();
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
        LevelBrowserEditorSortCategory newSortCategory = (LevelBrowserEditorSortCategory)value;
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

    public void SetSortType(LevelBrowserSortType sortType)
    {
        _currentSortType = sortType;
    }

    public void SetSortCategory(LevelBrowserEditorSortCategory sortCategory)
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


public enum LevelBrowserEditorSortCategory
{
    Date,
    LevelName,
    Online,
}