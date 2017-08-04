using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class GridFactory : MonoBehaviour
{

    private static GameObject _gridPrefab;
    private static PortalGridField _lastSetPortal;

    public static PortalGridField LastSetPortal
    {
        get { return _lastSetPortal;  }
    }

    static GridFactory()
    {
        _gridPrefab = Resources.Load("GridField") as GameObject;
    }

    public static GridField CreateGridField(IntVector2 gridPositon, GridFieldType type, GridFieldColor color = GridFieldColor.None, int requriedConnections = -1, List<PossibleConnectionDirection> possibleConnectionDirections = null)
    {
        GameObject gridFieldGO = Instantiate(_gridPrefab);

        switch (type)
        {
            case GridFieldType.SimpleGridField:
                gridFieldGO.AddComponent<SimpleGridField>();
                break;
            case GridFieldType.EmptyGridField:
                gridFieldGO.AddComponent<EmptyGridField>();
                break;
            case GridFieldType.BarrierGridField:
                gridFieldGO.AddComponent<BarrierGridField>();
                break;
            case GridFieldType.PortalGridField:
                gridFieldGO.AddComponent<PortalGridField>();

                if (_lastSetPortal != null)
                {
        
                    gridFieldGO.GetComponent<PortalGridField>().RelatedPortalGridField = _lastSetPortal;
                    _lastSetPortal.RelatedPortalGridField = gridFieldGO.GetComponent<PortalGridField>();
                    _lastSetPortal = null;
                }
                else
                {
                    _lastSetPortal = gridFieldGO.GetComponent<PortalGridField>();
                }
                break;
            case GridFieldType.MultiGridField:
                gridFieldGO.AddComponent<MultiGridField>();
                MultiGridField multiGridField = gridFieldGO.GetComponent<MultiGridField>();
                multiGridField.SetRequiredConnections(requriedConnections);
                break;
            case GridFieldType.MultiDirectionsGridField:
                gridFieldGO.AddComponent<MultiDirectionsGridField>();
                MultiDirectionsGridField multiDirGridField = gridFieldGO.GetComponent<MultiDirectionsGridField>();
                multiDirGridField.SetPossibleDirections(possibleConnectionDirections);
                break;
        }

        GridField gridField = gridFieldGO.GetComponent<GridField>() as GridField;
        gridField.InitializeGridField(Cube.Instance, gridPositon, type, color);
        gridField.SetCubeSide(GridFactory.GetCubeSide(Cube.Instance, gridPositon.x, gridPositon.y));
        GridFactory.AdjustGridFieldTransform(gridField, gridPositon);
        gridField.gameObject.name = Enum.GetName(typeof(GridFieldType), type) + gridPositon.x + "|" + gridPositon.y + " | " + gridField.GetComponent<GridField>().CubeSide;

        return gridFieldGO.GetComponent<GridField>();
    }

    private static void AdjustGridFieldTransform(GridField gridField, IntVector2 gridPositon)
    {
        int rotorX = 1, rotorY = 1;
        float posX = 1, posY = 1;
        float scaledGridSize = 1f / Cube.Instance.GridSize;

        switch (gridField.GetComponent<GridField>().CubeSide)
        {
            case CubeSide.Back:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Back.transform);
                gridField.transform.position = Cube.Instance._cubeSideGOs.Back.transform.position;
                rotorY = -1;
                rotorX = -1;
                posX = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posY = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(Cube.Instance.CubeScale / Cube.Instance.GridSize, Cube.Instance.CubeScale / Cube.Instance.GridSize, 0);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(180,0,0)) : gridField.transform.rotation;
                break;
            case CubeSide.Front:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Front.transform);
                gridField.transform.localPosition = Cube.Instance._cubeSideGOs.Front.transform.localPosition;
                rotorX = -1;
                posX = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posY = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(Cube.Instance.CubeScale / Cube.Instance.GridSize, Cube.Instance.CubeScale / Cube.Instance.GridSize, 0);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(0, 0, 0)) : gridField.transform.rotation;
                break;
            case CubeSide.Left:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Left.transform);
                gridField.transform.position = Cube.Instance._cubeSideGOs.Left.transform.position;
               
                posY = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posX = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(0, Cube.Instance.CubeScale / Cube.Instance.GridSize, Cube.Instance.CubeScale / Cube.Instance.GridSize);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(0, -90.00001f, 0)) : gridField.transform.rotation;
                break;
            case CubeSide.Right:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Right.transform);
                gridField.transform.position = Cube.Instance._cubeSideGOs.Right.transform.position;
                rotorX = -1;
                posY = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posX = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(0, Cube.Instance.CubeScale / Cube.Instance.GridSize, Cube.Instance.CubeScale / Cube.Instance.GridSize);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(0, 90.00001f, 0)) : gridField.transform.rotation;
                break;
            case CubeSide.Bottom:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Bottom.transform);
                gridField.transform.position = Cube.Instance._cubeSideGOs.Bottom.transform.position;
                rotorY = -1;
                rotorX = -1;
                posX = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posY = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(Cube.Instance.CubeScale / Cube.Instance.GridSize, 0, Cube.Instance.CubeScale / Cube.Instance.GridSize);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(90.00001f, 0, 0)) : gridField.transform.rotation;
                break;
            case CubeSide.Top:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Top.transform);
                gridField.transform.position = Cube.Instance._cubeSideGOs.Top.transform.position;
                rotorX = -1;
                posX = rotorX * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.x / Cube.Instance.GridSize));
                posY = rotorY * (-(Cube.Instance.CubeScale / 2) + (scaledGridSize / 2) + ((float)gridField.LocalGridPosition.y / Cube.Instance.GridSize));
                gridField.transform.localScale = new Vector3(Cube.Instance.CubeScale / Cube.Instance.GridSize, 0, Cube.Instance.CubeScale / Cube.Instance.GridSize);
                gridField.transform.rotation = Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap ? Quaternion.Euler(new Vector3(-90.00001f, 0, 0)) : gridField.transform.rotation;
                break;
            case CubeSide.Imagenary:
                gridField.transform.SetParent(Cube.Instance._cubeSideGOs.Imagenary.transform);
                break;
        }

        gridField.transform.localPosition = new Vector3(posX, posY, 0);
    }

    private static void CreateOldSchool(GameObject gridFieldGO, Cube parentCube, IntVector2 gridPositon, GridFieldType type)
    {
        GridField gridField = gridFieldGO.GetComponent<GridField>() as GridField;
        gridField.SetCubeSide(GridFactory.GetCubeSide(parentCube, gridPositon.x, gridPositon.y));
        gridField.InitializeGridField(parentCube, gridPositon, type);


        gridField.transform.SetParent(parentCube.transform);
        gridField.transform.localPosition = new Vector3(gridPositon.x, gridPositon.y, 0);

        gridField.gameObject.name = Enum.GetName(typeof(GridFieldType), type) + gridPositon.x + "|" + gridPositon.y + " | " + gridField.GetComponent<GridField>().CubeSide;

    }

    /// <summary>  
    /// Get the cube side this grid field is placed on
    /// </summary>
    public static CubeSide GetCubeSide(Cube cube, int x, int y)
    {
        CubeSide cubeSide;

        if (x < cube.GridSize)
        {
            cubeSide = y >= cube.GridSize * 2 && y < cube.GridSize * 3
                ? CubeSide.Right
                : CubeSide.Imagenary;
        }
        else if (x < cube.GridSize * 2)
        {
            if (y >= cube.GridSize && y < cube.GridSize * 2)
            {
                cubeSide = CubeSide.Back;
            }
            else if (y >= cube.GridSize * 2 && y < cube.GridSize * 3)
            {
                cubeSide = CubeSide.Bottom;
            }
            else if (y >= cube.GridSize * 3 && y < cube.GridSize * 4)
            {
                cubeSide = CubeSide.Front;
            }
            else
            {
                cubeSide = CubeSide.Top;
            }
        }
        else
        {
            cubeSide = y >= cube.GridSize * 2 && y < cube.GridSize * 3
               ? CubeSide.Left
               : CubeSide.Imagenary;
        }

        return cubeSide;
    }
}
