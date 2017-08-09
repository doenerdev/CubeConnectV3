using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopDownloadedLevelBrowserButton : MonoBehaviour {

    public void Clicked()
    {
        GameManager.Instance.ShowWorkshopDownloadedLevelBrowser();
    }
}
