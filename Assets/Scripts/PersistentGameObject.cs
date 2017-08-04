using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentGameObject : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
