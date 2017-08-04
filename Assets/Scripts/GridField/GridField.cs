using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
public abstract class GridField : MonoBehaviour, ICloneable
{
    protected Cube _parentCube;
    protected IntVector2 _gridPosition;
    protected IntVector2 _localGridPosition;
    protected GridFieldType _gridFieldType;
    [SerializeField] protected GridFieldConnectionState _connectionState = GridFieldConnectionState.Empty;
    protected GridFieldColor _gridFieldColor;
    protected int _requiredConnections = 0;
    protected int _qtyConnections = 0;

    protected Dictionary<GridFieldMaterial, Material> _gridFieldMaterials;

    public CubeSide _cubeSide;

    public IntVector2 GridPosition
    {
        get { return _gridPosition; }
    }
    public IntVector2 LocalGridPosition
    {
        get { return _localGridPosition; }
    }
    public CubeSide CubeSide
    {
        get { return _cubeSide; }
    }
    public GridFieldType GridFieldType
    {
        get { return _gridFieldType; }
    }
    public GridFieldConnectionState ConnectionState
    {
        get { return _connectionState; }
    }
    public GridFieldColor GridFieldColor
    {
        get { return _gridFieldColor; }
    }
    public Dictionary<GridFieldMaterial, Material> GridFieldMaterials
    {
        get { return _gridFieldMaterials; }
    }
    public int QtyConnections
    {
        get { return _qtyConnections; }
    }

    protected abstract void OnMouseUp();

    protected abstract void OnMouseOver();

    /// <summary>  
    /// Returns the appropriate grid field connection to a target grid field
    /// </summary>
    public virtual GridFieldConnection GetGridFieldConnection(GridField gridFieldTarget, bool playabilityTesting = false)
    {
        return GetGridFieldConnectionBehaviour().GetGridFieldConnection(this, gridFieldTarget, playabilityTesting);
    }

    /// <summary>  
    /// Returns the appropriate derived grid field instance (e.g. SimpleGridField, PortalGridField etc.)
    /// </summary>
    public abstract Object GetSpecializedGridField();
 
    /// <summary>  
    /// Initialize the grid field. Set its coordinates, the grid field type etc.
    /// </summary>
    public virtual void InitializeGridField(Cube parentCube, IntVector2 gridPositon, GridFieldType gridFieldType, GridFieldColor color = GridFieldColor.None)
    {
        _gridPosition = gridPositon;
        _localGridPosition = new IntVector2(_gridPosition.x % (int) parentCube.GridSize, _gridPosition.y % (int)parentCube.GridSize);
        _parentCube = parentCube;
        _gridFieldType = gridFieldType;
        _gridFieldColor = color;
        //UpdateCubeSide();
        LoadMaterials();
        GetComponent<Renderer>().material = _gridFieldMaterials[GridFieldMaterial.Default];
    }

    protected virtual void LoadMaterials()
    {
        _gridFieldMaterials = new Dictionary<GridFieldMaterial, Material>();
        _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/Grid") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }

    /// <summary>  
    /// Updates the grid fields visuals according to its current connection state
    /// </summary>
    public virtual void UpdateVisualsToConnectionState()
    {
        switch (_connectionState)
        {
            case GridFieldConnectionState.PortalConnection:
            case GridFieldConnectionState.SimpleConnection:
                GetComponent<Renderer>().material = _gridFieldMaterials[GridFieldMaterial.Connected];
                break;
            case GridFieldConnectionState.Empty:
                GetComponent<Renderer>().material = _gridFieldMaterials[GridFieldMaterial.Default];
                break;
        }

    }

    public virtual void SetSelected(bool selected, bool revertingConnection = false)
    {
        GetGridFieldSelectionBehaviour().SetGridFieldSelected(this, selected, revertingConnection);
    }

    /// <summary>  
    /// Set the grid fields connection state (whether it contains a connection or not)
    /// </summary>
    public virtual void SetConnectionState(GridFieldConnectionState state, bool playabilityTesting = false)
    {
        _connectionState = state;
        if (playabilityTesting == false)
        {
            UpdateVisualsToConnectionState();
        }
    }

    /// <summary>  
    /// Get the connection type this gridfield would receive, if it serves as a target grid field
    /// </summary>
    public virtual GridFieldConnectionState GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState state)
    {
        if (_connectionState == GridFieldConnectionState.Empty && state != GridFieldConnectionState.Empty)
        {
            _qtyConnections++;
        }
        else if (state == GridFieldConnectionState.Empty)
        {
            _qtyConnections = 0;
        }

        return state;
    }

    public void SetCubeSide(CubeSide side)
    {
        _cubeSide = side;
    }

    /// <summary>  
    /// Set the cube side this grid field is placed on
    /// </summary>
    protected void UpdateCubeSide()
    {
        if (_gridPosition.x < _parentCube.GridSize)
        {
            _cubeSide = _gridPosition.y >= _parentCube.GridSize * 2 && _gridPosition.y < _parentCube.GridSize * 3
                ? CubeSide.Right
                : CubeSide.Imagenary;
        }
        else if (_gridPosition.x < _parentCube.GridSize * 2)
        {
            if (_gridPosition.y >= _parentCube.GridSize && _gridPosition.y < _parentCube.GridSize * 2)
            {
                _cubeSide = CubeSide.Back;
            }
            else if (_gridPosition.y >= _parentCube.GridSize * 2 && _gridPosition.y < _parentCube.GridSize * 3)
            {
                _cubeSide = CubeSide.Bottom;
            }
            else if (_gridPosition.y >= _parentCube.GridSize * 3 && _gridPosition.y < _parentCube.GridSize * 4)
            {
                _cubeSide = CubeSide.Front;
            }
            else
            {
                _cubeSide = CubeSide.Top;
            }
        }
        else
        {
            _cubeSide = _gridPosition.y >= _parentCube.GridSize * 2 && _gridPosition.y < _parentCube.GridSize * 3
               ? CubeSide.Left
               : CubeSide.Imagenary;
        }
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public abstract int RequiredConnections();

    protected abstract GridFieldConnectionBehaviour GetGridFieldConnectionBehaviour();

    protected abstract GridFieldSelectionBehaviour GetGridFieldSelectionBehaviour();
}

/// <summary>  
/// A vector implementation based on integers
/// </summary>
public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>  
    /// Overrides the == operator for the comparison of the two IntVector2
    /// </summary>
    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    /// <summary>  
    /// Overrides the != operator for the comparison of the two IntVector2
    /// </summary>
    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return !(a == b);
    }

    /// <summary>  
    /// Returns the distance between two integer vectors
    /// </summary>
    public static int Distance(IntVector2 vectorA, IntVector2 vectorB)
    {
        return (int) Mathf.Sqrt(Mathf.Pow((vectorA.x - vectorB.x), 2) + Mathf.Pow((vectorA.y - vectorB.y), 2));
    }

    /// <summary>  
    /// Returns the direction (left, right...) between a origin and a target vector
    /// </summary>
    public static Direction DirectionFromTo(IntVector2 vectorA, IntVector2 vectorB)
    {
        if ((vectorA.x == vectorB.x && vectorA.y == vectorB.y) || (vectorA.x != vectorB.x && vectorA.y != vectorB.y))
            return Direction.None;

        if(vectorA.x < vectorB.x)
            return Direction.Right;
        else if(vectorA.x > vectorB.x)
            return Direction.Left;
        else if(vectorA.y < vectorB.y)
            return  Direction.Up;
        else
            return Direction.Down;
    }
}

public enum GridFieldType
{
    EmptyGridField,
    SimpleGridField,
    BarrierGridField,
    PortalGridField,
    MultiGridField,
    MultiDirectionsGridField,
}

public enum GridFieldConnectionState
{
    Empty,
    SimpleConnection,
    PortalConnection,
    OpenMultiConnection,
    ClosedMultiConnection,
}

public enum GridFieldMaterial
{
    Default,
    Connected,
    Selected
}

public enum GridFieldColor
{
    None,
    Blue,
    Red,
    Multi,
}