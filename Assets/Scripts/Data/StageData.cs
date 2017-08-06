using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class StageData {

    [ProtoMember(1)]
    private List<LevelData> _levels;
    [ProtoMember(2)]
    private string _stageName;
    [ProtoMember(3)]
    private int _orderIndex;
    [ProtoMember(4)]
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

    private StageData() { }

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
[ProtoContract]
public enum StageStatus
{
    Unlocked,
    Locked
}
