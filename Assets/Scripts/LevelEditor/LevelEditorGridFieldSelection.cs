using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TagFrenzy;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorGridFieldSelection : MonoBehaviour
{

    private Canvas _gridFieldSelectionCanvas;
    private List<CreateGridFieldButton> _gridFieldButtons; 

    private void Awake()
    {
        _gridFieldSelectionCanvas = GetComponent<Canvas>();
        _gridFieldButtons = gameObject.GetComponentsInChildren<CreateGridFieldButton>().ToList(); 
    }

    public void DisableGridFieldButtonsByTypeExcept(GridFieldType type)
    {
        foreach (var gridFieldButton in _gridFieldButtons)
        {
            if (gridFieldButton.GridFieldType != type)
            {
                gridFieldButton.Selectable(false);
            }
        }
    }

    public void EnableAllGridFieldButtons()
    {
        foreach (var gridFieldButton in _gridFieldButtons)
        {
            gridFieldButton.Selectable(true);
        }
    }

    public void ToggleCanvas()
    {
        _gridFieldSelectionCanvas.enabled = !_gridFieldSelectionCanvas.enabled;         
    }
}
