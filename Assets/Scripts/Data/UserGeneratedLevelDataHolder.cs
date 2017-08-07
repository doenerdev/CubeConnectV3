using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class UserGeneratedLevelDataHolder
{
    [ProtoMember(1)]
    private List<UserGeneratedLevelInfo> _levelInfos; 
    private List<UserGeneratedLevelData> _levels;

    public List<UserGeneratedLevelInfo> LevelInfos
    {
        get { return _levelInfos; }
    } 
    public List<UserGeneratedLevelData> Levels
    {
        get { return _levels; }
    }


    public UserGeneratedLevelDataHolder()
    {
        _levels = new List<UserGeneratedLevelData>();
    }
}

[ProtoContract]
public class UserGeneratedLevelInfoHolder
{
    [ProtoMember(1)]
    private Dictionary<string, UserGeneratedLevelInfo> _levelInfos;

    public Dictionary<string, UserGeneratedLevelInfo> LevelInfos
    {
        get { return _levelInfos; }
    }

    public UserGeneratedLevelInfoHolder()
    {
        _levelInfos = new Dictionary<string, UserGeneratedLevelInfo>();
    }
}
