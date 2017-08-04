using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopDiscoverButton : MonoBehaviour {

    public void Clicked()
    {
        GameManager.Instance.ShowWorkshopLevelBrowser();
    }
}
