using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWorkshopButton : MonoBehaviour {

    public void Clicked()
    {
        GameManager.Instance.ShowWorkshop();
    }
}
