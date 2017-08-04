using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LevelEditor : MonoBehaviour
{
    protected LevelData _currentLevelData;

    [SerializeField] protected GameObject _levelDetailControls;
    [SerializeField] protected LevelEditorGridFieldSelection _gridFieldSelection;

    public LevelData CurrentLevelData
    {
        get { return _currentLevelData;}
    }

    private void PlayabilityValidationComplete(object sender, EventTextArgs args)
    {
        Debug.Log("Validation Complete. Valid: " + LevelEditorLevelPlayabilityValidator.PlayabilityValid);
        LevelEditorLevelPlayabilityValidator.PrintPossibleSolutionsToConsole();

        LevelEditorLevelPlayabilityValidator.LevelPlayabilityValidationCompleted -= new EventHandler<EventTextArgs>(PlayabilityValidationComplete);
    }

    public void Update()
    {
        if (Input.GetKeyUp("t"))
        {
            Debug.Log("Run Validation");
            LevelEditorLevelPlayabilityValidator.LevelPlayabilityValidationCompleted += new EventHandler<EventTextArgs>(PlayabilityValidationComplete);
            LevelEditorLevelPlayabilityValidator.ValidateCubeMapPlayability();
        }
        else if (Input.GetKeyUp("z"))
        {
            Debug.Log("Find all Solutions");
            LevelEditorLevelPlayabilityValidator.LevelPlayabilityValidationCompleted += new EventHandler<EventTextArgs>(PlayabilityValidationComplete);
            LevelEditorLevelPlayabilityValidator.ValidateCubeMapPlayability(true);
        }
    }


    public void SetGridField(GridFieldType type, GridFieldColor color = GridFieldColor.None, int requiredConnections = -1, List<PossibleConnectionDirection> possibleConnectionDirections = null)
    {
        if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded)
            return;

        GridField selected = Cube.Instance.SelectedGridField;
        if (selected == null)
            return;

        GridField lastSetPortalClone_baseType = GridFactory.LastSetPortal != null ? GridFactory.LastSetPortal.Clone() as GridField : null; //copy and preserve the grid factory's lastSetPortal because it will be altered because of the CreateGridField-Function call
        Destroy(Cube.Instance.CubeMap[selected.GridPosition.x, selected.GridPosition.y].gameObject);
        GridField gridField = GridFactory.CreateGridField(new IntVector2(selected.GridPosition.x, selected.GridPosition.y), type, color, requiredConnections, possibleConnectionDirections);
        Cube.Instance.SetGridField(gridField, selected.GridPosition.x, selected.GridPosition.y);
        PossibleConnectionDirection[] connectionDirections = possibleConnectionDirections != null ? possibleConnectionDirections.ToArray(): null;
        _currentLevelData.CubeMap[selected.GridPosition.x, selected.GridPosition.y] = new CubeMapGridInfo(type, color, requiredConnections, connectionDirections);

      
        GridField playabilityValidatorGridField = gridField.Clone() as GridField;
        LevelEditorLevelPlayabilityValidator.SetGridField(playabilityValidatorGridField, selected.GridPosition.x, selected.GridPosition.y);

        if (type == GridFieldType.PortalGridField)
        {
            if (GridFactory.LastSetPortal == null)
            {
                _gridFieldSelection.EnableAllGridFieldButtons();
            }
            else
            {
                _gridFieldSelection.DisableGridFieldButtonsByTypeExcept(GridFieldType.PortalGridField);
            }
            
            if(lastSetPortalClone_baseType != null) //set the relatedPortal field for the portal field that was added before (to the LevelEditorLevelPlayabilityValidator-CubeMap)
            {
                PortalGridField lastSetPortalClone = lastSetPortalClone_baseType.GetSpecializedGridField() as PortalGridField;
                lastSetPortalClone.RelatedPortalGridField = playabilityValidatorGridField.GetSpecializedGridField() as PortalGridField;
                LevelEditorLevelPlayabilityValidator.SetGridField(lastSetPortalClone, lastSetPortalClone.GridPosition.x, lastSetPortalClone.GridPosition.y);
            }
        }

        //LevelEditorLevelPlayabilityValidator.Instance.ValidateCubeMapPlayability();
    }

    public virtual void ShowCubeLevel(LevelData levelData)
    {
        GameObject cubeGameplayWrapper = new GameObject();
        cubeGameplayWrapper.AddComponent<CubeGameplay>();
        Cube.Create(levelData);
        Cube.Instance.gameObject.SetActive(true);
        _levelDetailControls.SetActive(true);
        Camera.main.GetComponent<CameraRotation>().enabled = true;
        _currentLevelData = levelData;
    }

    public virtual void HideCubeLevel()
    {
        Camera.main.GetComponent<CameraRotation>().enabled = false;
        if (Cube.Instance != null)
        {
            Destroy(Cube.Instance.gameObject);
        }
    }

    public virtual void ToggleGridFieldSelection()
    {
        _gridFieldSelection.ToggleCanvas();
    }

    public abstract void SaveLevel();

}
