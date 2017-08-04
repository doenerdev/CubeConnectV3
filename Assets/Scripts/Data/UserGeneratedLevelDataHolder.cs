using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class UserGeneratedLevelDataHolder
{

    private List<UserGeneratedLevelData> _levels;

    public List<UserGeneratedLevelData> Levels
    {
        get
        {
            return _levels;
        }
    }


    public UserGeneratedLevelDataHolder()
    {
        _levels = new List<UserGeneratedLevelData>();
    }
}
