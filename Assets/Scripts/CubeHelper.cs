using UnityEngine;
using System.Collections;

public class CubeHelper
{
    /// <summary>  
    /// Given a grid field, this returns the opposite grid field on the cube
    /// </summary>
    public static GridField GetOppositeSideGridField(GridField originGridField)
    {
        switch (originGridField.CubeSide)
        {
            case CubeSide.Bottom:
            case CubeSide.Back:
                return Cube.Instance.CubeMap[originGridField.GridPosition.x, originGridField.GridPosition.y + Cube.Instance.GridSize * 2];
            case CubeSide.Top:
            case CubeSide.Front:
                return Cube.Instance.CubeMap[originGridField.GridPosition.x, originGridField.GridPosition.y - Cube.Instance.GridSize * 2];
            case CubeSide.Left:
                return Cube.Instance.CubeMap[originGridField.GridPosition.x + Cube.Instance.GridSize * 2, originGridField.GridPosition.y];
            case CubeSide.Right:
                return Cube.Instance.CubeMap[originGridField.GridPosition.x - Cube.Instance.GridSize * 2, originGridField.GridPosition.y];
            default:
                return null;
        }
    }


    /// <summary>  
    /// Returns the connection direction between two gridfields taking the target's perspective. Therefore the
    /// connection direction can be referred to as "incoming".
    /// </summary>
    public static ConnectionDirection GetConnectionDirectionIncoming(GridField gridFieldOrigin, GridField gridFieldTarget)
    {
        if ((gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Bottom) ||
            (gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Right) ||
            (gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Left))
        {
            return ConnectionDirection.Bottom;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Top) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Right))
        {
            return ConnectionDirection.Top;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Right) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Top) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Back) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Back && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Bottom))
        {
            return ConnectionDirection.Left;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Top) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Bottom) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Back) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Back && gridFieldTarget.CubeSide == CubeSide.Right))
        {
            return ConnectionDirection.Right;
        }
        else
        {
            if (gridFieldOrigin.GridPosition.x < gridFieldTarget.GridPosition.x)
            {
                return ConnectionDirection.Right;
            }
            else if (gridFieldOrigin.GridPosition.x > gridFieldTarget.GridPosition.x)
            {
                return ConnectionDirection.Left;
            }
            else if (gridFieldOrigin.GridPosition.y > gridFieldTarget.GridPosition.y)
            {
                return ConnectionDirection.Top;
            }
            else
            {
                return ConnectionDirection.Bottom;
            }
        }
    }

    /// <summary>  
    /// Returns the connection direction between two gridfields taking the origin's perspective. Therefore the
    /// connection direction can be referred to as "outgoing".
    /// </summary>
    public static ConnectionDirection GetConnectionDirectionOutgoing(GridField gridFieldOrigin, GridField gridFieldTarget)
    {
        if ((gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Bottom) ||
            (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Bottom) ||
            (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Bottom))
        {
            return ConnectionDirection.Bottom;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Top) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Top) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Top))
        {
            return ConnectionDirection.Top;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Right) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Right) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Right) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Back) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Back && gridFieldTarget.CubeSide == CubeSide.Right))
        {
            return ConnectionDirection.Right;
        }
        else if ((gridFieldOrigin.CubeSide == CubeSide.Bottom && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Top && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Left && gridFieldTarget.CubeSide == CubeSide.Back) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Back && gridFieldTarget.CubeSide == CubeSide.Left) ||
                 (gridFieldOrigin.CubeSide == CubeSide.Right && gridFieldTarget.CubeSide == CubeSide.Left))
        {
            return ConnectionDirection.Left;
        }
        else
        {
            if (gridFieldOrigin.GridPosition.x < gridFieldTarget.GridPosition.x)
            {
                return ConnectionDirection.Left;
            }
            else if (gridFieldOrigin.GridPosition.x > gridFieldTarget.GridPosition.x)
            {
                return ConnectionDirection.Right;
            }
            else if (gridFieldOrigin.GridPosition.y > gridFieldTarget.GridPosition.y)
            {
                return ConnectionDirection.Bottom;
            }
            else
            {
                return ConnectionDirection.Top;
            }
        }
    }

    /// <summary>  
    /// Checks whether two grid fields are on the same row of the cube (and could therefore be connected)
    /// </summary>
    public static bool GridFieldsOnSameRow(GridField gridFieldOrigin, GridField gridFieldTarget, GridField imagenaryGridField = null)
    {
        if (imagenaryGridField == null)
        {
            //special case top->left/right and vice versa
            if ((gridFieldOrigin.CubeSide == CubeSide.Top && (gridFieldTarget.CubeSide == CubeSide.Left || gridFieldTarget.CubeSide == CubeSide.Right)) ||
                (gridFieldTarget.CubeSide == CubeSide.Top && (gridFieldOrigin.CubeSide == CubeSide.Left || gridFieldOrigin.CubeSide == CubeSide.Right)))
            {
                int yTop = gridFieldTarget.CubeSide == CubeSide.Top ? gridFieldTarget.GridPosition.y : gridFieldOrigin.GridPosition.y;
                int ySide = gridFieldTarget.CubeSide == CubeSide.Top ? gridFieldOrigin.GridPosition.y : gridFieldTarget.GridPosition.y;

                if ((Cube.Instance.GridSize*3 -1) - yTop == ySide)
                    return true;
            }
            else if (((gridFieldOrigin.GridPosition.x == gridFieldTarget.GridPosition.x) ||  //check if the grid fields can be connected without an imagenary cube side
                 (gridFieldOrigin.GridPosition.y == gridFieldTarget.GridPosition.y)))
            {
                return true;
            }
        }
        else //check for a posible connection via an imagenary cube side
        {
            // get the nearest two grid fields (one on the same row as the origin grid field, one on the same row as the target grid field) originating from the appropriate imagenary grid field
            // and check if the distances to the imagenary grid field are equal
            int distanceOriginToImag = IntVector2.Distance(imagenaryGridField.GridPosition,
                GetNearestGridFieldFromImagenary(imagenaryGridField, IntVector2.DirectionFromTo(imagenaryGridField.GridPosition, gridFieldOrigin.GridPosition)).GridPosition);
            int distanceImagToTarget = IntVector2.Distance(imagenaryGridField.GridPosition,
                GetNearestGridFieldFromImagenary(imagenaryGridField, IntVector2.DirectionFromTo(imagenaryGridField.GridPosition, gridFieldTarget.GridPosition)).GridPosition);

            if (distanceImagToTarget == distanceOriginToImag)
                return true;         
        }

        return false;
    }

    /// <summary>  
    /// Returns the nearest non-imagenary grid field to an imagenary grid field, given a certain direction
    /// </summary> 
    private static GridField GetNearestGridFieldFromImagenary(GridField imagenaryGridField, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                for (int y = imagenaryGridField.GridPosition.y; y < Cube.Instance.CubeMap.GetLength(1); y++)
                {
                    if (Cube.Instance.CubeMap[imagenaryGridField.GridPosition.x, y].CubeSide != CubeSide.Imagenary)
                        return Cube.Instance.CubeMap[imagenaryGridField.GridPosition.x, y];
                }
                break;
            case Direction.Down:
                for (int y = imagenaryGridField.GridPosition.y; y >= 0; y--)
                {
                    if (Cube.Instance.CubeMap[imagenaryGridField.GridPosition.x, y].CubeSide != CubeSide.Imagenary)
                        return Cube.Instance.CubeMap[imagenaryGridField.GridPosition.x, y];
                }
                break;
            case Direction.Right:
                for (int x = imagenaryGridField.GridPosition.x; x < Cube.Instance.CubeMap.GetLength(0); x++)
                {
                    if (Cube.Instance.CubeMap[x, imagenaryGridField.GridPosition.y].CubeSide != CubeSide.Imagenary)
                        return Cube.Instance.CubeMap[x, imagenaryGridField.GridPosition.y];
                }
                break;
            case Direction.Left:
                for (int x = imagenaryGridField.GridPosition.x; x >= 0; x--)
                {
                    if (Cube.Instance.CubeMap[x, imagenaryGridField.GridPosition.y].CubeSide != CubeSide.Imagenary)
                        return Cube.Instance.CubeMap[x, imagenaryGridField.GridPosition.y];
                }
                break;
        }
        return null;
    }
}
