using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CameraRotationBehaviour
{
    protected CameraRotation _cameraRotation;

    public CameraRotationBehaviour(CameraRotation cameraRotation)
    {
        _cameraRotation = cameraRotation;
    }

    public abstract void ExecuteRotation();

    public virtual void RotationRevertingStarted() { }

    public static float ClampRotationAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
