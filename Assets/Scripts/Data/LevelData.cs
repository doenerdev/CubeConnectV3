using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[Serializable]
public class LevelData
{

    protected uint _gridSize;
    protected CubeMapGridInfo[,] _cubeMap;
    protected string _levelName;
    protected LevelStatus _levelStatus;
    protected uint _rating;

    public uint GridSize
    {
        get
        {
            return _gridSize;
        }
    }
    public CubeMapGridInfo[,] CubeMap
    {
        set
        {
            if (value.GetLength(0) == _cubeMap.GetLength(0) && value.GetLength(1) == _cubeMap.GetLength(1))
                _cubeMap = value;
        }
        get { return _cubeMap; }
    }
    public string LevelName
    {
        get
        {
            if (_levelName == null) return "";
            return _levelName;
        }
    }
    public LevelStatus LevelStatus
    {
        get { return _levelStatus; }
    }
    public uint Rating
    {
        get { return _rating; }
    }

    public void SetLevelName(string levelName)
    {
        _levelName = levelName;
    }

    public void SetRating(uint rating)
    {
        //if (rating > _rating)
            _rating = rating;
    }

    public void SetLevelStatus(LevelStatus status)
    {
        _levelStatus = status;

        if (_levelStatus != LevelStatus.Finished)
            _rating = 0;
    }

    public LevelData(uint gridSize)
    {
        _gridSize = gridSize;

        _cubeMap = new CubeMapGridInfo[_gridSize * 3, _gridSize * 4];
        _levelName = "unnamed "; //+ _orderIndex;

        for (int x = 0; x < _cubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < _cubeMap.GetLength(1); y++)
            {         
                _cubeMap[x, y] = new CubeMapGridInfo(GridFieldType.EmptyGridField, GridFieldColor.Blue, 0, null);
            }
        }
    }

    /*public void SetGridFieldDetails(int x, int y, GridFieldType type, GridFieldColor color)
    {
        if (x >= 0 && x < _cubeMap.GetLength(0) && y >= 0 && y < _cubeMap.GetLength(1))
        {
            _cubeMap[x, y] = new CubeMapGridInfo(type, color);
        }
    }*/

    public void SetGridSize(uint gridSize)
    {
        _gridSize = gridSize;
        ReinitializeCubeMap();
    }

    protected void ReinitializeCubeMap()
    {
        _cubeMap = new CubeMapGridInfo[_gridSize * 3, _gridSize * 4];
        for (int x = 0; x < _cubeMap.GetLength(0); x++)
        {
            for (int y = 0; y < _cubeMap.GetLength(1); y++)
            {
                _cubeMap[x, y] = new CubeMapGridInfo(GridFieldType.EmptyGridField, GridFieldColor.Blue, 0, null);
            }
        }
    }
}

[Serializable]
public enum LevelStatus
{
    Unlocked,
    Locked,
    Finished
}

[Serializable]
public struct CubeMapGridInfo
{
    public CubeMapGridInfo(GridFieldType type, GridFieldColor color, int requiredConnections, PossibleConnectionDirection[] possibleConnectionDirections)
    {
        GridFieldType = type;
        GridFieldColor = color;
        RequiredConnections = requiredConnections;
        PossibleConnectionDirections = possibleConnectionDirections;
    }

    public GridFieldType GridFieldType;
    public GridFieldColor GridFieldColor;
    public int RequiredConnections;
    public PossibleConnectionDirection[] PossibleConnectionDirections;
}