using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLevelButton : MonoBehaviour {

    public void Clicked()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.SaveLevel();
                break;
            case GameState.LevelEditorUser:
                LevelEditorUser.Instance.SaveLevel();
                break;
        }
    }
}
