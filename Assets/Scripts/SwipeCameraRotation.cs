using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCameraRotation : CameraRotationBehaviour {

    public SwipeCameraRotation(CameraRotation cameraRotation) : base(cameraRotation) { }

    public override void ExecuteRotation()
    {
        Quaternion rotation = Quaternion.identity;
        Vector3 position = Vector3.zero;

        if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Left)
        {
            rotation = Quaternion.Euler(0, _cameraRotation.YMinRotation, 0);
            rotation = _cameraRotation.transform.rotation == Quaternion.Euler(0, _cameraRotation.YMaxRotation, 0) ? Quaternion.Euler(0, _cameraRotation.InitialRotation.eulerAngles.y, 0) : rotation;
            position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;

            _cameraRotation.PerformRotation(_cameraRotation.transform.rotation, rotation, _cameraRotation.transform.position, position);
        }
        else if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Right)
        {
            rotation = Quaternion.Euler(0, _cameraRotation.YMaxRotation, 0);
            rotation = _cameraRotation.transform.rotation == Quaternion.Euler(0, _cameraRotation.YMinRotation, 0) ? Quaternion.Euler(0, _cameraRotation.InitialRotation.eulerAngles.y, 0) : rotation;
            position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;

            _cameraRotation.PerformRotation(_cameraRotation.transform.rotation, rotation, _cameraRotation.transform.position, position);
        }
        else if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Up)
        {
            rotation = Quaternion.Euler(_cameraRotation.XMaxRotation, 0, 0);
            rotation = _cameraRotation.transform.rotation == Quaternion.Euler(_cameraRotation.XMinRotation, 0, 0) ? Quaternion.Euler(_cameraRotation.InitialRotation.eulerAngles.x, 0, 0) : rotation;
            position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;

            _cameraRotation.PerformRotation(_cameraRotation.transform.rotation, rotation, _cameraRotation.transform.position, position);
        }
        else if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Down)
        {
            rotation = Quaternion.Euler(_cameraRotation.XMinRotation, 0, 0);
            rotation = _cameraRotation.transform.rotation == Quaternion.Euler(_cameraRotation.XMaxRotation, 0, 0) ? Quaternion.Euler(_cameraRotation.InitialRotation.eulerAngles.x, 0, 0) : rotation;
            position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;

            _cameraRotation.PerformRotation(_cameraRotation.transform.rotation, rotation, _cameraRotation.transform.position, position);
        }
    }

    //public override void ExecuteRotation()
    //{
    //    Quaternion rotation = Quaternion.identity;
    //    Vector3 position = Vector3.zero;

    //    Debug.Log(MobileInputManager.Instance.SwipeInformation.SwipeDirection);
    //    if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Left)
    //    {
    //        Debug.Log("Swipe left");
    //        rotation = Quaternion.Euler(0, _cameraRotation.YMaxRotation, 0);
    //        position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;
    //    }
    //    else if (MobileInputManager.Instance.SwipeInformation.SwipeDirection == SwipeDirection.Right)
    //    {
    //        Debug.Log("Swipe right");
    //        rotation = Quaternion.Euler(0, _cameraRotation.YMinRotation, 0);
    //        position = rotation * new Vector3(0f, 0f, -_cameraRotation.CameraDistance) + Cube.Instance.transform.position;
    //    }

    //    _cameraRotation.PerformRotation(_cameraRotation.transform.rotation, rotation, _cameraRotation.transform.position, position);
    //}
}
