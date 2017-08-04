using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBrowserScrollView : MonoBehaviour
{

    private int _closestToCenterStageSlideIndex;
    private int _currentPageIndex;
    private bool _sliding = false;
    private float _stageSlideWidth;
    private List<float> _stageSlideCenterDistances;
    private LevelBrowserPageSelection _levelBrowserPageSelection;
    private bool _initialized;

    [SerializeField] private Transform _scrollViewCenter;
    [SerializeField] private RectTransform _scrollPanel;
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private float _slideSpeed;

    public float StageSlideWidth
    {
        get { return _stageSlideWidth; }
    }
    public Transform ContentContainer
    {
        get { return _contentContainer; }
    }

    public void Initialize(LevelBrowserPageSelection levelBrowserPageSelection)
    {
        _levelBrowserPageSelection = levelBrowserPageSelection;
        _stageSlideWidth = _scrollPanel.localScale.x * _scrollPanel.sizeDelta.x;
        _stageSlideCenterDistances = new List<float>();
        _initialized = true;
    }

    private void Update()
    {
        if (_initialized == false)
            return;

        for (int i = 0; i < _stageSlideCenterDistances.Count; i++)
        {
            _stageSlideCenterDistances[i] = Mathf.Abs(_scrollViewCenter.transform.position.x - _levelBrowserPageSelection.Pages[i].transform.position.x);
        }

        float minDistance = Mathf.Min(_stageSlideCenterDistances.ToArray());
        for (int i = 0; i < _levelBrowserPageSelection.Pages.Count; i++)
        {
            if (minDistance == _stageSlideCenterDistances[i])
            {
                _closestToCenterStageSlideIndex = i;
                if (_sliding == false)
                {
                    
                    if (i > _currentPageIndex)
                    {
                        LevelBrowser.Instance.DownloadLeveInfosForPage(i + 1);
                    }
                    _currentPageIndex = i;
                    SlideToIndex(i);
                }
                break;
            }
        }
    }

    public void AddPage(LevelBrowserLevelSelection page)
    {
        _stageSlideCenterDistances.Add(Mathf.Abs(_scrollViewCenter.transform.position.x - page.transform.position.x));
    }

    public void RemovePage(int index)
    {
        _stageSlideCenterDistances.RemoveAt(index);
        if (_currentPageIndex == index && index > 1)
        {
            SlideToIndex(index - 1);
        }
    }

    public void RemoveAllPages()
    {
        SlideToIndex(0);
        _stageSlideCenterDistances.Clear();
    }

    public void StartSliding()
    {
        _sliding = true;
    }

    public void EndSliding()
    {
        _sliding = false;
    }

    public void SlideToIndex(int index)
    {
        float positionX = Mathf.Lerp(_scrollPanel.anchoredPosition.x, index * -_stageSlideWidth, Time.deltaTime * _slideSpeed);
        Vector2 newPosition = new Vector2(positionX, _scrollPanel.anchoredPosition.y);
        _scrollPanel.anchoredPosition = newPosition;
    }
}
