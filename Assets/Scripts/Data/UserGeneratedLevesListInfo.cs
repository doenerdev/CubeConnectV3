using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UserGeneratedLevesListInfo
{
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

public class UserGeneratedLevelInfo
{
    public string AuthorName;
    public string AuthorID;
    public string LevelID;
    public string LevelDataURL;
    public string LevelCode;
    public int QtyRatings;
    public int UserRating;
    public int Difficulty;
    public int QtyDownloads;
    public long Date;
    public string FileLocation;
}