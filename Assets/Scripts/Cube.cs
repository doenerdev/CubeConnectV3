using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

/*
 * The cubemap 2D-layplan-layout is the following:
 *          
 *  imag -  FRONT  - imag
 *  RIGHT - BOTTOM - LEFT
 *  imag -  BACK - imag
 *  imag -   TOP  - imag
 * 
 */

public partial class Cube : Singleton<Cube>
{
    private static uint _defaultGridSize = 4;

    private CubeState _currentCubeState;
    private GridField[,] _cubeMap;
    private GridField _selectedGridField;
    private List<GridFieldConnection> _connections;
    private int _necessaryConnectionsToWin;
    private Dictionary<CubeStateID, CubeState> _cubeStates; 
    private uint _gridSize;
    private float _cubeScale = 1f;
    private Animator _animator;
    private ActiveCubeSide _currentActiveSide;
    private bool _isBusy = false;

    [SerializeField] public CubeSideGameObjects _cubeSideGOs;
    public float rotateSpeed = 0.5f;

    public GridField[,] CubeMap
    {
        get { return _cubeMap; }
    }
    public uint GridSize
    {
        get { return _gridSize; }
    }
    public GridField SelectedGridField
    {
        get { return _selectedGridField; }
    }
    public CubeStateID CurrentCubeStateID
    {
        get { return _currentCubeState.CubeStateID; }
    }
    public CubeState CurrentCubeState
    {
        get { return _currentCubeState; }
    }
    public float CubeScale
    {
        get { return _cubeScale; }
    }
    public Animator Animator
    {
        get { return _animator; }
    }
    public ActiveCubeSide CurrentActiveSide
    {
        get { return _currentActiveSide; }
    }
    public int NecessaryConnectionsToWin
    {
        get { return _necessaryConnectionsToWin; }
    }
    public static uint DefaultGridSize
    {
        get { return _defaultGridSize; }
    }

    public static Cube Create(LevelData levelData = null)
    {
        GameObject cubeGO = Instantiate(Resources.Load("Cube")) as GameObject;
        cubeGO.name = "Cube";
        Cube cube = cubeGO.GetComponent<Cube>();
        cube.InitializeCube(levelData);
        return cube;
    }

    private void InitializeCube(LevelData levelData)
    {
        _currentActiveSide = new ActiveCubeSide();
        _currentActiveSide.CubeSide = CubeSide.Front;
        _currentActiveSide.Rotation = Quaternion.identity;
        _connections = new List<GridFieldConnection>();
        gameObject.transform.localScale = new Vector3(_cubeScale, _cubeScale, _cubeScale);
        _animator = GetComponent<Animator>();
        InitializeEvents();
        InitializeCubeStates();
        InitializeCubeMap(levelData);
    }

    private void Update()
    {
        _currentCubeState.CheckForTransition();
    }

    private void InitializeEvents()
    {
        CubeGameplay.Instance.CubeRotation.CubeRotationCompleted += new EventHandler<EventTextArgs>(CubeRotationComplete);
        CubeGameplay.Instance.CubeRotation.CubeRotationStarted += new EventHandler<EventTextArgs>(CubeRotationStarted);
    }

    private void InitializeCubeStates()
    {
        CubeStateFolded cubeStateFolded = new CubeStateFolded(this, CubeStateID.CubeStateFolded);
        CubeStateLaymap cubeStateLaymap = new CubeStateLaymap(this, CubeStateID.CubeStateLaymap);
        CubeStateFoldingTransition cubeStateFoldingTransition = new CubeStateFoldingTransition(this, CubeStateID.CubeStateFoldingTransition);
        CubeStateRotating cubeStateRotating = new CubeStateRotating(this, CubeStateID.CubeStateRotating);

        _cubeStates = new Dictionary<CubeStateID, CubeState>();
        _cubeStates.Add(cubeStateFolded.CubeStateID, cubeStateFolded);
        _cubeStates.Add(cubeStateLaymap.CubeStateID, cubeStateLaymap);
        _cubeStates.Add(cubeStateFoldingTransition.CubeStateID, cubeStateFoldingTransition);
        _cubeStates.Add(cubeStateRotating.CubeStateID, cubeStateRotating);

        _currentCubeState = _cubeStates[0];
    }

    private void CubeRotationStarted(object sender, EventTextArgs args)
    {
        if (_currentCubeState.CubeStateID != CubeStateID.CubeStateRotating)
        {
            ChangeState(CubeStateID.CubeStateRotating, false);
        }
    }

    private void CubeRotationComplete(object sender, EventTextArgs args)
    {
        if (_currentCubeState.CubeStateID == CubeStateID.CubeStateRotating)
        {
            ChangeState(CubeStateID.CubeStateFolded, false);
        }
    }

    public void RevertConnection()
    {   
        if (_connections.Count > 0 && _currentCubeState.CubeStateID == CubeStateID.CubeStateFolded)
        {
            GridFieldConnection lastConnection = _connections[_connections.Count - 1];
            bool unrestrictedRevertRotation = lastConnection.Target.ConnectionState == GridFieldConnectionState.PortalConnection ? false : true;

            Debug.Log(lastConnection.ValidGridFields.Count);
            for (int i = lastConnection.ValidGridFields.Count-1; i > 0; i--) //don't include the origin gridfield
            {
                lastConnection.ValidGridFields[i].SetConnectionState(lastConnection.ValidGridFields[i].GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.Empty));
            }
            _connections.RemoveAt(_connections.Count - 1);

            if (_connections.Count > 0)
            {
                CubeSide originCubeSide = _selectedGridField.CubeSide;  
                lastConnection = _connections[_connections.Count - 1];
                SetSelectedGridField(lastConnection.ValidGridFields[lastConnection.ValidGridFields.Count - 1]);
                _selectedGridField.SetSelected(true, true);

                CubeGameplay.Instance.CubeRotation.RotateTowards(originCubeSide, _selectedGridField.CubeSide, unrestrictedRevertRotation);
                CubeGameplay.Instance.CubeCameraRotation.RevertToInitialRotation();
            }
            else
            {
                CubeSide originCubeSide = _selectedGridField.CubeSide;
                Debug.Log(lastConnection.Target.ConnectionState);
                lastConnection.Origin.SetConnectionState(lastConnection.Origin.GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.Empty));
                lastConnection.Origin.SetSelected(false, true);
                _selectedGridField = null;

                CubeGameplay.Instance.CubeRotation.RotateTowards(originCubeSide, CubeSide.Front, unrestrictedRevertRotation);
                CubeGameplay.Instance.CubeCameraRotation.RevertToInitialRotation();
            }

            PlayManager.Instance.SetQtyConnections(_connections.Count);
        }
    }

    public void SetSelectedGridField(GridField gridField)
    {
        _selectedGridField = gridField;
    }

    public void RevertAllConnections()
    {
        foreach (var gridFieldConnection in _connections)
        {
            foreach (var gridField in gridFieldConnection.ValidGridFields)
            {
                gridField.SetConnectionState(gridField.GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.Empty));
                gridField.SetSelected(false, true);
            }
        }
        _selectedGridField = null;
    }

    /// <summary>  
    /// Initialize the cube map, which is a 2D-layplan representation of the cube and its sides
    /// </summary>
    private void InitializeCubeMap(LevelData levelData)
    {
        _gridSize = levelData.GridSize;
        _cubeMap = new GridField[_gridSize * 3, _gridSize * 4];
        _necessaryConnectionsToWin = 0;

        for (int x = 0; x < _gridSize * 3; x++)
        {
            for (int y = 0; y < _gridSize * 4; y++)
            {
                List<PossibleConnectionDirection> possibleConnections = levelData.CubeMap[x + (y * _gridSize * 3)].PossibleConnectionDirections != null? levelData.CubeMap[x + (y * _gridSize * 3)].PossibleConnectionDirections.ToList() : null;
                _cubeMap[x, y] = GridFactory.CreateGridField(new IntVector2(x, y), levelData.CubeMap[x + (y * _gridSize * 3)].GridFieldType, levelData.CubeMap[x + (y * _gridSize * 3)].GridFieldColor, levelData.CubeMap[x + (y * _gridSize * 3)].RequiredConnections, possibleConnections);
                _necessaryConnectionsToWin += _cubeMap[x, y].RequiredConnections();
            }
        }

        _necessaryConnectionsToWin--; //is always one less
        PlayManager.Instance.SetQtyConnections(0);
        PlayManager.Instance.SetMaxQtyConnections(_necessaryConnectionsToWin);
    }

    public void ReinitializeCubeMapEmpty(uint gridSize)
    {
        DestroyAllGridFields();
        _gridSize = gridSize;
        _cubeMap = new GridField[_gridSize * 3, _gridSize * 4];
        _necessaryConnectionsToWin = 0;

        for (int x = 0; x < _cubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < _cubeMap.GetLength(1); y++)
            {
                _cubeMap[x, y] = GridFactory.CreateGridField(new IntVector2(x, y), GridFieldType.EmptyGridField);
                _necessaryConnectionsToWin += _cubeMap[x, y].RequiredConnections();
            }
        }

        _necessaryConnectionsToWin--; //is alway one less
        PlayManager.Instance.SetMaxQtyConnections(_necessaryConnectionsToWin);
    }

    public void SetGridField(GridField gridField, int x, int y)
    {
        if (x >= 0 && x < _cubeMap.GetLength(0) && y >= 0 && y < _cubeMap.GetLength(1))
        {
            _necessaryConnectionsToWin -= _cubeMap[x, y].RequiredConnections();
            _cubeMap[x, y] = gridField;
            _necessaryConnectionsToWin += _cubeMap[x, y].RequiredConnections();
        }
    }

    public void DestroyAllGridFields()
    {
        for (int x = 0; x < _cubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < _cubeMap.GetLength(1); y++)
            {
                Destroy(_cubeMap[x, y].gameObject);
            }
        }
    }

    public void SetCurrentActiveSide(CubeSide side, Quaternion rotation)
    {
        _currentActiveSide.CubeSide = side;
        _currentActiveSide.Rotation = rotation;
    }

    public void ChangeState(CubeStateID stateID, bool executePreAndPostMethods = true)
    {
        if (stateID == _currentCubeState.CubeStateID || _cubeStates.ContainsKey(stateID) == false)
            return;

        if (executePreAndPostMethods == true)
        {
            _currentCubeState.DoBeforeLeaving();
        }

        _currentCubeState = _cubeStates[stateID];

        if (executePreAndPostMethods == true)
        {
            _currentCubeState.DoBeforeEntering();
        }
    }

    /// <summary>  
    /// Registeres a clicked grid field as the selected grid field. If there is already a selected grid field, the function will check if a connection between
    /// the grid fields is possible
    /// </summary>
    public void RegisterClickedGridField(GridField gridField)
    {
        _currentCubeState.RegisterClickedGridField(gridField);
    }

    public GridField[,] CloneCubeMap()
    {
        GridField[,] cubeMapClone = new GridField[_cubeMap.GetLength(0), _cubeMap.GetLength(1)];
        for (int x = 0; x < cubeMapClone.GetLength(0); x++)
        {
            for (int y = 0; y < cubeMapClone.GetLength(1); y++)
            {
                cubeMapClone[x, y] = _cubeMap[x,y].Clone() as GridField;
            }
        }
        return cubeMapClone;
    }

    #region cube side connections
    /// <summary>  
    /// A dictionary-handled list of all cube-side-connections and their validity
    /// </summary>  
    private static Dictionary<CubeSide, Dictionary<CubeSide, bool>> _validCubeSideConnection = new Dictionary<CubeSide, Dictionary<CubeSide, bool>>()
    {
        {CubeSide.Back, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Front, false},
                {CubeSide.Top, true},
                {CubeSide.Bottom, true},
                {CubeSide.Right, true},
                {CubeSide.Back, true},
            }
        },
        {CubeSide.Bottom, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Front, true},
                {CubeSide.Top, false},
                {CubeSide.Back, true},
                {CubeSide.Right, true},
                {CubeSide.Bottom, true},
            }
        },
        {CubeSide.Left, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Back, true},
                {CubeSide.Front, true},
                {CubeSide.Top, true},
                {CubeSide.Bottom, true},
                {CubeSide.Right, false},
                {CubeSide.Left, true},
            }
        },
        {CubeSide.Right, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, false},
                {CubeSide.Front, true},
                {CubeSide.Top, true},
                {CubeSide.Back, true},
                {CubeSide.Bottom, true},
                {CubeSide.Right, true},
            }
        },
        {CubeSide.Front, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Bottom, true},
                {CubeSide.Top, true},
                {CubeSide.Back, false},
                {CubeSide.Right, true},
                {CubeSide.Front, true},
            }
        },
        {CubeSide.Top, new Dictionary<CubeSide, bool>()
            {
                {CubeSide.Left, true},
                {CubeSide.Front, true},
                {CubeSide.Bottom, false},
                {CubeSide.Back, true},
                {CubeSide.Right, true},
                {CubeSide.Top, true},
            }
        },
    };
    public static Dictionary<CubeSide, Dictionary<CubeSide, bool>> ValidCubeSideConnections
    {
        get
        {
            return _validCubeSideConnection;
        }
    }
    #endregion
}

/// <summary>  
///  Indicates one of the six sides of a cube or an imagenery side, 
/// used to fill up the 2D-represenation of a cube
/// </summary>  
public enum CubeSide
{
    Back,
    Bottom,
    Left,
    Right,
    Front,
    Top,
    Imagenary
}

/// <summary>  
///  Moving direction on the 2D-layplan represantation of the cube
/// </summary> 
public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    None,
}

public enum CubeAxis
{
    X,
    Y
}

public enum CubeStateID
{
    CubeStateFolded,
    CubeStateLaymap,
    CubeStateFoldingTransition,
    CubeStateRotating,
}

/// <summary>  
///  The struct provides the information about whether a connection between two grid fields is valid. It also
///  offers a list containing all the valid grids in the connection (originating from the origin grid field)
/// </summary> 
public struct GridFieldConnection
{
    public bool Valid;
    public List<GridField> ValidGridFields;

    public GridField Origin
    {
        get { return ValidGridFields[0]; }
    }

    public GridField Target
    {
        get { return ValidGridFields[ValidGridFields.Count - 1]; }
    }
}

[Serializable]
public struct CubeSideGameObjects
{
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Front;
    public GameObject Back;
    public GameObject Imagenary;
}

public struct ActiveCubeSide
{
    public CubeSide CubeSide;
    public Quaternion Rotation;
}