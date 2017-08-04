using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMultiGridFieldButton : CreateGridFieldButton {

    [SerializeField][Range(2,4)] protected int _requiredConnections = 2;

    public override void Clicked()
    {
        if (_selectable == false) return;

        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.SetGridField(_gridFieldType, _gridFieldColor, _requiredConnections);
                break;
            case GameState.LevelEditorUser:
                LevelEditorUser.Instance.SetGridField(_gridFieldType, _gridFieldColor, _requiredConnections);
                break;
        }
    }
}
