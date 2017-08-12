using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStageAndLevelSelection : MonoBehaviour
{

    private MainMenuStageSelection[] _stages;

    [SerializeField] private MainMenuStageScrollView _stagesScrollView;

    public MainMenuStageSelection[] Stages
    {
        get { return _stages; }
    }

    public static MainMenuStageAndLevelSelection Create(Transform parentCanvas)
    {
        GameObject go = Instantiate(Resources.Load("MainMenu/StageAndLevelSelection")) as GameObject;
        MainMenuStageAndLevelSelection stageAndLevelSelection = go.GetComponent<MainMenuStageAndLevelSelection>();
        stageAndLevelSelection.transform.SetParent(parentCanvas, false);
        stageAndLevelSelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
        stageAndLevelSelection.name = "StageAndLevelSelection";

        List<StageData> stages = StageAndLevelDataManager.Instance.GetStages();
        stageAndLevelSelection._stages = new MainMenuStageSelection[stages.Count];
        stageAndLevelSelection._stagesScrollView.gameObject.SetActive(true);

        stageAndLevelSelection.StartCoroutine(stageAndLevelSelection.CreateStages());

        return stageAndLevelSelection;
    }

    private IEnumerator CreateStages()
    {
        for (int i = 0; i < _stages.Length; i++)
        {
            _stages[i] = MainMenuStageSelection.Create(i, _stagesScrollView);
            yield return null;
        }
        _stagesScrollView.Initialize(this);
        RaiseCreatingStageAndLevelSelectionComplete("Created stage and level selection");
    }

    private void RaiseCreatingStageAndLevelSelectionComplete(string message)
    {
        EventHandler<EventTextArgs> handler = CreatingStageAndLevelSelectionComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    public void HomeButtonClicked()
    {   
        Debug.Log("Show Main Menu Clicked");
        GameManager.Instance.ShowMainMenu();
    }

    #region Events
    public event EventHandler<EventTextArgs> CreatingStageAndLevelSelectionComplete;
    #endregion Events
}
