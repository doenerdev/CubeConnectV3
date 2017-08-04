using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class KeyboardCameraRotation : CameraRotationBehaviour {

    private Vector2 _rotationSpeedKeyboard = new Vector2(45f, 45f);

    public KeyboardCameraRotation(CameraRotation cameraRotation) : base(cameraRotation) { }

    public override void ExecuteRotation()
    {
        float rotationAngleX = _cameraRotation.CurrentRotationAngleX + Input.GetAxis("Horizontal") * _rotationSpeedKeyboard.x * _cameraRotation.CameraDistance * 0.02f;
        float rotationAngleY = _cameraRotation.CurrentRotationAngleY + Input.GetAxis("Vertical") * _rotationSpeedKeyboard.y * 0.02f;

        rotationAngleX = ClampRotationAngle(rotationAngleX, _cameraRotation.XMinRotation, _cameraRotation.XMaxRotation);
        rotationAngleY = ClampRotationAngle(rotationAngleY, _cameraRotation.YMinRotation, _cameraRotation.YMaxRotation);

        Quaternion rotation = Quaternion.Euler(rotationAngleY, rotationAngleX, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, - _cameraRotation.CameraDistance) + Cube.Instance.transform.position;
        _cameraRotation.transform.rotation = rotation;
        _cameraRotation.transform.position = position;
        _cameraRotation.CurrentRotationAngleX = rotationAngleX;
        _cameraRotation.CurrentRotationAngleY = rotationAngleY;
    }
}
