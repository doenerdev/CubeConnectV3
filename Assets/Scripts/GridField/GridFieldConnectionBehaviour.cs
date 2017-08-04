using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

[Serializable]
public abstract class GridFieldConnectionBehaviour
{

    public GridField[,] GetAppropriateCubeMap(bool playabilityTesting = false)
    {
        if (playabilityTesting == true)
        {
            return LevelEditorLevelPlayabilityValidator.ValidatorCubeMap;
        }
        else
        {
            return Cube.Instance.CubeMap;
        } 
    } 

    //the connection state added to grid fields by this connection type
    protected GridFieldConnectionState _accordinGridFieldConnectionState = GridFieldConnectionState.Empty;

    /// <summary>  
    /// Constructs and returns the appropriate GridFieldConnection between two grid fields, indicating whether the connection is valid and holding all the grid fields used for the connection
    /// </summary>
    public abstract GridFieldConnection GetGridFieldConnection(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false);


    /// <summary>  
    /// Checks whether a grid field is a valid connection piece depending on its type and connection state
    /// </summary>
    protected virtual bool IsGridFieldValidConnectionPiece(GridField gridField)
    {
        return ValidConnectionGridFieldTypes.ContainsKey(gridField.GridFieldType) && ValidConnectionGridFieldTypes[gridField.GridFieldType] &&
            ValidConnectionGridFieldConnectionStates.ContainsKey(gridField.ConnectionState) && ValidConnectionGridFieldConnectionStates[gridField.ConnectionState];
    }

    protected virtual bool IsValidTarget(GridField targetGridField, GridField originGridField)
    {
        //check for a MultiDirectionsGridField special case and possible early outs
        if (originGridField.GridFieldType == GridFieldType.MultiDirectionsGridField)
        {
            MultiDirectionsGridField origin = originGridField.GetSpecializedGridField() as MultiDirectionsGridField;
            Debug.Log("Outgoing Direction:" + CubeHelper.GetConnectionDirectionOutgoing(originGridField, targetGridField)); //TODO remove later
            if (origin.IsDirectionPossible(CubeHelper.GetConnectionDirectionOutgoing(originGridField, targetGridField)) == false) return false;
        }
        if (targetGridField.GridFieldType == GridFieldType.MultiDirectionsGridField)
        {
            MultiDirectionsGridField target = targetGridField.GetSpecializedGridField() as MultiDirectionsGridField;
            Debug.Log("Incoming Direction:" + CubeHelper.GetConnectionDirectionIncoming(originGridField, targetGridField)); //TODO remove later
            if (target.IsDirectionPossible(CubeHelper.GetConnectionDirectionIncoming(originGridField, targetGridField)) == false) return false;
        }

        Debug.Log("Direction valid"); //TODO remove later

        return ValidTargetGridFieldTypes.ContainsKey(targetGridField.GridFieldType) && ValidTargetGridFieldTypes[targetGridField.GridFieldType] &&
            ValidTargetGridFieldConnetionStates.ContainsKey(targetGridField.ConnectionState) && ValidTargetGridFieldConnetionStates[targetGridField.ConnectionState];
    }

    protected virtual bool DoColorsMatchValid(GridField originGridField, GridField targetGridField)
    {
        return (originGridField.GridFieldColor == GridFieldColor.Multi || targetGridField.GridFieldColor == GridFieldColor.Multi) || (originGridField.GridFieldColor == targetGridField.GridFieldColor);
    }

    /// <summary>  
    /// Constructs and returns the appropriate GridFieldConnection between two grid fields, given a specific axis to move along
    /// </summary>
    protected GridFieldConnection CheckStraightConnectionFromTo(GridField gridFieldOrigin, GridField gridFieldTarget, CubeAxis axis, bool checkOrigin = false, bool checkTarget = false, bool playabilityTesting = false)
    {
        GridFieldConnection gridFieldConnection = new GridFieldConnection();
        gridFieldConnection.ValidGridFields = new List<GridField>();
        bool connectionValid = true;

        //special case for front-top connection (and vice versa). because of the 2d-laymap, testing for a out-of-bound connection is needed
        if ((gridFieldOrigin.CubeSide == CubeSide.Front && gridFieldTarget.CubeSide == CubeSide.Top) ||
            (gridFieldTarget.CubeSide == CubeSide.Front && gridFieldOrigin.CubeSide == CubeSide.Top))
        {
            return CheckStraightConnectionOutOfBounds(gridFieldOrigin, gridFieldTarget, playabilityTesting);
        }
        else if (axis == CubeAxis.X)
        {
            int loopIncrementer = gridFieldOrigin.GridPosition.x > gridFieldTarget.GridPosition.x ? -1 : 1; //get the moving direction
            for (int x = gridFieldOrigin.GridPosition.x; x != gridFieldTarget.GridPosition.x; x += 1 * loopIncrementer) //iterate through the connection
            {
                if (x == gridFieldOrigin.GridPosition.x) //check for the origin itself
                {
                    if (checkOrigin == false || IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[x, gridFieldOrigin.GridPosition.y]) || gridFieldOrigin == gridFieldTarget) //if necessary, check the origin grid field as well
                    {
                        gridFieldConnection.ValidGridFields.Add(GetAppropriateCubeMap(playabilityTesting)[x, gridFieldOrigin.GridPosition.y]);
                        continue;
                    }
                    else
                    {
                        connectionValid = false;
                        break;
                    }
                }

                if (IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[x, gridFieldOrigin.GridPosition.y]))
                {
                    gridFieldConnection.ValidGridFields.Add(GetAppropriateCubeMap(playabilityTesting)[x, gridFieldOrigin.GridPosition.y]);
                }
                else
                {
                    connectionValid = false;
                    break;
                }
            }

            if (connectionValid && (checkTarget == false || IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[gridFieldTarget.GridPosition.x, gridFieldTarget.GridPosition.y]) || gridFieldOrigin == gridFieldTarget)) //if necessary, check the target grid field as well
                gridFieldConnection.ValidGridFields.Add(gridFieldTarget);
        }
        else
        {
            int loopIncrementer = gridFieldOrigin.GridPosition.y > gridFieldTarget.GridPosition.y ? -1 : 1; //get the moving direction
            for (int y = gridFieldOrigin.GridPosition.y; y != gridFieldTarget.GridPosition.y; y += 1 * loopIncrementer) //iterate through the connection
            {
                if (y == gridFieldOrigin.GridPosition.y) //check for the origin itself
                {
                    if (checkOrigin == false || IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, y]) || gridFieldOrigin == gridFieldTarget) //if necessary, check the origin grid field as well
                    {
                        gridFieldConnection.ValidGridFields.Add(GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, y]);
                        continue;
                    }
                    else
                    {
                        connectionValid = false;
                        break;
                    }
                }

                if (IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, y]))
                {
                    gridFieldConnection.ValidGridFields.Add(GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, y]);
                }
                else
                {
                    connectionValid = false;
                    break;
                }
            }

            if (connectionValid && (checkTarget == false || IsGridFieldValidConnectionPiece(GetAppropriateCubeMap(playabilityTesting)[gridFieldTarget.GridPosition.x, gridFieldTarget.GridPosition.y]) || gridFieldOrigin == gridFieldTarget)) //if necessary, check the target grid field as well
                gridFieldConnection.ValidGridFields.Add(gridFieldTarget);
        }

        gridFieldConnection.Valid = connectionValid;
        return gridFieldConnection;
    }

    /// <summary>  
    /// Constructs and returns the appropriate GridFieldConnection between two grid fields, given the special case of an out-of-bounds connection (top-front/front-top)
    /// </summary>
    protected GridFieldConnection CheckStraightConnectionOutOfBounds(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        GridFieldConnection gridFieldConnection = new GridFieldConnection();
        gridFieldConnection.ValidGridFields = new List<GridField>();

        int direction = gridFieldOrigin.GridPosition.y > gridFieldTarget.GridPosition.y ? 1 : -1;
        int bound = gridFieldOrigin.GridPosition.y > gridFieldTarget.GridPosition.y ? (int)(Cube.Instance.GridSize * 4) - 1 : 0; //get the bounds

        if (gridFieldOrigin.GridPosition.y != bound)
        {
            GridField gridFieldFirstBound = GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, bound];
            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, gridFieldFirstBound, CubeAxis.Y, false, true, playabilityTesting); //get the connection from origin to the first bound
        }
        else
        {
            gridFieldConnection.ValidGridFields.Add(gridFieldOrigin);
            gridFieldConnection.Valid = true;
        }
            
        if (gridFieldConnection.Valid)
        {
            int secondBound = bound + ((((int) Cube.Instance.GridSize*4) - 1) * -direction);
            if (gridFieldTarget.GridPosition.y != secondBound)
            {
                GridField gridFieldSecondBound = GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, secondBound];
                GridFieldConnection gridFieldConnectionBoundToTarget = CheckStraightConnectionFromTo(gridFieldSecondBound, gridFieldTarget, CubeAxis.Y, true, false, playabilityTesting); //check connection from the secound bound to target
                gridFieldConnection.ValidGridFields = gridFieldConnection.ValidGridFields.Concat(gridFieldConnectionBoundToTarget.ValidGridFields).ToList();
                gridFieldConnection.Valid = gridFieldConnectionBoundToTarget.Valid;
            }
        }

        if (gridFieldConnection.Valid)
            gridFieldConnection.ValidGridFields.Add(gridFieldTarget);

        return gridFieldConnection;
    }

    /// <summary>  
    /// Checks whether two grid fields can be connected via a simple connection (not using an imaganery cube side of the 2D-layplan) and returns a GridFieldConnection
    /// </summary>
    protected GridFieldConnection CanConnectGridFields(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        GridFieldConnection gridFieldConnection = new GridFieldConnection();
        gridFieldConnection.Valid = false;

        if (CubeHelper.GridFieldsOnSameRow(gridFieldOrigin, gridFieldTarget) == false)
            return gridFieldConnection;
        
        //check for special connection top->left/right and vice versa
        if ((gridFieldOrigin.CubeSide == CubeSide.Top && (gridFieldTarget.CubeSide == CubeSide.Left || gridFieldTarget.CubeSide == CubeSide.Right)) ||
            (gridFieldTarget.CubeSide == CubeSide.Top && (gridFieldOrigin.CubeSide == CubeSide.Left || gridFieldOrigin.CubeSide == CubeSide.Right)))
        {
            gridFieldConnection = CanConnectSpecialTopToLeftRight(gridFieldOrigin, gridFieldTarget, playabilityTesting);
        }
        else if (gridFieldOrigin.GridPosition.x == gridFieldTarget.GridPosition.x)
        {
            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, gridFieldTarget, CubeAxis.Y, false, false, playabilityTesting);
        }
        else if (gridFieldOrigin.GridPosition.y == gridFieldTarget.GridPosition.y)
        {
            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, gridFieldTarget, CubeAxis.X, false, false, playabilityTesting);
        }

        return gridFieldConnection;
    }

    /// <summary>  
    /// Checks whether a connection from top to left/right or vice versa is possible and returns a GridFieldConnection
    /// </summary>
    protected GridFieldConnection CanConnectSpecialTopToLeftRight(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        GridFieldConnection gridFieldConnection = new GridFieldConnection();
        gridFieldConnection.Valid = false;


        if (gridFieldOrigin.CubeSide == CubeSide.Top && (gridFieldTarget.CubeSide == CubeSide.Left || gridFieldTarget.CubeSide == CubeSide.Right))
        {
            GridField originTarget, targetOrigin;

            if (IntVector2.DirectionFromTo(gridFieldOrigin.GridPosition, new IntVector2(gridFieldTarget.GridPosition.x, gridFieldOrigin.GridPosition.y)) == Direction.Left)
            {
                originTarget = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize, gridFieldOrigin.GridPosition.y]; //the target to connect to from origin
                targetOrigin = GetAppropriateCubeMap(playabilityTesting)[0, gridFieldTarget.GridPosition.y]; //the origin to connect from to target
            }
            else
            {
                originTarget = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize * 2 - 1, gridFieldOrigin.GridPosition.y];
                targetOrigin = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize * 3 - 1, gridFieldTarget.GridPosition.y];
            }

            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, originTarget, CubeAxis.X, false, true, playabilityTesting); //first, connect to the outer bound of the origin cube side (include the target field in the validity check)
            if (gridFieldConnection.Valid == true)
            {
                GridFieldConnection gridFieldConnectionTarget = CheckStraightConnectionFromTo(targetOrigin, gridFieldTarget, CubeAxis.X, true, false, playabilityTesting); //connect from the outer bound of the target cube side to the target (include the origin field in the validity check)
                gridFieldConnection.ValidGridFields = gridFieldConnection.ValidGridFields.Concat(gridFieldConnectionTarget.ValidGridFields).ToList();
                gridFieldConnection.Valid = gridFieldConnectionTarget.Valid;
            }
        }
        else if (gridFieldTarget.CubeSide == CubeSide.Top && (gridFieldOrigin.CubeSide == CubeSide.Left || gridFieldOrigin.CubeSide == CubeSide.Right))
        {
            GridField originTarget, targetOrigin;

            if (IntVector2.DirectionFromTo(gridFieldOrigin.GridPosition, new IntVector2(gridFieldTarget.GridPosition.x, gridFieldOrigin.GridPosition.y)) == Direction.Left)
            {
                originTarget = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize * 3 - 1, gridFieldOrigin.GridPosition.y]; //the target to connect to from origin
                targetOrigin = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize * 2 - 1, gridFieldTarget.GridPosition.y]; //the origin to connect from to target
            }
            else
            {
                originTarget = GetAppropriateCubeMap(playabilityTesting)[0, gridFieldOrigin.GridPosition.y];
                targetOrigin = GetAppropriateCubeMap(playabilityTesting)[Cube.Instance.GridSize, gridFieldTarget.GridPosition.y];
            }

            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, originTarget, CubeAxis.X, false, true, playabilityTesting); //first, connect to the outer bound of the origin cube side (include the target field in the validity check)
            if (gridFieldConnection.Valid == true)
            {
                GridFieldConnection gridFieldConnectionTarget = CheckStraightConnectionFromTo(targetOrigin, gridFieldTarget, CubeAxis.X, true, false, playabilityTesting); //connect from the outer bound of the target cube side to the target (include the origin field in the validity check)
                gridFieldConnection.ValidGridFields = gridFieldConnection.ValidGridFields.Concat(gridFieldConnectionTarget.ValidGridFields).ToList();
                gridFieldConnection.Valid = gridFieldConnectionTarget.Valid;
            }
        }

        return gridFieldConnection;
    }

    /// <summary>  
    /// Checks whether two grid fields can be connected through an imagenary cube side of the 2D-layplan and returns a GridFieldConnection
    /// </summary>
    protected GridFieldConnection CanConnetGridFieldsViaImagenary(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        GridFieldConnection gridFieldConnection = new GridFieldConnection();
        gridFieldConnection.Valid = false;
        bool connectionValid = true;

        if (_validImagenaryConnectionSides[gridFieldOrigin.CubeSide][gridFieldTarget.CubeSide] == false) //check if the side are suitable for an imagenary connection
            return gridFieldConnection;

        //first check for the appropriate imagenary cube side and check if the origin and target grid field are on the same cube row
        if (GetAppropriateCubeMap(playabilityTesting)[gridFieldTarget.GridPosition.x, gridFieldOrigin.GridPosition.y]._cubeSide == CubeSide.Imagenary && CubeHelper.GridFieldsOnSameRow(gridFieldOrigin, gridFieldTarget, GetAppropriateCubeMap(playabilityTesting)[gridFieldTarget.GridPosition.x, gridFieldOrigin.GridPosition.y]))
        {
            GridField imagenaryGridField = GetAppropriateCubeMap(playabilityTesting)[gridFieldTarget.GridPosition.x, gridFieldOrigin.GridPosition.y];
            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, imagenaryGridField, CubeAxis.X, false, false, playabilityTesting); //get the connection from origin to imag

            if (gridFieldConnection.Valid == true) //check if the connection from origin to imag is valid
            {
                GridFieldConnection gridFieldConnectionImagToTarget = CheckStraightConnectionFromTo(imagenaryGridField, gridFieldTarget, CubeAxis.Y, false, false, playabilityTesting); //check connection from imag to target
                gridFieldConnection.ValidGridFields = gridFieldConnection.ValidGridFields.Concat(gridFieldConnectionImagToTarget.ValidGridFields).ToList();
                gridFieldConnection.Valid = gridFieldConnectionImagToTarget.Valid;
            }
        }
        else if (GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, gridFieldTarget.GridPosition.y]._cubeSide == CubeSide.Imagenary && CubeHelper.GridFieldsOnSameRow(gridFieldOrigin, gridFieldTarget, GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, gridFieldTarget.GridPosition.y]))
        {
            GridField imagenaryGridField = GetAppropriateCubeMap(playabilityTesting)[gridFieldOrigin.GridPosition.x, gridFieldTarget.GridPosition.y]; 
            gridFieldConnection = CheckStraightConnectionFromTo(gridFieldOrigin, imagenaryGridField, CubeAxis.Y, false, false, playabilityTesting); //get the connection from origin to imag

            if (gridFieldConnection.Valid == true) //check if the connection from origin to imag is valid
            {
                GridFieldConnection gridFieldConnectionImagToTarget = CheckStraightConnectionFromTo(imagenaryGridField, gridFieldTarget, CubeAxis.X, false, false, playabilityTesting); //check connection from imag to target
                gridFieldConnection.ValidGridFields = gridFieldConnection.ValidGridFields.Concat(gridFieldConnectionImagToTarget.ValidGridFields).ToList();
                gridFieldConnection.Valid = gridFieldConnectionImagToTarget.Valid;
            }
        }

        if(gridFieldConnection.Valid)
            gridFieldConnection.ValidGridFields.Add(gridFieldTarget); //also add the target itself to the connection

        return gridFieldConnection;
    }

    protected Dictionary<GridFieldType, bool> _validTargetGridFieldTypes = new Dictionary<GridFieldType, bool>()
    {
        { GridFieldType.EmptyGridField, false},
        { GridFieldType.BarrierGridField, false},
        { GridFieldType.SimpleGridField, true},
        { GridFieldType.PortalGridField, true},
        { GridFieldType.MultiGridField, true},
        { GridFieldType.MultiDirectionsGridField, true},
    };
    public virtual Dictionary<GridFieldType, bool> ValidTargetGridFieldTypes 
    {
        get { return _validTargetGridFieldTypes; }
    }

    protected Dictionary<GridFieldConnectionState, bool> _validTargetGridFieldConnetionStates = new Dictionary<GridFieldConnectionState, bool>()
    {
        { GridFieldConnectionState.Empty, true},
        { GridFieldConnectionState.SimpleConnection, false},
        { GridFieldConnectionState.OpenMultiConnection, true},
        { GridFieldConnectionState.ClosedMultiConnection, false},
    };
    public virtual Dictionary<GridFieldConnectionState, bool> ValidTargetGridFieldConnetionStates
    {
        get { return _validTargetGridFieldConnetionStates; }
    }

    protected Dictionary<GridFieldType, bool> _validConnectionGridFieldTypes = new Dictionary<GridFieldType, bool>()
    {
        { GridFieldType.EmptyGridField, true},
        { GridFieldType.BarrierGridField, false},
        { GridFieldType.SimpleGridField, false},
        { GridFieldType.PortalGridField, false},
        { GridFieldType.MultiGridField, false},
        { GridFieldType.MultiDirectionsGridField, false},
    };
    public virtual Dictionary<GridFieldType, bool> ValidConnectionGridFieldTypes
    {
        get { return _validConnectionGridFieldTypes; }
    }

    protected Dictionary<GridFieldConnectionState, bool> _validConnectionGridFieldConnectionStates = new Dictionary<GridFieldConnectionState, bool>()
    {
        { GridFieldConnectionState.Empty, true},
        { GridFieldConnectionState.SimpleConnection, false},
        { GridFieldConnectionState.OpenMultiConnection, false},
        { GridFieldConnectionState.ClosedMultiConnection, false},
    };
    public virtual Dictionary<GridFieldConnectionState, bool> ValidConnectionGridFieldConnectionStates
    {
        get { return _validConnectionGridFieldConnectionStates; }
    }

    protected Dictionary<CubeSide, Dictionary<CubeSide, bool>> _validImagenaryConnectionSides = new Dictionary<CubeSide, Dictionary<CubeSide, bool>>()
    {
        {CubeSide.Back, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Front, false},
                {CubeSide.Top, false},
                {CubeSide.Bottom, false},
                {CubeSide.Right, true},
                {CubeSide.Back, false},
            }
        },
        {CubeSide.Bottom, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, false},
                {CubeSide.Front, false},
                {CubeSide.Top, false},
                {CubeSide.Back, false},
                {CubeSide.Right, false},
                {CubeSide.Bottom, false},
            }
        },
        {CubeSide.Left, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Back, true},
                {CubeSide.Front, true},
                {CubeSide.Top, false},
                {CubeSide.Bottom, false},
                {CubeSide.Right, false},
                {CubeSide.Left, false},
            }
        },
        {CubeSide.Right, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, false},
                {CubeSide.Front, true},
                {CubeSide.Top, false},
                {CubeSide.Back, true},
                {CubeSide.Bottom, false},
                {CubeSide.Right, false},
            }
        },
        {CubeSide.Front, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Bottom, false},
                {CubeSide.Top, false},
                {CubeSide.Back, false},
                {CubeSide.Right, true},
                {CubeSide.Front, false},
            }
        },
        {CubeSide.Top, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, false},
                {CubeSide.Front, false},
                {CubeSide.Bottom, false},
                {CubeSide.Back, false},
                {CubeSide.Right, false},
                {CubeSide.Top, false},
            }
        },
    };
}
