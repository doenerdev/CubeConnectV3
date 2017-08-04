using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

[Serializable]
public class EmptyGridField : GridField {

    private GridFieldSelectionBehaviour _selectionBehaviour = new SimpleGridFieldSelectionBehaviour();
    private GridFieldConnectionBehaviour _connectionBehaviour = new EmptyGridFieldConnectionBehaviour();
    private int _requiredConnections = 0;

    public override Object GetSpecializedGridField()
    {
        return this;
    }

    protected override void LoadMaterials()
    {
        _gridFieldMaterials = new Dictionary<GridFieldMaterial, Material>();

        if (_cubeSide == CubeSide.Imagenary)
        {
            _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/Imagenary") as Material);
        }
        else
        {
            _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/Grid") as Material);
        }
       
        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }

    protected override void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false || GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            //Debug.Log("Grid Field clicked");
            _parentCube.RegisterClickedGridField(this);
        }
    }

    protected override void OnMouseOver()
    {
        //Debug.Log("Mouse over" + _gridPosition);
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
