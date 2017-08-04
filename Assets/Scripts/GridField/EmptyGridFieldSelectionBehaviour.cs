using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmptyGridFieldSelectionBehaviour : GridFieldSelectionBehaviour {

    public override void SetGridFieldSelected(GridField gridField, bool selected, bool revertingConnection = false)
    {
        if (GameManager.Instance.GameState != GameState.LevelEditor || Cube.Instance.CurrentCubeStateID != CubeStateID.CubeStateLaymap)
            return;

        if (selected == true)
        {
            gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Selected];
        }
        else
        {
            if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded)
            {
                switch (gridField.ConnectionState)
                {
                    case GridFieldConnectionState.Empty:
                        gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Default];
                        break;
                    case GridFieldConnectionState.SimpleConnection:
                        gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Connected];
                        break;
                }
            }
            else
            {
                gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Default];
            }

        }
    }
}
