using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class UserGeneratedLevelData : LevelData
{
    [ProtoMember(1)]
    private string _authorName;
    [ProtoMember(2)]
    private string _authorID;
    [ProtoMember(3)]
    private string _levelID;
    [ProtoMember(5)]
    private int _qtyRatings = 0;
    [ProtoMember(6)]
    private float _userRating = 0;
    [ProtoMember(7)]
    private int _difficulty = 1;
    [ProtoMember(8)]
    private int _qtyDownloads = 0;
    [ProtoMember(9)]
    private long _date;
    [ProtoMember(10)]
    private string _fileLocation;

    public string AuthorName
    {
        get { return _authorName; }
    }
    public string AuthorID
    {
        get { return _authorID; }
    }
    public string LevelID
    {
        get { return _levelID; }
    }
    public int QtyRatings
    {
        get { return _qtyRatings; }
    }
    public float UserRating 
    {
       get { return _userRating; }
    }
    public int Difficulty 
    {
        get { return _difficulty; }
    }
    public int QtyDownloads 
    {
        get { return _qtyDownloads; }
    }
    public long Date
    {
        get { return _date; }
    }
    public uint Rating
    {
        get { return _rating; }
    }
    public string LevelName
    {
        get
        {
            if (_levelName == null) return "";
            return _levelName;
        }
    }

    public string FileLocation
    {
        get { return _fileLocation; }
    }

    private UserGeneratedLevelData() { }

    public UserGeneratedLevelData(uint gridSize) : base(gridSize)
    {
    }

    public void SetLevelID(string levelID)
    {
        _levelID = levelID;
    }

    public void SyncWithLevelInfo(UserGeneratedLevelInfo levelInfo)
    {
        _authorName = levelInfo.AuthorName;
        _authorID = levelInfo.AuthorID;
        _levelID = levelInfo.LevelID;
        _qtyRatings = levelInfo.QtyRatings;
        _userRating = levelInfo.UserRating;
        _difficulty = levelInfo.Difficulty;
        _qtyDownloads = levelInfo.QtyDownloads;
        _fileLocation = levelInfo.FileLocation;
        _date = levelInfo.Date;
    }
}
