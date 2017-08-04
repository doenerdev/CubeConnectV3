using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PortalGridField : GridField
{

    private GridFieldSelectionBehaviour _selectionBehaviour = new PortalGridFieldSelectionBehaviour();
    private GridFieldConnectionBehaviour _connectionBehaviour = new PortalGridFieldConnectionBehaviour();
    private int _requiredConnections = 1;
    [SerializeField] private PortalGridField _relatedPortalGridField;

    public GridFieldSelectionBehaviour Behave
    {
        get { return _selectionBehaviour;}
    }
    public PortalGridField RelatedPortalGridField
    {
        get { return _relatedPortalGridField;}
        set { _relatedPortalGridField = value; }
    }

    public override Object GetSpecializedGridField()
    {
        return this as Object;
        return gameObject.GetComponent<PortalGridField>();
    }

    public override void InitializeGridField(Cube parentCube, IntVector2 gridPositon, GridFieldType gridFieldType, GridFieldColor color = GridFieldColor.None)
    {
        base.InitializeGridField(parentCube, gridPositon, gridFieldType, color);
        if (color == GridFieldColor.None)
        {
            _gridFieldColor = GridFieldColor.Blue;
        }
    }

    /// <summary>  
    /// Returns the appropriate grid field connection to a target grid field
    /// </summary>
    public override GridFieldConnection GetGridFieldConnection(GridField gridFieldTarget, bool playabilityTesting = false)
    {
        return GetGridFieldConnectionBehaviour().GetGridFieldConnection(this, gridFieldTarget, playabilityTesting);
    }

    protected override void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false || GameManager.Instance.GameState == GameState.CubeGameplay)
        {
            _parentCube.RegisterClickedGridField(this);
        }
    }

    protected override void OnMouseOver()
    {
        //Debug.Log("Mouse over" + _gridPosition);
    }

    protected override void LoadMaterials()
    {
        _gridFieldMaterials = new Dictionary<GridFieldMaterial, Material>();

        switch (_gridFieldColor)
        {
            case GridFieldColor.Blue:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridPortalBlue") as Material);
                break;
            case GridFieldColor.Red:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridPortalRed") as Material);
                break;
            case GridFieldColor.Multi:
                _gridFieldMaterials.Add(GridFieldMaterial.Default, Resources.Load("Materials/GridPortalMulti") as Material);
                break;
        }

        _gridFieldMaterials.Add(GridFieldMaterial.Connected, Resources.Load("Materials/Connected") as Material);
        _gridFieldMaterials.Add(GridFieldMaterial.Selected, Resources.Load("Materials/Selected") as Material);
    }

    public override int RequiredConnections()
    {
        return _requiredConnections;
    }

    protected override GridFieldConnectionBehaviour GetGridFieldConnectionBehaviour()
    {
        return _connectionBehaviour;
    }

    protected override GridFieldSelectionBehaviour GetGridFieldSelectionBehaviour()
    {
        return _selectionBehaviour;
    }
}
