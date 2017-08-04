using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGridFieldConnectionBehaviour : SimpleGridFieldConnectionBehaviour {

    protected override bool IsValidTarget(GridField targetGridField, GridField originGridField = null)
    {
        if (originGridField.RequiredConnections() <= originGridField.QtyConnections) //don't allow MultiGridFields as an origin grid field after it's required connections criteria is already met
        {
            return false;
        }

        return base.IsValidTarget(targetGridField, originGridField);
    }
}
