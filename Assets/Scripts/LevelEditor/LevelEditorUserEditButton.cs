using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUserEditButton : MonoBehaviour
{
    [SerializeField] private LevelEditorLevelBrowserLevelTile _levelTile;

    public void Clicked()
    {
        LevelEditorUser.Instance.LevelSelected(_levelTile.LevelInfo.LevelCode);
    }
}
