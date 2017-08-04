using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelTile : MonoBehaviour
{

    private int _levelIndex;
    private LevelStatus _status;
    private MainMenuStageSelection _parentStageSelection;

    [SerializeField] private GameObject _ratingStarPrefab;
    [SerializeField] private GameObject _noRatingStarPrefab;
    [SerializeField] private Image _tileBackground;
    [SerializeField] private Color _unlockedTileColor;
    [SerializeField] private Color _lockedTileColor;
    [SerializeField] private GameObject _ratingStarsContainer;

    public static MainMenuLevelTile Create(int index, MainMenuStageSelection stageSelection)
    {
        GameObject go = Instantiate(Resources.Load("MainMenu/LevelTile")) as GameObject;
        MainMenuLevelTile levelTile = go.GetComponent<MainMenuLevelTile>();
        LevelData levelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(stageSelection.StageIndex).Levels[index];
        levelTile._levelIndex = index;
        levelTile._status = levelData.LevelStatus;
        levelTile._parentStageSelection = stageSelection;

        switch (levelTile._status)
        {
            case LevelStatus.Finished:
                levelTile._tileBackground.color = levelTile._unlockedTileColor;
                levelTile._ratingStarsContainer.SetActive(true);

                levelTile.UpdateStatus(levelTile._status, levelData.Rating);
                break;
            case LevelStatus.Unlocked:
                levelTile._tileBackground.color = levelTile._unlockedTileColor;
                levelTile._ratingStarsContainer.SetActive(false);
                break;
            case LevelStatus.Locked:
                levelTile._tileBackground.color = levelTile._lockedTileColor;
                levelTile._ratingStarsContainer.SetActive(false);
                break;
        }

        return levelTile;
    }

    public void UpdateStatus(LevelStatus status, uint rating)
    {
        switch (status)
        {
            case LevelStatus.Finished:
                _tileBackground.color = _unlockedTileColor;
                _ratingStarsContainer.SetActive(true);

                foreach (Transform child in _ratingStarsContainer.transform)
                {
                    Destroy(child.gameObject);
                }
                int starsCounter = 0;
                for (int i = 0; i < rating; i++)
                {
                    GameObject ratingStar = Instantiate(_ratingStarPrefab) as GameObject;
                    ratingStar.transform.SetParent(_ratingStarsContainer.transform);
                    starsCounter++;
                }
                for (int i = starsCounter; i < 3; i++)
                {
                    GameObject ratingStar = Instantiate(_noRatingStarPrefab) as GameObject;
                    ratingStar.transform.SetParent(_ratingStarsContainer.transform);
                }
                break;
            case LevelStatus.Unlocked:
                _tileBackground.color = _unlockedTileColor;
                _ratingStarsContainer.SetActive(false);
                break;
            case LevelStatus.Locked:
                _tileBackground.color = _lockedTileColor;
                _ratingStarsContainer.SetActive(false);
                break;
        }
    }

    public void Clicked()
    {
        if (_status != LevelStatus.Locked)
        {
            GameManager.Instance.LoadCubeGameplayLevel(_parentStageSelection.StageIndex, _levelIndex);
        }
    }
}
