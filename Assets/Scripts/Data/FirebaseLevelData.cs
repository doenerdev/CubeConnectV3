using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class FirebaseLevelData : MonoBehaviour {

    public string AuthorName;
    public string AuthorID;
    public string LevelID;
    public int QtyRatings;
    public float UserRating;
    public int Difficulty;
    public string LevelDataURL;

    public FirebaseLevelData(UserGeneratedLevelData levelData, string dataURL)
    {
        AuthorName = levelData.AuthorName;
        AuthorID = levelData.AuthorID;
        LevelID = levelData.LevelID;
        QtyRatings = levelData.QtyRatings;
        UserRating = levelData.UserRating;
        Difficulty = levelData.Difficulty;
        LevelDataURL = dataURL;
    }
}
