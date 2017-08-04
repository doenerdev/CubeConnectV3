using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PortalGridFieldSelectionBehaviour : GridFieldSelectionBehaviour
{

    public override void SetGridFieldSelected(GridField gridField, bool selected, bool revertingConnection = false)
    {
        if (selected == true)
        {
            if (gridField.ConnectionState == GridFieldConnectionState.PortalConnection || revertingConnection == true || Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap)
            {
                gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Selected];
            }
            else
            {
                gridField.GetComponent<Renderer>().material = gridField.GridFieldMaterials[GridFieldMaterial.Connected];

                PortalGridField portalGridField = gridField.GetSpecializedGridField() as PortalGridField;
                if (portalGridField != null)
                {
                    Cube.Instance.CurrentCubeState.RegisterClickedGridField(portalGridField.RelatedPortalGridField);
                }
            }
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
                    case GridFieldConnectionState.PortalConnection:
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
