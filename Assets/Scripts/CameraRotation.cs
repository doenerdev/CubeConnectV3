using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    public float gyroSpeedX;
    public float gyroSpeedY;
    public float gyroDampening;
    public float gyroRecenterTreshold;
    public float gyroRotationThreshold;

    private float _cameraDistance = 5.0f;
    private float _currentRotationAngleX = 0.0f;
    private float _currentRotationAngleY = 0.0f;
    private Quaternion _initalRotation;
    private Vector3 _initialPosition;
    private bool _cameraIsRotReverting = false;
    private CameraRotationBehaviour _cameraRotationBehaviour;
    private Dictionary<CameraRotationBehaviourType, CameraRotationBehaviour> _cameraRotationBehaviours;

    [SerializeField] private CameraRotationBehaviourType _cameraRotationBehaviourType;
    [SerializeField] private bool _swipeControls = false; //TODO just for testing, remove later
    [SerializeField] private float _revertRotationDuration = 0.75f;
    [SerializeField] private float _cameraDistanceFolded = 5.0f;
    [SerializeField] private float _cameraDistanceLaymap = 5.0f;
    [SerializeField] private float _xMinRotation = -45f;
    [SerializeField] private float _xMaxRotation = 45f;
    [SerializeField] private float _yMinRotation = -45f;
    [SerializeField] private float _yMaxRotation = 45f;
    [SerializeField] private AnimationEasing _cameraDistanceAnimationEasing;

    public float CurrentRotationAngleX
    {
        set { _currentRotationAngleX = value; }
        get { return _currentRotationAngleX; }
    }
    public float CurrentRotationAngleY
    {
        set { _currentRotationAngleY = value; }
        get { return _currentRotationAngleY; }
    }
    public float CameraDistance
    {
        get { return _cameraDistance; }
    }
    public float CameraDistanceFolded 
    {
        get { return _cameraDistanceFolded; }
    }
    public float CameraDistanceLaymap
    {
        get { return _cameraDistanceLaymap; }
    }
    public float XMinRotation
    {
        get { return _xMinRotation; }
    }
    public float XMaxRotation
    {
        get { return _xMaxRotation; }
    }
    public float YMinRotation 
    {
        get { return _yMinRotation; }
    }
    public float YMaxRotation
    {
        get { return _yMaxRotation; }
    }
    public Quaternion InitialRotation
    {
        get { return _initalRotation; }
    }
    public Vector3 InitialPosition
    {
        get { return _initialPosition; }
    }

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _currentRotationAngleY = angles.y;
        _currentRotationAngleX = angles.x;
        _initalRotation = transform.rotation;
        _initialPosition = transform.position;
        Input.gyro.enabled = true;
        InitializeCameraRotationBehaviours();
    }

    private void InitializeCameraRotationBehaviours()
    {
        _cameraRotationBehaviours = new Dictionary<CameraRotationBehaviourType, CameraRotationBehaviour>();
        _cameraRotationBehaviours.Add(CameraRotationBehaviourType.Keyboard, new KeyboardCameraRotation(this));
        _cameraRotationBehaviours.Add(CameraRotationBehaviourType.Swipe, new SwipeCameraRotation(this));
        _cameraRotationBehaviours.Add(CameraRotationBehaviourType.ConstantSwipeAllAxis, new ConstantSwipeAllAxis(this));
        _cameraRotationBehaviours.Add(CameraRotationBehaviourType.ConstantSwipeRestricted, new ConstantSwipeRestricted(this));
        _cameraRotationBehaviours.Add(CameraRotationBehaviourType.Gyro, new GyroCameraRotation(this));
    }

    private void LateUpdate()
    {
        if(_cameraIsRotReverting == false)
            _cameraRotationBehaviours[_cameraRotationBehaviourType].ExecuteRotation();  
        
        if (Input.GetKeyUp("v"))
        {
            RevertToInitialRotation();
        }

        if (Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateLaymap)
        {
            //_cameraDistance = _cameraDistanceLaymap;
        }
        else if(Cube.Instance.CurrentCubeStateID == CubeStateID.CubeStateFolded)
        {
            //_cameraDistance = _cameraDistanceFolded;
        }
    }

    public IEnumerator ChangeCameraDistance(CameraDistanceType distanceType, float rotationRate)
    {
        float i = 0.0f;
        float desiredDistance = distanceType == CameraDistanceType.CameraDistanceFolded ? _cameraDistanceFolded : _cameraDistanceLaymap;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rotationRate;
            _cameraDistance = Mathf.Lerp(_cameraDistance, desiredDistance, AnimationEasings.AnimationEasingByType(_cameraDistanceAnimationEasing, i));
            yield return null;
        }
    }

    public void RevertToInitialRotation()
    {
        _cameraIsRotReverting = true;
        _cameraRotationBehaviours[_cameraRotationBehaviourType].RotationRevertingStarted();
        StartCoroutine(PerformRevertRotation(transform.rotation, _initalRotation, _initialPosition, transform.position));
    }

    public void PerformRotation(Quaternion startRotation, Quaternion endRotation, Vector3 startPosition, Vector3 endPosition)
    {
        _cameraIsRotReverting = true;
        StartCoroutine(PerformRevertRotation(startRotation, endRotation, startPosition, endPosition));
    }

    public IEnumerator PerformRevertRotation(Quaternion startRotation, Quaternion endRotation, Vector3 startPosition, Vector3 endPosition)
    {
        float i = 0.0f;
        float updateRate = 1.0f / _revertRotationDuration;

        while (i < 1.0f)
        {
            i += Time.deltaTime * updateRate;
            Quaternion rotation = Quaternion.Slerp(startRotation, endRotation, Mathf.SmoothStep(0.0f, 1.0f, i));
            Vector3 position = rotation * new Vector3(0f, 0f, -_cameraDistance) + Cube.Instance.transform.position;

            transform.rotation = rotation;
            transform.position = position;
            yield return null;
        }

        _currentRotationAngleX = transform.rotation.eulerAngles.x;
        _currentRotationAngleY = transform.rotation.eulerAngles.y;
        _cameraIsRotReverting = false;
    }
}


public enum CameraRotationBehaviourType
{
    Keyboard,
    Swipe,
    ConstantSwipeRestricted,
    ConstantSwipeAllAxis,
    Gyro,
}

public enum CameraDistanceType
{
    CameraDistanceFolded,
    CameraDistanceLaymap
}