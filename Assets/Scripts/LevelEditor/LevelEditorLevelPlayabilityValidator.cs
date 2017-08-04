using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


public static class LevelEditorLevelPlayabilityValidator {

    private static GridField[,] _validatorCubeMap;
    private static GridField _selectedGridField;
    private static BackgroundWorker _asyncPlayabilityValidator;
    private static List<GridField>[,] _possibleLevelSolutions;
    private static List<GridField[,]> _solutionTriesLog; 
    private static List<PlayabilityValidationResult> _playabilityValidationResults;

    public static bool PlayabilityValid
    {
        get
        {
            if (_playabilityValidationResults.Count(item => item.Valid == true) > 0)
            {
                return true;
            }
            return false;
        }
    }
    public static List<PlayabilityValidationResult> PlayabilityValidationResults
    {
        get { return _playabilityValidationResults; }
    }
    public static GridField[,] ValidatorCubeMap
    {
        get
        {
            if (_validatorCubeMap == null && Cube.Instance != null && Cube.Instance.CubeMap != null)
            {
                _validatorCubeMap = Cube.Instance.CloneCubeMap();
            }
            return _validatorCubeMap;
        }
        set { _validatorCubeMap = value; }
    }

    static LevelEditorLevelPlayabilityValidator()
    {
        InitializeBackgroundWorker();
    }

    public static void PrintPossibleSolutionsToConsole()
    {
        for (int i = 0; i < PlayabilityValidationResults.Count; i++)
        {
            Debug.Log(PlayabilityValidationResults[i].Valid + "Solution " + (i+1) + ":");
            for (int j = 0; j < PlayabilityValidationResults[i].LevelSolution.Count; j++)
            {
                if (j == PlayabilityValidationResults[i].LevelSolution.Count - 1)
                {
                    Debug.Log(PlayabilityValidationResults[i].LevelSolution[j] + " | ConnectionState:" + PlayabilityValidationResults[i].LevelSolution[j].ConnectionState + 
                        " | QtyConnections:" + PlayabilityValidationResults[i].LevelSolution[j].QtyConnections);
                }
                else
                {
                    Debug.Log(PlayabilityValidationResults[i].LevelSolution[j] + " | ConnectionState:" + PlayabilityValidationResults[i].LevelSolution[j].ConnectionState +
                        " | QtyConnections:" + PlayabilityValidationResults[i].LevelSolution[j].QtyConnections  + " --> ");
                }
            }
            Debug.Log("-----------------------------------");
        }
    }

    private static void InitializeBackgroundWorker()
    {
        _asyncPlayabilityValidator = new BackgroundWorker();
        _asyncPlayabilityValidator.WorkerReportsProgress = true;
        _asyncPlayabilityValidator.WorkerSupportsCancellation = true;
        _asyncPlayabilityValidator.ProgressChanged += new ProgressChangedEventHandler(ValidationProgressChanged);
        _asyncPlayabilityValidator.DoWork += (obj, e) => ValidateCubeMapPlayabilityAsync(obj, e);
        _asyncPlayabilityValidator.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ValidationCompleted);
    }

    private static void ValidationCompleted(object sender, RunWorkerCompletedEventArgs args)
    {
        RaiseLevelPlayabilityValidationCompleted("Validation Complete");
    }

    private static void ValidationProgressChanged(object sender, ProgressChangedEventArgs args)
    {
        Debug.Log("Validation Progress:" + args.ProgressPercentage + "%");
    }

    public static void SetGridField(GridField gridField, int x, int y)
    {
        if (x >= 0 && x < ValidatorCubeMap.GetLength(0) && y >= 0 && y < ValidatorCubeMap.GetLength(1))
        {
            ValidatorCubeMap[x, y] = gridField;
        }
    }

    public static void ValidateCubeMapPlayability(bool findAllPossibleSolutions = false)
    {
        _asyncPlayabilityValidator.CancelAsync();
        _asyncPlayabilityValidator.RunWorkerAsync(findAllPossibleSolutions);
        Debug.Log("Starting Validation");
    }

    private static void ValidateCubeMapPlayabilityAsync(object sender, DoWorkEventArgs args)
    {
        List<PlayabilityValidatorGridFieldInfo> connectableGridFields = GetAndResetConnectableEmptyGridFields();
        _playabilityValidationResults = new List<PlayabilityValidationResult>();
        if (CheckPortalGridFieldsValidity(connectableGridFields.Select(item => item.GridField).ToArray()) == false) //Check if each portal has a target portal
        {
            return;
        }

        for (int i = 0; i < connectableGridFields.Count; i++)
        { 
            List<GridField> levelSolution = new List<GridField>();
            PlayabilityValidationResult validationResult = CheckConnectionsFrom(i, new List<PlayabilityValidatorGridFieldInfo>(connectableGridFields), levelSolution);
            if (validationResult.Valid == true)
            {
                _playabilityValidationResults.Add(validationResult);
            
                if ((bool) args.Argument == false) //early out if only the first possible solution is needed
                {
                    return;
                }
            }      
            ResetCubeMap(); //resets the connections states of the grid fields, important!
            _asyncPlayabilityValidator.ReportProgress((i/ connectableGridFields.Count) * 100);
        }
    }

    private static PlayabilityValidationResult CheckConnectionsFrom(int originIndex, List<PlayabilityValidatorGridFieldInfo> connectableGridFields, List<GridField> levelSolution)
    {
        GridField startGridField = connectableGridFields[originIndex].GridField;
        levelSolution.Add(startGridField);

        if (startGridField.GridFieldType == GridFieldType.PortalGridField)
        {
            switch (connectableGridFields[originIndex].GridFieldConnectionState)
            {
                case GridFieldConnectionState.Empty:
                case GridFieldConnectionState.SimpleConnection:
                    connectableGridFields[originIndex] = new PlayabilityValidatorGridFieldInfo(connectableGridFields[originIndex].GridField, PlayabilityValidatorGridFieldState.Connected, GridFieldConnectionState.SimpleConnection);
                    PortalGridField portalGridField = connectableGridFields[originIndex].GridField.GetSpecializedGridField() as PortalGridField;
                    GridField targetPortalGridField = portalGridField.RelatedPortalGridField;
                    int targetPortalGridFieldIndex = connectableGridFields.FindIndex(item => item.GridField.GridPosition == targetPortalGridField.GridPosition);
                    connectableGridFields[targetPortalGridFieldIndex] = new PlayabilityValidatorGridFieldInfo(connectableGridFields[targetPortalGridFieldIndex].GridField, PlayabilityValidatorGridFieldState.Connected, GridFieldConnectionState.PortalConnection);
                    connectableGridFields[targetPortalGridFieldIndex].GridField.SetConnectionState(GridFieldConnectionState.PortalConnection, true);
                    startGridField = connectableGridFields[targetPortalGridFieldIndex].GridField;
                    levelSolution.Add(startGridField);
                    break;
            }
        }
        else
        {
            //PlayabilityValidatorGridFieldState playabilityState = connectableGridFields[originIndex].GridField.RequiredConnections() <= connectableGridFields[originIndex].GridField.QtyConnections ? PlayabilityValidatorGridFieldState.Connected : PlayabilityValidatorGridFieldState.Default;
            //connectableGridFields[originIndex] = AppropriateStartingGridFieldInfo(connectableGridFields[originIndex]);
        }

        for (int i = 0; i < connectableGridFields.Count; i++)
        {
            if (i == originIndex || connectableGridFields[i].PlayabilityValidatorGridFieldState == PlayabilityValidatorGridFieldState.Connected)
            {
                continue;
            }

            if (startGridField.GetGridFieldConnection(connectableGridFields[i].GridField, true).Valid == true)
            {
                PlayabilityValidatorGridFieldState playabilityStateOrigin = connectableGridFields[originIndex].GridField.RequiredConnections() <= connectableGridFields[originIndex].GridField.QtyConnections ? PlayabilityValidatorGridFieldState.Connected : PlayabilityValidatorGridFieldState.Visited;
                connectableGridFields[originIndex] = new PlayabilityValidatorGridFieldInfo(connectableGridFields[originIndex].GridField, playabilityStateOrigin, connectableGridFields[originIndex].GridFieldConnectionState);
                PlayabilityValidatorGridFieldState playabilityStateTarget = connectableGridFields[i].GridField.RequiredConnections() <= connectableGridFields[i].GridField.QtyConnections ? PlayabilityValidatorGridFieldState.Connected : PlayabilityValidatorGridFieldState.Visited;
                connectableGridFields[i] = new PlayabilityValidatorGridFieldInfo(connectableGridFields[i].GridField, playabilityStateTarget, connectableGridFields[i].GridFieldConnectionState);

                return CheckConnectionsFrom(i, new List<PlayabilityValidatorGridFieldInfo>(connectableGridFields), new List<GridField>(levelSolution));
            }
            else
            {
                //connectableGridFields[i] = new PlayabilityValidatorGridFieldInfo(connectableGridFields[i].GridField, PlayabilityValidatorGridFieldState.Visited, GridFieldConnectionState.Empty);
            }
        }

        if (connectableGridFields.Count(x => x.PlayabilityValidatorGridFieldState == PlayabilityValidatorGridFieldState.Connected) == connectableGridFields.Count)
        {
            return new PlayabilityValidationResult(true, levelSolution);
        }
        return new PlayabilityValidationResult(false, levelSolution);
    }

    private static bool CheckPortalGridFieldsValidity(GridField[] connectableGridFields)
    {
        //Debug.Log("Length:" + connectableGridFields.Length);
        for (int i = 0; i < connectableGridFields.Length; i++)
        {
            //Debug.Log("Type:" + connectableGridFields[i].GridFieldType);
            //Debug.Log("####################");
            if (connectableGridFields[i].GridFieldType != GridFieldType.PortalGridField)
            {
                continue;
            }

            PortalGridField portalGridField = connectableGridFields[i].GetSpecializedGridField() as PortalGridField;
            if (portalGridField == null || portalGridField.RelatedPortalGridField == null)
            {
                return false;
            }
        }
        return true;
    }

    private static void ResetCubeMap()
    {
        for (int x = 0; x < ValidatorCubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < ValidatorCubeMap.GetLength(1); y++)
            {
                ValidatorCubeMap[x, y].SetConnectionState(ValidatorCubeMap[x, y].GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.Empty), true);
            }
        }
    }

    private static List<PlayabilityValidatorGridFieldInfo> GetAndResetConnectableEmptyGridFields()
    {
        List<PlayabilityValidatorGridFieldInfo> connectableGridFields = new List<PlayabilityValidatorGridFieldInfo>();
        for (int x = 0; x < ValidatorCubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < ValidatorCubeMap.GetLength(1); y++)
            {
                ValidatorCubeMap[x, y].SetConnectionState(GridFieldConnectionState.Empty, true);
                if (ValidatorCubeMap[x,y].GridFieldType != GridFieldType.EmptyGridField && ValidatorCubeMap[x, y].GridFieldType != GridFieldType.BarrierGridField)
                {
                    connectableGridFields.Add(new PlayabilityValidatorGridFieldInfo(ValidatorCubeMap[x, y], PlayabilityValidatorGridFieldState.Default, GridFieldConnectionState.Empty));
                }
            }         
        }
        return connectableGridFields;
    }

    private static void RaiseLevelPlayabilityValidationCompleted(string message)
    {
        EventHandler<EventTextArgs> handler = LevelPlayabilityValidationCompleted;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    private static PlayabilityValidatorGridFieldInfo AppropriateStartingGridFieldInfo(PlayabilityValidatorGridFieldInfo gridFieldInfo)
    {
        if (gridFieldInfo.GridField.GridFieldType != GridFieldType.MultiGridField)
        {
            gridFieldInfo.PlayabilityValidatorGridFieldState = PlayabilityValidatorGridFieldState.Connected;
            gridFieldInfo.GridFieldConnectionState = GridFieldConnectionState.SimpleConnection;
            return gridFieldInfo;
        }
        else
        {

            if (gridFieldInfo.GridField.RequiredConnections() <= (gridFieldInfo.GridField.QtyConnections))
            {
                gridFieldInfo.PlayabilityValidatorGridFieldState = PlayabilityValidatorGridFieldState.Connected;
                gridFieldInfo.GridFieldConnectionState = GridFieldConnectionState.ClosedMultiConnection;
                return gridFieldInfo;
            }
            else
            {
                gridFieldInfo.PlayabilityValidatorGridFieldState = PlayabilityValidatorGridFieldState.Default;
                gridFieldInfo.GridFieldConnectionState = GridFieldConnectionState.OpenMultiConnection;
                return gridFieldInfo;
            }
        }
    }

    #region Events
    public static event EventHandler<EventTextArgs> LevelPlayabilityValidationCompleted;
    #endregion Events
}

public enum PlayabilityValidatorGridFieldState
{
    Default,
    Visited,
    Connected
}

public struct PlayabilityValidatorGridFieldInfo
{
    public PlayabilityValidatorGridFieldInfo(GridField gridField, PlayabilityValidatorGridFieldState playabilityValidatorGridFieldState, GridFieldConnectionState gridFieldConnectionState)
    {
        GridField = gridField;
        PlayabilityValidatorGridFieldState = playabilityValidatorGridFieldState;
        GridFieldConnectionState = gridFieldConnectionState;
    }

    public GridField GridField;
    public PlayabilityValidatorGridFieldState PlayabilityValidatorGridFieldState;
    public GridFieldConnectionState GridFieldConnectionState;
}

public struct PlayabilityValidationResult
{
    public PlayabilityValidationResult(bool valid, List<GridField> levelSolution)
    {
        Valid = valid;
        LevelSolution = levelSolution;
    }

    public bool Valid;
    public List<GridField> LevelSolution;
}