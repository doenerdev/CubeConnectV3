using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGridFieldButton : MonoBehaviour
{
    protected bool _selectable = true;

    [SerializeField] protected Button _gridFieldButtonScript;
    [SerializeField] protected GridFieldType _gridFieldType;
    [SerializeField] protected GridFieldColor _gridFieldColor = GridFieldColor.None;

    public GridFieldType GridFieldType
    {
        get { return _gridFieldType; }
    }

    public virtual void Clicked()
    {
        if (_selectable == false) return;

        switch (GameManager.Instance.GameState)
        {
            case GameState.LevelEditor:
                LevelEditorProduction.Instance.SetGridField(_gridFieldType, _gridFieldColor);
                break;
            case GameState.LevelEditorUser:
                LevelEditorUser.Instance.SetGridField(_gridFieldType, _gridFieldColor);
                break;
        }
    }

    public void Selectable(bool selectable)
    {
        _selectable = selectable;
        _gridFieldButtonScript.interactable = selectable;
    }
}


