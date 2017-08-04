using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputManager : Singleton<MobileInputManager>
{

    private SwipeInformation _swipeInformation;
    private SwipeInformation _constantSwipeInformationHorizontal;
    private SwipeInformation _constantSwipeInformationVertical;
    private Vector3 _swipeStart;
    private Vector3 _constantSwipeLastPosition;

    [SerializeField] private Vector2 _constantSwipeThreshold;
    [SerializeField] private Vector2 _swipeThreshold;

    public SwipeInformation SwipeInformation
    {
        get { return _swipeInformation; }
    }
    public SwipeInformation ConstantSwipeInformationHorizontal
    {
        get { return _constantSwipeInformationHorizontal; }
    }
    public SwipeInformation ConstantSwipeInformationVertical
    {
        get { return _constantSwipeInformationVertical; }
    }

    protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Update () {
        UpdateSwipeInformation();
        UpdateConstantSwipeInformation();
	}

    private void UpdateSwipeInformation()
    {
        //reset the swipe information
        _swipeInformation.SwipeDirection = SwipeDirection.None;
        _swipeInformation.SwipeAmount = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            _swipeStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeDistance = _swipeStart - Input.mousePosition;

            if (Mathf.Abs(swipeDistance.x) > _swipeThreshold.x)
            {
                _swipeInformation.SwipeDirection = swipeDistance.x < 0f ? SwipeDirection.Right : SwipeDirection.Left;
                _swipeInformation.SwipeAmount = swipeDistance.x;
            }
            if (Mathf.Abs(swipeDistance.y) > _swipeThreshold.y && Mathf.Abs(swipeDistance.y) > Mathf.Abs(swipeDistance.x))
            {
                _swipeInformation.SwipeDirection = swipeDistance.y < 0f ? SwipeDirection.Down : SwipeDirection.Up;
                _swipeInformation.SwipeAmount = swipeDistance.y;

            }
        }
    }

    private void UpdateConstantSwipeInformation()
    {
        //reset the swipe informations
        _constantSwipeInformationHorizontal.SwipeDirection = SwipeDirection.None;
        _constantSwipeInformationHorizontal.SwipeAmount = 0f;
        _constantSwipeInformationVertical.SwipeDirection = SwipeDirection.None;
        _constantSwipeInformationVertical.SwipeAmount = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            _constantSwipeLastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 swipeDistance = _constantSwipeLastPosition - Input.mousePosition;
            _constantSwipeLastPosition = Input.mousePosition;

            if (Mathf.Abs(swipeDistance.x) > _constantSwipeThreshold.x)
            {
                _constantSwipeInformationHorizontal.SwipeDirection = swipeDistance.x < 0f ? SwipeDirection.Left : SwipeDirection.Right;
                _constantSwipeInformationHorizontal.SwipeAmount = swipeDistance.x;
            }
            if (Mathf.Abs(swipeDistance.y) > _constantSwipeThreshold.y)
            {
                _constantSwipeInformationVertical.SwipeDirection = swipeDistance.y < 0f ? SwipeDirection.Down : SwipeDirection.Up;
                _constantSwipeInformationVertical.SwipeAmount = swipeDistance.y;
            }
        }
    }
}

public struct SwipeInformation
{
    public SwipeDirection SwipeDirection;
    public float SwipeAmount;
}

public enum SwipeDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}