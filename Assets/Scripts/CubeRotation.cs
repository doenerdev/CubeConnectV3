using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotation : MonoBehaviour
{

    private Quaternion _initalRotation;
    private Quaternion _activeSideRotation;


    [SerializeField][Range(0f, 5f)] private float _rotationRate;
    [SerializeField] private AnimationEasing _animationEasing;

    public Quaternion ActiveSideRotation
    {
         get { return _activeSideRotation; }
    }

    private void Start()
    {
        _initalRotation = transform.rotation;
        _activeSideRotation = Quaternion.identity;
    }

    public void RevertToInitialRotation()
    {
        StartCoroutine(PerformRotation(transform.rotation, _initalRotation));
    }

    public void RevertToLastActiveSideRotation()
    {
        StartCoroutine(PerformRotation(transform.rotation, _activeSideRotation));
    }

    public void RotateTowards(CubeSide originCubeSide, CubeSide targetCubeSide, bool restrictedRotation = true)
    {
        // first, get the vector showing to the direction of the cubeSide to rotate to

        Vector3 vectorTo = GetVectorForCubeSide(targetCubeSide);
        Vector3 vectorFrom = GetVectorForCubeSide(originCubeSide);

        // second, compute dot product with all possible sides (4 sides are possible in the restricted mode)
        // 1 => direction found, 0 => no alignment
        CubeRotationDirection rotateDirection = CubeRotationDirection.none;
        bool sideFound = false;
        float rotationAngle = Mathf.Abs(Vector3.Angle(vectorFrom, vectorTo));

        float dotProduct = Mathf.Infinity;
        for (int i = 0; i < 6; i++)
        {
            Vector3 possibleSide = Vector3.zero;

            switch (i)
            {
                case 0:
                    possibleSide = Vector3.up;
                    rotateDirection = CubeRotationDirection.up;
                    break;
                case 1:
                    possibleSide = Vector3.down;
                    rotateDirection = CubeRotationDirection.down;
                    break;
                case 2:
                    possibleSide = Vector3.left;
                    rotateDirection = CubeRotationDirection.left;
                    break;
                case 3:
                    possibleSide = Vector3.right;
                    rotateDirection = CubeRotationDirection.right;
                    break;
            }

            dotProduct = (int)Vector3.Dot(possibleSide, vectorTo);
            if (dotProduct == 1)
            {
                sideFound = true;
                break;
            }
        }

        if(sideFound == false && restrictedRotation == false)
        {
            rotateDirection = CubeRotationDirection.up;
            sideFound = true;
        }

        if (sideFound)
        {
            RotateCube(rotateDirection, rotationAngle);
        }
    }

    public void RotateCube(CubeRotationDirection direction, float rotationAngle = 90)
    {
        RaiseCubeRotationStartedEvent("Rotation Started");
        Vector3 rotationAxis = Vector3.back;
        switch (direction)
        {
            case CubeRotationDirection.up:
                rotationAxis = Vector3.left;
                break;
            case CubeRotationDirection.down:
                rotationAxis = Vector3.right;
                break;
            case CubeRotationDirection.left:
                rotationAxis = Vector3.down;
                break;
            case CubeRotationDirection.right:
                rotationAxis = Vector3.up;
                break;
        }

        Quaternion oldRot = transform.rotation;
        transform.Rotate(rotationAxis * rotationAngle, Space.World);
        Quaternion quaternionTo = transform.rotation;
        _activeSideRotation = quaternionTo;

        StartCoroutine(PerformRotation(oldRot, quaternionTo));
    }

    private IEnumerator PerformRotation(Quaternion startRot, Quaternion endRot)
    {
        float i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime * _rotationRate;
            transform.rotation = Quaternion.Slerp(startRot, endRot, AnimationEasings.AnimationEasingByType(_animationEasing, i));
            yield return null;
        }

        RaiseCubeRotationCompletedEvent("Rotation Completed");
    }

    private Vector3 GetVectorForCubeSide(CubeSide side)
    {
        switch (side)
        {
            case CubeSide.Front:
                return transform.forward * (-1); // weird, but front is back
            case CubeSide.Left:
                return transform.right * (-1);
            case CubeSide.Right:
                return transform.right;
            case CubeSide.Top:
                return transform.up;
            case CubeSide.Bottom:
                return transform.up * (-1);
            case CubeSide.Back:
                return transform.forward;
            default:
                return Vector3.zero;
        }
    }

    private void RaiseCubeRotationCompletedEvent(string message)
    {
        EventHandler<EventTextArgs> handler = CubeRotationCompleted;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    private void RaiseCubeRotationStartedEvent(string message)
    {
        EventHandler<EventTextArgs> handler = CubeRotationStarted;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    #region Events
    public event EventHandler<EventTextArgs> CubeRotationStarted;
    public event EventHandler<EventTextArgs> CubeRotationCompleted;
    #endregion Events

    private Dictionary<CubeSide, Dictionary<KeyCode, Vector3>> _cubeSideRotationAxis = new Dictionary<CubeSide, Dictionary<KeyCode, Vector3>>()
    {
        { CubeSide.Front, new Dictionary<KeyCode, Vector3>()
            {
                { KeyCode.LeftArrow, Vector3.down },
                { KeyCode.RightArrow, Vector3.up },
                { KeyCode.UpArrow, Vector3.right },
                { KeyCode.DownArrow, Vector3.left },
            }
        }
    };


}

public enum CubeRotationDirection
{
    none,
    up,
    down,
    left,
    right
}