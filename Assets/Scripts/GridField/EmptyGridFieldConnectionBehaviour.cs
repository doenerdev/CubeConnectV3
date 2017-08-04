using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EmptyGridFieldConnectionBehaviour : GridFieldConnectionBehaviour {

    /// <summary>  
    /// Constructs and returns the appropriate GridFieldConnection between two grid fields, indicating whether the connection is valid and holding all the grid fields used for the connection
    /// </summary>
    public override GridFieldConnection GetGridFieldConnection(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        return new GridFieldConnection();
    }
}
