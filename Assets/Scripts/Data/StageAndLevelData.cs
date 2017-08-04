using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class StageAndLevelData {

    private List<StageData> _stages;

    public List<StageData> Stages
    {
        get
        {
            return _stages;
        }
    }


    public StageAndLevelData()
    {
        _stages = new List<StageData>();
        //_stages.Add(new StageData());
    }
}
