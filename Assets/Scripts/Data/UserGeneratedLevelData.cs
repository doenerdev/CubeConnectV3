using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UserGeneratedLevelData : LevelData
{
    private string _authorName;
    private string _authorID;
    private string _levelID;
    private string _levelDataURL;
    private int _qtyRatings;
    private int _userRating;
    private int _difficulty;
    private int _qtyDownloads;
    private long _date;

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
    public string LevelDataURL
    {
        get { return _levelDataURL; }
    }
    public int QtyRatings
    {
        get { return _qtyRatings; }
    }
    public int UserRating 
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

    public UserGeneratedLevelData(uint gridSize) : base(gridSize)
    {
    }
}
