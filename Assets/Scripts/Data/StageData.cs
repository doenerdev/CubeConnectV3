using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[Serializable]
public class StageData {

    private List<LevelData> _levels;
    private string _stageName;
    private int _orderIndex;
    private StageStatus _stageStatus;

    public List<LevelData> Levels
    {
        get
        {
            return _levels;
        }
    }
    public string StageName
    {
        get { return _stageName; }
    }
    public StageStatus StageStatus
    {
        get { return _stageStatus; }
    }

    
    public StageData(int index)
    {
        _levels = new List<LevelData>();
        _orderIndex = index;
        _stageName = "unnamed " + _orderIndex;
    }

    public void SetStageName(string stageName)
    {
        _stageName = stageName;
    }

    public LevelData GetLevelDataByIndex(int index)
    {
        if (index >= 0 && index < _levels.Count)
            return _levels[index];

        return null;
    }
}

[Serializable]
public enum StageStatus
{
    Unlocked,
    Locked
}
