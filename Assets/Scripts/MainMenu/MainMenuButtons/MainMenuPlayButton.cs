using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayButton : MonoBehaviour {

    public void Clicked()
    {
        Debug.Log("Clicked Play Button");
        GameManager.Instance.ShowStageAndLevelSelection();
    }
}
