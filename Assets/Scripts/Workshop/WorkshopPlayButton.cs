using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopPlayButton : MonoBehaviour {

    public void Clicked()
    {
        GameManager.Instance.ShowWorkshopPlayLevelBrowser();
    }
}
