using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMulitDirectionsGridFieldButton : CreateMultiGridFieldButton
{
    [SerializeField] protected List<PossibleConnectionDirection> _possibleConnectionDirections;

    public override void Clicked()
    {
        if (_selectable == false) return;

        foreach (var possibleConnectionDirection in _possibleConnectionDirections)
        {
            Debug.Log("Direction:" + possibleConnectionDirection.Direction + " Value:" + possibleConnectionDirection.Possible);
        }

        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.SetGridField(_gridFieldType, _gridFieldColor, _requiredConnections, _possibleConnectionDirections);
                break;
            case GameState.LevelEditorUser:
                LevelEditorUser.Instance.SetGridField(_gridFieldType, _gridFieldColor, _requiredConnections, _possibleConnectionDirections);
                break;
        }
    }
}
