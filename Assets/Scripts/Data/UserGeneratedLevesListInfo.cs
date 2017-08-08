using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class UserGeneratedLevesListInfo
{
    [ProtoMember(1)]
    private List<UserGeneratedLevelInfo> _userGeneratedLevelInfos;

    public List<UserGeneratedLevelInfo> UserGeneratedLevelInfos
    {
        get { return _userGeneratedLevelInfos; }
    }

    public UserGeneratedLevesListInfo()
    {
        _userGeneratedLevelInfos = new List<UserGeneratedLevelInfo>();
    }
}

[ProtoContract]
public class UserGeneratedLevelInfo
{
    [ProtoMember(1)]
    public string AuthorName;
    [ProtoMember(2)]
    public string AuthorID;
    [ProtoMember(3)]
    public string LevelID;
    [ProtoMember(4)]
    public string LevelDataURL;
    [ProtoMember(5)]
    public string LevelCode;
    [ProtoMember(6)]
    public int QtyRatings;
    [ProtoMember(7)]
    public float UserRating;
    [ProtoMember(8)]
    public int Difficulty;
    [ProtoMember(9)]
    public int QtyDownloads;
    [ProtoMember(10)]
    public long Date;
    [ProtoMember(11)]
    public string FileLocation;
    [ProtoMember(12)]
    public string LevelName;
}