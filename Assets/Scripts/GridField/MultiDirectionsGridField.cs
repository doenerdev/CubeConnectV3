using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class MultiDirectionsGridField : MultiGridField
{
    public List<PossibleConnectionDirection> _possibleConnectionDirections;

    public override UnityEngine.Object GetSpecializedGridField()
    {
        return gameObject.GetComponent<MultiDirectionsGridField>();
    }

    public override void InitializeGridField(Cube parentCube, IntVector2 gridPositon, GridFieldType gridFieldType, GridFieldColor color = GridFieldColor.None)
    {
        base.InitializeGridField(parentCube, gridPositon, gridFieldType, color);
        if (color == GridFieldColor.None)
        {
            _gridFieldColor = GridFieldColor.Blue;
        }
    }

    private void InitializePossbileConnectionDirections()
    {
        _possibleConnectionDirections = new List<PossibleConnectionDirection>()
        {
            new PossibleConnectionDirection(ConnectionDirection.Top, false),
            new PossibleConnectionDirection(ConnectionDirection.Bottom, false),
            new PossibleConnectionDirection(ConnectionDirection.Left, false),
            new PossibleConnectionDirection(ConnectionDirection.Right, false),
        };
    }

    public bool IsDirectionPossible(ConnectionDirection direction)
    {
        return _possibleConnectionDirections.Count(x => x.Direction == direction && x.Possible == true) > 0 ? true : false;
    }

    public void SetPossibleDirections(List<PossibleConnectionDirection> possibleConnections)
    {
        if(_possibleConnectionDirections == null) InitializePossbileConnectionDirections();
        if (possibleConnections == null) return;

        foreach (var possibleConnection in possibleConnections)
        {
            int index = _possibleConnectionDirections.FindIndex(x => x.Direction == possibleConnection.Direction);
            if (index >= 0)
            {
                _possibleConnectionDirections[index] = possibleConnection;
            }
        }
    }

    public override int RequiredConnections()
    {
        return _possibleConnectionDirections.Count(x => true);
    }
}


[Serializable]
public enum ConnectionDirection
{
    None,
    Top,
    Bottom,
    Left,
    Right,
}

[Serializable]
public struct PossibleConnectionDirection
{
    public ConnectionDirection Direction;
    public bool Possible;

    public PossibleConnectionDirection(ConnectionDirection direction, bool possible)
    {
        Direction = direction;
        Possible = possible;
    }
}