using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GridFieldSelectionBehaviour
{

    public abstract void SetGridFieldSelected(GridField gridField, bool selected, bool revertingConneciton = false);
}
