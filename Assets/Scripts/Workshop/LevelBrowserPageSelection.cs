using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBrowserPageSelection : MonoBehaviour
{

    protected List<LevelBrowserLevelSelection> _pages;

    [SerializeField] private LevelBrowserScrollView _levelBrowserScrollView;

    public List<LevelBrowserLevelSelection> Pages
    {
        get { return _pages; }
    }

    public void Initialize(Transform parentCanvas)
    {
        transform.SetParent(parentCanvas, false);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        name = "LevelBrowserPageSelection";

        //List<StageData> pages = StageAndLevelDataManager.Instance.GetStages();
        _pages = new List<LevelBrowserLevelSelection>();
        _levelBrowserScrollView.gameObject.SetActive(true);

        StartCoroutine(CreatePages());
    }

    private IEnumerator CreatePages()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            _pages[i] = LevelBrowserLevelSelection.Create(i, _levelBrowserScrollView);
            yield return null;
        }
        _levelBrowserScrollView.Initialize(this);
        
    }

    public void AddPage()
    {
        _pages.Add(LevelBrowserLevelSelection.Create(_pages.Count, _levelBrowserScrollView));
        _levelBrowserScrollView.AddPage(_pages[_pages.Count -1]);
    }

    public void RemovePage(int index)
    {
        LevelBrowserLevelSelection pageBuffer = _pages[index];
        _pages.RemoveAt(index);
        _levelBrowserScrollView.RemovePage(index);
        Destroy(pageBuffer.gameObject);
    }

    public void RemoveAllPages()
    {
        foreach (var page in _pages)
        {
            Destroy(page.gameObject);
        }

        _pages.Clear();
        _levelBrowserScrollView.RemoveAllPages();
    }

    public void HomeButtonClicked()
    {
        GameManager.Instance.ShowMainMenu();
    }
}
