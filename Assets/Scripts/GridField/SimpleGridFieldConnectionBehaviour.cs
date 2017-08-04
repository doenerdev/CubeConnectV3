using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SimpleGridFieldConnectionBehaviour : GridFieldConnectionBehaviour
{
    //the connection state added to grid fields by this connection type
    private GridFieldConnectionState _accordinGridFieldConnectionState = GridFieldConnectionState.SimpleConnection;

    /// <summary>  
    /// Constructs and returns the appropriate GridFieldConnection between two grid fields, indicating whether the connection is valid and holding all the grid fields used for the connection
    /// </summary>
    public override GridFieldConnection GetGridFieldConnection(GridField gridFieldOrigin, GridField gridFieldTarget, bool playabilityTesting = false)
    {
        GridFieldConnection falseGridFieldConnection = new GridFieldConnection();
        falseGridFieldConnection.Valid = false;

        //first, check if the target grid field is a valid target
        if (IsValidTarget(gridFieldTarget, gridFieldOrigin) == false || DoColorsMatchValid(gridFieldOrigin, gridFieldTarget) == false)
        {
            return falseGridFieldConnection;
        }

        //check if the cube side connection is valid and if the gridFields are on the same or else return early
        if (Cube.ValidCubeSideConnections[gridFieldOrigin._cubeSide][gridFieldTarget._cubeSide] == false)
        {
            return falseGridFieldConnection;
        }
        else
        {
            //first check for a simple connection without an imagenary grid field
            GridFieldConnection simpleConnection = CanConnectGridFields(gridFieldOrigin, gridFieldTarget, playabilityTesting);
            if (simpleConnection.Valid == true)
            {
                for (int i = 0; i < simpleConnection.ValidGridFields.Count; i++)
                {
                    simpleConnection.ValidGridFields[i].SetConnectionState(simpleConnection.ValidGridFields[i].GetAppropriateTargetGridFieldConnectionState(_accordinGridFieldConnectionState), playabilityTesting);
                }
                return simpleConnection;
            }

            //check for a connection via an imagenary grid field
            GridFieldConnection imageneryConnection = CanConnetGridFieldsViaImagenary(gridFieldOrigin, gridFieldTarget, playabilityTesting);
            if (imageneryConnection.Valid == true)
            {
                for (int i = 0; i < imageneryConnection.ValidGridFields.Count; i++)
                {
                    imageneryConnection.ValidGridFields[i].SetConnectionState(imageneryConnection.ValidGridFields[i].GetAppropriateTargetGridFieldConnectionState(_accordinGridFieldConnectionState), playabilityTesting);
                }
                return imageneryConnection;
            }
        }
    
        return falseGridFieldConnection;
    }


    #region valid connection and target dictionaries
    


    #endregion
}
