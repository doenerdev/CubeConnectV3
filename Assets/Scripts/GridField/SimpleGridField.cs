using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

[Serializable]
public class SimpleGridField : GridField
{
    protected GridFieldSelectionBehaviour _selectionBehaviour = new SimpleGridFieldSelectionBehaviour();
    protected GridFieldConnectionBehaviour _connectionBehaviour = new SimpleGridFieldConnectionBehaviour();
    protected int _requiredConnections = 1;

    public override Object GetSpecializedGridField()
    {
        return gameObject.GetComponent<SimpleGridField>();
    }

    public override void InitializeGridField(Cube parentCube, IntVector2 gridPositon, GridFieldType gridFieldType, GridFieldColor color = GridFieldColor.None)
    {
        base.InitializeGridField(parentCube, gridPositon, gridFieldType, color);
        if (color == GridFieldColor.None)
        {
            _gridFieldColor = GridFieldColor.Blue;
        }
    }

    protected override void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false || GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            _parentCube.RegisterClickedGridField(this);
        }
    }

    protected override void OnMouseOver()
    {
        //Debug.Log("Mouse over" + _gridPosition);
    }

    protected override void LoadMaterials()
    {
        _gridFieldMaterials = new Dictionary<GridFieldMaterial, Material>();

        switch (_gridFieldColor)
        {
            case GridFieldColor.Blue:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridDot") as Material);
                break;
            case GridFieldColor.Red:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridDotRed") as Material);
                break;
            case GridFieldColor.Multi:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridDotMulti") as Material);
                break;
        }

        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }

    public override int RequiredConnections()
    {
        return _requiredConnections;
    }

    protected override GridFieldConnectionBehaviour GetGridFieldConnectionBehaviour()
    {
        return _connectionBehaviour;
    }

    protected override GridFieldSelectionBehaviour GetGridFieldSelectionBehaviour()
    {
        return _selectionBehaviour;
    }
}
