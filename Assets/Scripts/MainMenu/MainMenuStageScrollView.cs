using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStageScrollView : MonoBehaviour {

    private int _closestToCenterStageSlideIndex;
    private int _currentStageIndex;
    private bool _sliding = false;
    private float _stageSlideWidth;
    private float[] _stageSlideCenterDistances;
    private MainMenuStageAndLevelSelection _stageAndLevelSelection;
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

    public void Initialize(MainMenuStageAndLevelSelection stageAndLevelSelection)
    {
        _stageAndLevelSelection = stageAndLevelSelection;
        _stageSlideWidth = _scrollPanel.localScale.x * _scrollPanel.sizeDelta.x;
        _stageSlideCenterDistances = new float[_stageAndLevelSelection.Stages.Length];
        _initialized = true;
    }

    private void Update()
    {
        if (_initialized == false)
            return;

        for (int i = 0; i < _stageSlideCenterDistances.Length; i++)
        {
            _stageSlideCenterDistances[i] = Mathf.Abs(_scrollViewCenter.transform.position.x - _stageAndLevelSelection.Stages[i].transform.position.x);
        }

        float minDistance = Mathf.Min(_stageSlideCenterDistances);
        for (int i = 0; i < _stageAndLevelSelection.Stages.Length; i++)
        {
            if (minDistance == _stageSlideCenterDistances[i])
            {
                _closestToCenterStageSlideIndex = i;
                if (_sliding == false)
                    SlideToIndex(i);
                break;
            }
        }
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
