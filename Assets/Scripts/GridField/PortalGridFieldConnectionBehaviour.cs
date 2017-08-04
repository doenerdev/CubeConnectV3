using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PortalGridFieldConnectionBehaviour : GridFieldConnectionBehaviour
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
        bool isPortalConnection = IsPortalConnection(gridFieldOrigin);

        //first, check if the target grid field is a valid target
        if (IsValidTarget(gridFieldTarget, gridFieldOrigin) == false || (isPortalConnection == false && DoColorsMatchValid(gridFieldOrigin, gridFieldTarget) == false))
        {
            return falseGridFieldConnection;
        }

        //check if the desired connection is a portal connection (from portal to portal) or a default connection (outgoing from an already connected portal)
        if (isPortalConnection == true)
        {
            GridFieldConnection portalConnection = new GridFieldConnection();
            portalConnection.Valid = true;
            portalConnection.ValidGridFields = new List<GridField>();
            portalConnection.ValidGridFields.Add(gridFieldOrigin);
            portalConnection.ValidGridFields.Add(gridFieldTarget);
            if (gridFieldOrigin.ConnectionState == GridFieldConnectionState.Empty) //if the portal field is the first field selected (overall)
            {
                gridFieldOrigin.SetConnectionState(gridFieldTarget.GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.SimpleConnection), playabilityTesting);
            }
            gridFieldTarget.SetConnectionState(gridFieldTarget.GetAppropriateTargetGridFieldConnectionState(GridFieldConnectionState.PortalConnection), playabilityTesting);
            return portalConnection;
        }
        else
        {
            if (Cube.ValidCubeSideConnections[gridFieldOrigin._cubeSide][gridFieldTarget._cubeSide] == false)
            {
                return falseGridFieldConnection;
            }

            //first check for a simple connection without an imagenary grid field
            GridFieldConnection simpleConnection = CanConnectGridFields(gridFieldOrigin, gridFieldTarget, playabilityTesting);
            if (simpleConnection.Valid == true)
            {
                for (int i = 1; i < simpleConnection.ValidGridFields.Count; i++) //don't include the portal (it must keep the portalconnection status)
                {
                    simpleConnection.ValidGridFields[i].SetConnectionState(simpleConnection.ValidGridFields[i].GetAppropriateTargetGridFieldConnectionState(_accordinGridFieldConnectionState), playabilityTesting);
                }
                return simpleConnection;
            }

            //check for a connection via an imagenary grid field
            GridFieldConnection imageneryConnection = CanConnetGridFieldsViaImagenary(gridFieldOrigin, gridFieldTarget, playabilityTesting);
            if (imageneryConnection.Valid == true)
            {
                for (int i = 1; i < imageneryConnection.ValidGridFields.Count; i++) //don't include the portal (it must keep the portalconnection status)
                {
                    imageneryConnection.ValidGridFields[i].SetConnectionState(imageneryConnection.ValidGridFields[i].GetAppropriateTargetGridFieldConnectionState(_accordinGridFieldConnectionState), playabilityTesting);
                }
                return imageneryConnection;
            }
        }

        return falseGridFieldConnection;
    }

    /// <summary>  
    /// Checks if the desired connection is a portal connection (from portal to portal) or a default connection (outgoing from an already connected portal)
    /// </summary>
    private bool IsPortalConnection(GridField gridFieldOrigin)
    {
        return gridFieldOrigin.ConnectionState != GridFieldConnectionState.PortalConnection || gridFieldOrigin.ConnectionState == GridFieldConnectionState.Empty;
    }

    protected override bool IsValidTarget(GridField targetGridField, GridField gridFieldOrigin)
    {
        if (gridFieldOrigin == null)
        {
            return false;
        }

        if (gridFieldOrigin.ConnectionState != GridFieldConnectionState.PortalConnection || gridFieldOrigin.ConnectionState == GridFieldConnectionState.Empty)
        {
            PortalGridField portalGridFieldOrigin = gridFieldOrigin.GetSpecializedGridField() as PortalGridField;
            PortalGridField portalGridFieldTarget = targetGridField.GetSpecializedGridField() as PortalGridField;

            //check if the targetGridField is the related portal for the origin grid field
            if (portalGridFieldOrigin == null || portalGridFieldOrigin.RelatedPortalGridField != targetGridField ||
                portalGridFieldTarget.RelatedPortalGridField != gridFieldOrigin)
            {
                return false;
            }

            return ValidTargetGridFieldTypesForTeleport.ContainsKey(targetGridField.GridFieldType) && ValidTargetGridFieldTypesForTeleport[targetGridField.GridFieldType] &&
            ValidTargetGridFieldConnetionStates.ContainsKey(targetGridField.ConnectionState) && ValidTargetGridFieldConnetionStates[targetGridField.ConnectionState];
        }
        else
        {
            return base.IsValidTarget(targetGridField, gridFieldOrigin);
        }
    }


    #region valid connection and target dictionaries
    protected Dictionary<GridFieldType, bool> _validTargetGridFieldTypesForTeleport = new Dictionary<GridFieldType, bool>()
    {
        { GridFieldType.EmptyGridField, false},
        { GridFieldType.BarrierGridField, false},
        { GridFieldType.SimpleGridField, false},
        { GridFieldType.PortalGridField, true},
        { GridFieldType.MultiGridField, false},
        { GridFieldType.MultiDirectionsGridField, false},
    };
    public Dictionary<GridFieldType, bool> ValidTargetGridFieldTypesForTeleport
    {
        get { return _validTargetGridFieldTypesForTeleport; }
    }

    protected Dictionary<GridFieldType, bool> _validTargetGridFieldTypes = new Dictionary<GridFieldType, bool>()
    {
        { GridFieldType.EmptyGridField, false},
        { GridFieldType.BarrierGridField, false},
        { GridFieldType.SimpleGridField, true},
        { GridFieldType.PortalGridField, false},
        { GridFieldType.MultiGridField, true},
        { GridFieldType.MultiDirectionsGridField, true},
    };
    public override Dictionary<GridFieldType, bool> ValidTargetGridFieldTypes
    {
        get { return _validTargetGridFieldTypes; }
    }

    protected Dictionary<GridFieldType, bool> _validConnectionGridFieldTypes = new Dictionary<GridFieldType, bool>()
    {
        { GridFieldType.EmptyGridField, true},
        { GridFieldType.BarrierGridField, false},
        { GridFieldType.SimpleGridField, false},
        { GridFieldType.PortalGridField, false},
        { GridFieldType.MultiGridField, false},
        { GridFieldType.MultiDirectionsGridField, false},
    };
    public override Dictionary<GridFieldType, bool> ValidConnectionGridFieldTypes
    {
        get { return _validConnectionGridFieldTypes; }
    }
    #endregion
}
