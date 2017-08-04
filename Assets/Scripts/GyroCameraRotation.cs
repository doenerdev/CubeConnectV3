using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCameraRotation : CameraRotationBehaviour {

    //[SerializeField] private Vector2 _rotationSpeedGyro = new Vector2(20f, 100f);

    private float calibRotX = 0f;
    private float calibRotY = 0f;

    private float lastXAngle = 0f;
    private float lastYAngle = 0f;
    private bool xAxisCalibrated = true;
    private bool yAxisCalibrated = false;

    public GyroCameraRotation(CameraRotation cameraRotation) : base(cameraRotation) { }

    public override void RotationRevertingStarted()
    {
        calibRotX = _cameraRotation.CurrentRotationAngleX - (Input.gyro.rotationRateUnbiased.y * _cameraRotation.gyroSpeedX * _cameraRotation.CameraDistance);
        xAxisCalibrated = false;
        yAxisCalibrated = false;
        lastXAngle = _cameraRotation.CurrentRotationAngleX;
        lastYAngle = _cameraRotation.CurrentRotationAngleY;
    }

    private void CheckCalibration()
    {
        float rotationAngleX = lastXAngle;
        float rotationAngleY = lastYAngle;

        rotationAngleX -= Input.gyro.rotationRateUnbiased.y * _cameraRotation.gyroSpeedX * _cameraRotation.CameraDistance;
        rotationAngleX = ClampRotationAngle(rotationAngleX, _cameraRotation.XMinRotation, _cameraRotation.XMaxRotation);
        rotationAngleY += Input.gyro.rotationRateUnbiased.x * _cameraRotation.gyroSpeedY * _cameraRotation.CameraDistance;
        rotationAngleY = ClampRotationAngle(rotationAngleY, _cameraRotation.YMinRotation, _cameraRotation.YMaxRotation);

        _cameraRotation.CurrentRotationAngleX = rotationAngleX;
        _cameraRotation.CurrentRotationAngleY = rotationAngleY;
        Quaternion rotation = Quaternion.Euler(_cameraRotation.CurrentRotationAngleY, _cameraRotation.CurrentRotationAngleX, 0);
        rotation = Quaternion.Lerp(_cameraRotation.transform.rotation, rotation, Time.deltaTime * _cameraRotation.gyroDampening);
        Vector3 position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;

        if (Mathf.Abs(rotation.eulerAngles.x) < _cameraRotation.gyroRecenterTreshold)
        {
            xAxisCalibrated = true;
        }
        if (Mathf.Abs(rotation.eulerAngles.y) < _cameraRotation.gyroRecenterTreshold)
        {
            yAxisCalibrated = true;
        }

        lastXAngle = rotationAngleX;
        lastYAngle = rotationAngleY;
    }

    public override void ExecuteRotation()
    {
        bool checkCalibration = false;
        float rotationAngleX = _cameraRotation.CurrentRotationAngleX;
        float rotationAngleY = _cameraRotation.CurrentRotationAngleY;

        if (Mathf.Abs(Input.gyro.rotationRateUnbiased.y) > _cameraRotation.gyroRotationThreshold)
        {
            rotationAngleX -= Input.gyro.rotationRateUnbiased.y * _cameraRotation.gyroSpeedX * _cameraRotation.CameraDistance;
            rotationAngleX = ClampRotationAngle(rotationAngleX, _cameraRotation.XMinRotation, _cameraRotation.XMaxRotation);
        }
        if (Mathf.Abs(Input.gyro.rotationRateUnbiased.x) > _cameraRotation.gyroRotationThreshold)
        {
            rotationAngleY += Input.gyro.rotationRateUnbiased.x * _cameraRotation.gyroSpeedY * _cameraRotation.CameraDistance;
            rotationAngleY = ClampRotationAngle(rotationAngleY, _cameraRotation.YMinRotation, _cameraRotation.YMaxRotation);
        }


        if (xAxisCalibrated == false || yAxisCalibrated == false) 
        {
            CheckCalibration();
            return;
        }

        _cameraRotation.CurrentRotationAngleX = rotationAngleX;
        _cameraRotation.CurrentRotationAngleY = rotationAngleY;
        Quaternion rotation = Quaternion.Euler(_cameraRotation.CurrentRotationAngleY, _cameraRotation.CurrentRotationAngleX, 0);
        rotation = Quaternion.Lerp(_cameraRotation.transform.rotation, rotation, Time.deltaTime * _cameraRotation.gyroDampening);
        Vector3 position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;
        _cameraRotation.transform.rotation = rotation;
        _cameraRotation.transform.position = position;
    }
}
