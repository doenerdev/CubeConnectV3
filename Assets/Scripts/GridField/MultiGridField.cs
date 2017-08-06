using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

[Serializable]
[ProtoContract]
public class MultiGridField : SimpleGridField
{
    protected new GridFieldConnectionBehaviour _connectionBehaviour = new MultiGridFieldConnectionBehaviour();
    protected new int _requiredConnections = 2;

    public override Object GetSpecializedGridField()
    {
        return this;
    }

    public void SetRequiredConnections(int requiredConnections)
    {
        if (requiredConnections > 2 && requiredConnections <= 4)
        {
            Debug.Log(_requiredConnections);
            _requiredConnections = requiredConnections;
        }
    }

    protected override void LoadMaterials()
    {
        _gridFieldMaterials = new Dictionary<GridFieldMaterial, Material>();

        switch (_gridFieldColor)
        {
            case GridFieldColor.Blue:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/MultiGridDot") as Material);
                break;
            case GridFieldColor.Red:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/MultiGridDotRed") as Material);
                break;
            case GridFieldColor.Multi:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/MultiGridDotMulti") as Material);
                break;
        }

        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }

    public override GridFieldConnectionState GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState state)
    {
        if (state != GridFieldConnectionState.Empty)
        {
            _qtyConnections++;
        }
        else
        {
            _qtyConnections = 0;
        }

        return _qtyConnections >= _requiredConnections ? GridFieldConnectionState.ClosedMultiConnection : GridFieldConnectionState.OpenMultiConnection;
    }

    public override int RequiredConnections()
    {
        return _requiredConnections;
    }

    protected override GridFieldConnectionBehaviour GetGridFieldConnectionBehaviour()
    {
        return _connectionBehaviour;
    }
}
