using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class BarrierGridField : EmptyGridField {

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
        _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/Barrier") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }
}
