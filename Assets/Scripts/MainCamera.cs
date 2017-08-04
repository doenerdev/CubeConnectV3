using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : Singleton<MainCamera> {

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
