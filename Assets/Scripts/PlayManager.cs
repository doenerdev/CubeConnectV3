using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayManager : Singleton<PlayManager>
{

    private int _moves; //the overall quantity of moves the player made (including reversed moves)
    private int _unfolds; //the overall quantity of unfolds initiated by the player

    [SerializeField] private Text _textQtyCurrentConnections;
    [SerializeField] private Text _textQtyMaxConnections;
    [SerializeField] private Animator _levelTransitionAnimator;
    [SerializeField] private LevelCompletedCanvas _levelCompletedCanvas;

    public Text errorText;

    public int Moves
    {
        set {
            _moves = value;
            if(GameManager.Instance.GameState == GameState.CubeGameplay)
                _textQtyCurrentConnections.text = _moves.ToString();
        }
        get { return _moves; }
    }
    public int Unfolds
    {
        get { return _unfolds; }
    }

    public LevelCompletedCanvas LevelCompletedCanvas
    {
        get
        {
            return _levelCompletedCanvas;
        }
    }

    public void SetMaxQtyConnections(int qty)
    {
        if (GameManager.Instance.GameState == GameState.CubeGameplay || GameManager.Instance.GameState == GameState.WorkshopCubeGameplay)
        {
            _textQtyMaxConnections.text = "/" + qty.ToString();
        }
    }

    public void SetQtyConnections(int qty)
    {
        if (GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            _textQtyCurrentConnections.text = qty.ToString();
            if (qty >= Cube.Instance.NecessaryConnectionsToWin)
            {
                UpdateCurrentLevelStatus();

                //win condition reached, do something (e.g. play animation, show rating stars etc...)
                bool loadNextLevel = true;

                if (StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels.Count - 1 > PersistentSceneData.CurrentLevelIndex) //check whether this was the last level in the current stage
                {
                    PersistentSceneData.CurrentLevelIndex++;
                }
                else if (StageAndLevelDataManager.Instance.GetStages().Count - 1 > PersistentSceneData.CurrentStageIndex && StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex + 1).Levels.Count > 0) //check if this is the last available stage
                {
                    PersistentSceneData.CurrentStageIndex++;
                    PersistentSceneData.CurrentLevelIndex = 0;
                }
                else
                {
                    loadNextLevel = false;
                }

                if (loadNextLevel)
                {
                    //CubeGameplay.Instance.NextLevel();
                    GameManager.Instance.LoadCubeGameplayLevel(PersistentSceneData.CurrentStageIndex, PersistentSceneData.CurrentLevelIndex);
                }
                else
                {
                    //GameManager.Instance.ShowStageAndLevelSelectionByIndex(PersistentSceneData.CurrentStageIndex);
                    ReturnToLevelSelectionAfterLeveFinished();
                }
            }
        }
        else if (GameManager.Instance.GameState == GameState.WorkshopCubeGameplay)
        {
            _textQtyCurrentConnections.text = qty.ToString();
            if (qty >= Cube.Instance.NecessaryConnectionsToWin)
            {
                ReturnToLevelSelectionAfterLeveFinished();
            }
        }
    }

    public void ReturnToLevelSelectionAfterLeveFinished()
    {
        StartCoroutine(InitializeReturnToLevelSelection());
    }

    private IEnumerator InitializeReturnToLevelSelection()
    {
        yield return StartCoroutine(CubeGameplay.Instance.PlayLevelTransitionAnimationIn());

        if (GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            GameManager.Instance.ShowStageAndLevelSelectionByIndex(PersistentSceneData.CurrentStageIndex);
        }
        else if (GameManager.Instance.GameState == GameState.WorkshopCubeGameplay)
        {
            GameManager.Instance.ShowWorkshopPlayLevelBrowser();
        }
    }

    public void ReturnToStageAndLevelSelection()
    {
        GameManager.Instance.ShowStageAndLevelSelectionByIndex(PersistentSceneData.CurrentStageIndex);
    }

    private void UpdateCurrentLevelStatus()
    {
        LevelData currentLevelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels[PersistentSceneData.CurrentLevelIndex];
        currentLevelData.SetRating(CalculateCurrentLevelRating());
        currentLevelData.SetLevelStatus(LevelStatus.Finished);

        if (GameManager.Instance.Nutzertest == false) //TODO remove later
            PersistentSceneData.MainMenu.StageAndLevelSelection.Stages[PersistentSceneData.CurrentStageIndex].LevelTiles[PersistentSceneData.CurrentLevelIndex].UpdateStatus(LevelStatus.Finished, CalculateCurrentLevelRating());

        if (StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels.Count - 1 > PersistentSceneData.CurrentLevelIndex) 
            //check if there are more leves or whether this was the last level in the current stage
        {       
            LevelData nextLevelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels[PersistentSceneData.CurrentLevelIndex + 1];
            if (nextLevelData.LevelStatus == LevelStatus.Locked)
            {
                nextLevelData.SetLevelStatus(LevelStatus.Unlocked);

                if (GameManager.Instance.Nutzertest == false) //TODO remove later
                    PersistentSceneData.MainMenu.StageAndLevelSelection.Stages[PersistentSceneData.CurrentStageIndex].LevelTiles[PersistentSceneData.CurrentLevelIndex + 1].UpdateStatus(nextLevelData.LevelStatus, nextLevelData.Rating);
            }
        }
        else if (StageAndLevelDataManager.Instance.GetStages().Count - 1 > PersistentSceneData.CurrentStageIndex && StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex + 1).Levels.Count > 0)
        //check if there are more stages or if this is the last available stage
        {
            LevelData nextLevelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex + 1).Levels[0];
            if (nextLevelData.LevelStatus == LevelStatus.Locked)
            {
                nextLevelData.SetLevelStatus(LevelStatus.Unlocked);
                if (GameManager.Instance.Nutzertest == false) //TODO remove later
                    PersistentSceneData.MainMenu.StageAndLevelSelection.Stages[PersistentSceneData.CurrentStageIndex].LevelTiles[PersistentSceneData.CurrentLevelIndex + 1].UpdateStatus(nextLevelData.LevelStatus, nextLevelData.Rating);
            }
        }

        if (GameManager.Instance.Nutzertest == false) //TODO remove later
            StageAndLevelDataManager.Instance.SaveLevelAndStageData();
    }

    public uint CalculateCurrentLevelRating()
    {
        uint rating = 1;
        float moveExcessPercentage = (float) _moves/Cube.Instance.NecessaryConnectionsToWin - 1f;
        moveExcessPercentage += 0.15f * _unfolds;

        if (moveExcessPercentage < 0.15f)
        {
            rating = 3;
        }
        else if (moveExcessPercentage < 0.4f)
        {
            rating = 2;
        }

        return rating;
    }

    public void RevertButtonClicked()
    {
        if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded)
        {
            Cube.Instance.RevertConnection();
        }
    }

    public void FoldButtonClicked()
    {
        if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded || Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap)
        {
            if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded) _unfolds++;
            Cube.Instance.ChangeState(CubeStateID.CubeStateFoldingTransition);
        }
    }
}
