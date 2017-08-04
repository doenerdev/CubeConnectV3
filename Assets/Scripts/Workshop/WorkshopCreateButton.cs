using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopCreateButton : MonoBehaviour {

    public void Clicked()
    {
        GameManager.Instance.ShowWorkshopLevelEditor();
    }
}
