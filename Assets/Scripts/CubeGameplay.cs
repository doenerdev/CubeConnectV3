using System.Collections;
using System.Collections.Generic;
using TagFrenzy;
using UnityEngine;

public class CubeGameplay : Singleton<CubeGameplay>
{

    private CameraRotation _cubeCameraRotation;
    private CubeRotation _cubeRotation;

    public CameraRotation CubeCameraRotation
    {
        get { return _cubeCameraRotation; }
    }
    public CubeRotation CubeRotation
    {
        get
        {
            if(Cube.Instance != null && _cubeRotation == null)
                _cubeRotation = Cube.Instance.GetComponent<CubeRotation>();
            return _cubeRotation;
        }
    }

    private void Awake()
    {
        if (GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            LevelData levelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels[PersistentSceneData.CurrentLevelIndex];
            Cube cube = Cube.Create(levelData);
            cube.transform.SetParent(transform, false);
            cube.gameObject.SetActive(true);
        }
        else if (GameManager.Instance.GameState == GameState.WorkshopCubeGameplay)
        {
            LevelData levelData = PersistentSceneData.CurrentUserGeneratedLevelData;
            Cube cube = Cube.Create(levelData);
            cube.transform.SetParent(transform, false);
            cube.gameObject.SetActive(true);
        }

        _cubeCameraRotation = MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>();
        _cubeCameraRotation.enabled = true;

        if (Cube.Instance != null)
        {
            _cubeRotation = Cube.Instance.GetComponent<CubeRotation>();
        }
    }

    public void NextLevel()
    {
        StartCoroutine(InitializeNextLevel());
    }

    public IEnumerator PlayLevelTransitionAnimationIn()
    {
        PlayManager.Instance.LevelTransitionAnimator.SetTrigger("AnimIn");
        yield return new WaitForSeconds(2);
    }

    public IEnumerator PlayLevelTransitionAnimationOut()
    {
        PlayManager.Instance.LevelTransitionAnimator.SetTrigger("AnimOut");
        yield return null;
    }

    IEnumerator InitializeNextLevel()
    {
        yield return StartCoroutine(PlayLevelTransitionAnimationIn());
        _cubeCameraRotation = MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>();
        _cubeCameraRotation.enabled = false;
        Destroy(Cube.Instance.gameObject);
        yield return null;

        LevelData levelData = StageAndLevelDataManager.Instance.GetStageDataByIndex(PersistentSceneData.CurrentStageIndex).Levels[PersistentSceneData.CurrentLevelIndex];
        Cube cube = Cube.Create(levelData);
        cube.transform.SetParent(transform, false);
        cube.gameObject.SetActive(true);
        _cubeCameraRotation = MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>();
        _cubeRotation = Cube.Instance.GetComponent<CubeRotation>();
        _cubeCameraRotation.enabled = true;

        yield return StartCoroutine(PlayLevelTransitionAnimationOut());
    }
}
