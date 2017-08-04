using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSwipeRestricted : CameraRotationBehaviour
{
    [SerializeField] private Vector2 _rotationSpeedSwiping = new Vector2(3.5f, 9.5f);

    public ConstantSwipeRestricted(CameraRotation cameraRotation) : base(cameraRotation) { }

    public override void ExecuteRotation()
    {
        float rotationAngleX = _cameraRotation.CurrentRotationAngleX;
        float rotationAngleY = _cameraRotation.CurrentRotationAngleY;

        if (DoSwipeLeftHorizontal() == true)
        {
            rotationAngleX -= MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeAmount * _rotationSpeedSwiping.x * _cameraRotation.CameraDistance * 0.02f;
            rotationAngleX = ClampRotationAngle(rotationAngleX, _cameraRotation.XMinRotation, _cameraRotation.XMaxRotation);
        }
        else if (DoSwipeLeftVertical() == true)
        {
            rotationAngleY += MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeAmount * _rotationSpeedSwiping.y * 0.02f;
            rotationAngleY = ClampRotationAngle(rotationAngleY, _cameraRotation.YMinRotation, _cameraRotation.YMaxRotation);
        }

        _cameraRotation.CurrentRotationAngleX = rotationAngleX;
        _cameraRotation.CurrentRotationAngleY = rotationAngleY;
        Quaternion rotation = Quaternion.Euler(_cameraRotation.CurrentRotationAngleY, _cameraRotation.CurrentRotationAngleX, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;
        _cameraRotation.transform.rotation = rotation;
        _cameraRotation.transform.position = position;
    }

    private bool DoSwipeLeftHorizontal()
    {
        return (MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeDirection == SwipeDirection.Left || MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeDirection == SwipeDirection.Right) && 
            (MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeDirection == SwipeDirection.None || 
            Mathf.Abs(MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeAmount) > Mathf.Abs(MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeAmount));
    }

    private bool DoSwipeLeftVertical()
    {
        return (MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeDirection == SwipeDirection.Up || MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeDirection == SwipeDirection.Down) &&
            (MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeDirection == SwipeDirection.None ||
            Mathf.Abs(MobileInputManager.Instance.ConstantSwipeInformationVertical.SwipeAmount) > Mathf.Abs(MobileInputManager.Instance.ConstantSwipeInformationHorizontal.SwipeAmount));
    }
}
