using System;
using System.Collections;
using System.Collections.Generic;
using TagFrenzy;
using UnityEngine;

/*
 *
 *  This partial class encapsulates the cube states to enable them to access private members of the parent cube class
 * 
 */
public partial class Cube : Singleton<Cube>
{

    /// <summary>  
    /// Abstract base class for cube states
    /// </summary>
    public abstract class CubeState
    {
        protected Cube _cube;
        protected CubeStateID _cubeStateID;

        public CubeStateID CubeStateID
        {
            get { return _cubeStateID; }
        }

        public CubeState(Cube cube, CubeStateID id)
        {
            _cube = cube;
            _cubeStateID = id;
        }

        public abstract void DoBeforeEntering();

        public abstract void DoBeforeLeaving();

        public abstract void CheckForTransition();

        public abstract void RegisterClickedGridField(GridField gridField);
    }

    public class EmptyCubeState : CubeState
    {
        private CubeStateID _initialCubeState;

        protected EmptyCubeState(Cube cube, CubeStateID id) : base(cube, id) { }

        public override void DoBeforeEntering() {}

        public override void DoBeforeLeaving() {}

        public override void CheckForTransition() {}

        public override void RegisterClickedGridField(GridField gridField) {}
    }

    /// <summary>  
    /// The cube state representing the folded state of the cube
    /// </summary>
    public class CubeStateFolded : Cube.CubeState
    {
        public CubeStateFolded(Cube cube, CubeStateID id) : base(cube, id) { }

        public override void DoBeforeEntering()
        {
            // do nothing
        }

        public override void DoBeforeLeaving()
        {
            switch (GameManager.Instance.GameState)
            {
                case GameState.LevelEditorUser:
                case GameState.LevelEditor:
                    _cube.RevertAllConnections();
                    break;
            }
        }

        public override void CheckForTransition()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                 _cube.ChangeState(CubeStateID.CubeStateFoldingTransition);
            }
        }

        /// <summary>  
        /// Register a clicked grid field. The following actions depend on the the current game state
        /// </summary>
        public override void RegisterClickedGridField(GridField gridField)
        {
            switch (GameManager.Instance.GameState) //check the current game state
            {
                case GameState.CubeGameplay:
                case GameState.WorkshopCubeGameplay:
                case GameState.LevelEditor:
                case GameState.LevelEditorUser:
                    Quaternion targetSideRotation = _cube._currentActiveSide.Rotation;

                    if (gridField.GridFieldType == GridFieldType.EmptyGridField || gridField.GridFieldType == GridFieldType.BarrierGridField)
                        return;

                    if (_cube._selectedGridField == null)
                    {
                        _cube.SetSelectedGridField(gridField);
                    }
                    else
                    {
                        var connection = _cube._selectedGridField.GetGridFieldConnection(gridField);
                        if (connection.Valid)
                        {
                            CubeSide originCubeSide = _cube.SelectedGridField.CubeSide;
                            _cube._connections.Add(connection);
                            _cube.SetSelectedGridField(gridField); //set the connection target as the new selected grid field
                            PlayManager.Instance.Moves++;
                            PlayManager.Instance.SetQtyConnections(_cube._connections.Count);

                            if (_cube.SelectedGridField.ConnectionState != GridFieldConnectionState.PortalConnection)
                            {
                                Cube.Instance.GetComponent<CubeRotation>().RotateTowards(originCubeSide, gridField.CubeSide);
                            }
                            else
                            {
                                Cube.Instance.GetComponent<CubeRotation>().RotateTowards(originCubeSide, gridField.CubeSide, false);
                            }
                            targetSideRotation = Cube.Instance.GetComponent<CubeRotation>().ActiveSideRotation;
                            MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>().RevertToInitialRotation();
                        }
                    }

                    _cube.SetCurrentActiveSide(_cube._selectedGridField.CubeSide, targetSideRotation);
                    _cube._selectedGridField.SetSelected(true);
                    break;
            }
        }
    }

    /// <summary>  
    /// The cube state representing the unfolded state of the cube
    /// </summary>
    public class CubeStateLaymap : Cube.CubeState
    {

        public CubeStateLaymap(Cube cube, CubeStateID id) : base(cube, id) { }

        public override void DoBeforeEntering()
        {
            // do nothing
        }

        public override void DoBeforeLeaving()
        {
            switch (GameManager.Instance.GameState)
            {
                case GameState.LevelEditorUser:
                case GameState.LevelEditor:
                    _cube.RevertAllConnections();
                    break;
            }

        }

        public override void CheckForTransition()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _cube.ChangeState(CubeStateID.CubeStateFoldingTransition);
            }
        }


        public override void RegisterClickedGridField(GridField gridField)
        {
            switch (GameManager.Instance.GameState) //check the current game state
            {
                case GameState.LevelEditorUser:
                case GameState.LevelEditor:
                    if (_cube.SelectedGridField != null)
                        _cube.SelectedGridField.SetSelected(false);

                    _cube.SetSelectedGridField(gridField);
                    _cube._selectedGridField.SetSelected(true);
                    break;
            }                   
        }
    }

    /// <summary>  
    /// The cube state representing the transition from laymap to folded and vice versa (no user actions like clicking a gridField possible)
    /// </summary>
    public class CubeStateFoldingTransition : Cube.CubeState
    {
        private CubeStateID _initialCubeState;

        public CubeStateFoldingTransition(Cube cube, CubeStateID id) : base(cube, id) { }

        public override void DoBeforeEntering()
        {
            if (Cube.Instance.Animator.GetCurrentAnimatorStateInfo(0).IsName("Folded"))
            {
                Cube.Instance.Animator.SetTrigger("Unfold");
                Cube.Instance.GetComponent<CubeRotation>().RevertToInitialRotation();
                CameraRotation cameraRotation = MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>();
                cameraRotation.RevertToInitialRotation();
                cameraRotation.StartCoroutine(cameraRotation.ChangeCameraDistance(CameraDistanceType.CameraDistanceLaymap, 0.5f));
                _initialCubeState = CubeStateID.CubeStateFolded;

            }
            else
            {
                Cube.Instance.Animator.SetTrigger("Fold");
                if (GameManager.Instance.GameState == GameState.LevelEditor && GameManager.Instance.GameState == GameState.LevelEditorUser)
                    Cube.Instance.GetComponent<CubeRotation>().RevertToInitialRotation();
                else
                    Cube.Instance.GetComponent<CubeRotation>().RevertToLastActiveSideRotation();

                CameraRotation cameraRotation = MultiTag.FindGameObjectsWithTags(Tags.CubeGameplayCamera)[0].GetComponent<CameraRotation>();
                cameraRotation.RevertToInitialRotation();
                cameraRotation.StartCoroutine(cameraRotation.ChangeCameraDistance(CameraDistanceType.CameraDistanceFolded, 0.5f));
                _initialCubeState = CubeStateID.CubeStateLaymap;
            }
        }

        public override void DoBeforeLeaving()
        {
            //do nothing
        }

        public override void CheckForTransition()
        {
            if (_initialCubeState == CubeStateID.CubeStateLaymap && Cube.Instance.Animator.GetCurrentAnimatorStateInfo(0).IsName("Folded"))
            {
                _cube.ChangeState(CubeStateID.CubeStateFolded);
            }
            else if (_initialCubeState == CubeStateID.CubeStateFolded && Cube.Instance.Animator.GetCurrentAnimatorStateInfo(0).IsName("Unfolded"))
            {
                _cube.ChangeState(CubeStateID.CubeStateLaymap);
            }
        }


        public override void RegisterClickedGridField(GridField gridField)
        {
            // do nothing
        }
    }

    /// <summary>  
    /// The cube state representing the rotation executed when two grid fields are connected or a connection is deleted/reversed
    /// </summary>
    public class CubeStateRotating : Cube.EmptyCubeState
    {
        private CubeStateID _initialCubeState;

        public CubeStateRotating(Cube cube, CubeStateID id) : base(cube, id) { }
    }
}
