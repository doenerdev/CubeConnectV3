using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class StageAndLevelDataManager : Singleton<StageAndLevelDataManager>
{
    private BackgroundWorker _asyncStageAndLevelLoader;
    private BackgroundWorker _asyncUserGeneratedLevelDataHolderLoader;
    private UserGeneratedLevesListInfo _userGeneratedLevesListInfo;

    [SerializeField] private UserGeneratedLevelDataHolder _userGeneratedLevelDataHolder;
    [SerializeField] private StageAndLevelData _stageAndLevelData;

    protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.gameObject);

        _stageAndLevelData = new StageAndLevelData();
        _userGeneratedLevelDataHolder = new UserGeneratedLevelDataHolder();
        InitializeBackgroundWorker();
    }

    private void InitializeBackgroundWorker()
    {
        _asyncUserGeneratedLevelDataHolderLoader = new BackgroundWorker();
        _asyncUserGeneratedLevelDataHolderLoader.WorkerReportsProgress = false;
        _asyncUserGeneratedLevelDataHolderLoader.WorkerSupportsCancellation = true;
        _asyncUserGeneratedLevelDataHolderLoader.DoWork += (obj, e) => LoadUserGeneratedLevelDataHolder((string)e.Argument);
        _asyncUserGeneratedLevelDataHolderLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                RaiseLoadingUserGeneratedLevelDataHolderComplete("Loading Completed");
            });
        });
    }

    public void AddStageAt(int index, StageData data)
    {
        _stageAndLevelData.Stages.Insert(index, data);
    }

    public void AddStage()
    {
        int index = _stageAndLevelData.Stages.Count;
        _stageAndLevelData.Stages.Insert(index, new StageData(index));
    }

    public void RemoveStageAt(int index)
    {
        _stageAndLevelData.Stages.RemoveAt(index);
    }

    public void AddLevelAt(int stageIndex, int levelIndex, LevelData data)
    {
        _stageAndLevelData.Stages[stageIndex].Levels.Insert(levelIndex, data);
    }

    public void AddLevel(int stageIndex, LevelData data)
    {
        _stageAndLevelData.Stages[stageIndex].Levels.Add(data);
    }

    public void RemoveLevelAt(int stageIndex, int levelIndex)
    {
        _stageAndLevelData.Stages[stageIndex].Levels.RemoveAt(levelIndex);
    }

    public void AddUserGeneratedLevel(UserGeneratedLevelData data)
    {
        _userGeneratedLevelDataHolder.Levels.Add(data);
    }

    public void AddUserGeneratedLevelAt(int levelIndex, UserGeneratedLevelData data)
    {
        _userGeneratedLevelDataHolder.Levels.Insert(levelIndex, data);
    }

    public void SaveUserGeneratedLevelDataHolder()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(GameManager.Instance.UserLevelsDataPath.Replace("file:///", ""), FileMode.Create);
        binaryFormatter.Serialize(stream, _userGeneratedLevelDataHolder);
        stream.Close();
    }

    public void SaveLevelAndStageData()
    {
        FileStream stream = new FileStream(GameManager.Instance.ProductionLevelsDataPath.Replace("file:///", ""), FileMode.Create);
        ProtoBuf.Serializer.Serialize(stream, _stageAndLevelData);
        stream.Close();
    }

    public void LoadUserGeneratedLevelDataHolder(string path)
    {
        Debug.Log("Loading Level");
        StartCoroutine(LoadUserGeneratedLevelDataHolderFromStreamingAsync(path));
    }

    private IEnumerator LoadUserGeneratedLevelDataHolderFromStreamingAsync(string path)
    {
        WWW file = new WWW(path);
        yield return file;
        byte[] fileBytes = file.bytes; //can only be accessed from the main thread, preparing it beforehand
        Task.Run(() =>
        {
            UserGeneratedLevelDataHolder userGeneratedLevelDataHolder = null;
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                userGeneratedLevelDataHolder = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelDataHolder>(new BufferedStream(ms));
                ms.Close();
            }

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (userGeneratedLevelDataHolder != null)
                {
                    _userGeneratedLevelDataHolder = userGeneratedLevelDataHolder;
                }
                RaiseLoadingUserGeneratedLevelDataHolderComplete("Loading user generated levels complete");
            });
        });
    }

    public void LoadLevelAndStageDataAsync(string path)
    {
        Debug.Log("Loading Async");
        StartCoroutine(LoadLevelAndStageDataFromStreaming(path));
    }

    private IEnumerator LoadLevelAndStageDataFromStreaming(string path)
    {
        WWW file = new WWW(path);
        yield return file;
        byte[] fileBytes = file.bytes; //can only be accessed from the main thread, preparing it beforehand
        Task.Run(() =>
        {
            StageAndLevelData stageAndLevelData = null;
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                stageAndLevelData = ProtoBuf.Serializer.Deserialize<StageAndLevelData>(new BufferedStream(ms));
                ms.Close();
            }

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (stageAndLevelData != null)
                {
                    _stageAndLevelData = stageAndLevelData;
                }
                RaiseLoadingStageAndLevelDataComplete("Loading Levels complete");
            });
        });
    }

    public void LoadUserGeneratedLevesListInfoAsync(string path)
    {
        StartCoroutine(LoadLevelAndStageDataFromStreaming(path));
    }

    public List<UserGeneratedLevelData> GetUserGeneratedLevels()
    {
        return _userGeneratedLevelDataHolder.Levels;
    } 

    public List<StageData> GetStages()
    {
        return _stageAndLevelData.Stages;
    }

    public StageData GetStageDataByIndex(int index)
    {
        if (index >= 0 && index < _stageAndLevelData.Stages.Count)
            return _stageAndLevelData.Stages[index];

        return null;
    }

    private void RaiseLoadingStageAndLevelDataComplete(string message)
    {
        EventHandler<EventTextArgs> handler = LoadingStageAndLevelDataComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    private void RaiseLoadingUserGeneratedLevelDataHolderComplete(string message)
    {
        EventHandler<EventTextArgs> handler = LoadingUserGeneratedLevelDataHolderComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    #region Events
    public event EventHandler<EventTextArgs> LoadingStageAndLevelDataComplete;
    public event EventHandler<EventTextArgs> LoadingUserGeneratedLevelDataHolderComplete;
    #endregion Events
}

public class EventTextArgs : EventArgs
{
    #region Fields
    private string _message;
    #endregion Fields
 
    #region ConstructorsH
    public EventTextArgs(string message)
    {
        _message = message;
    }
    #endregion Constructors
 
    #region Properties
    public string Message
    {
        get { return _message; }
        set { _message = value; }
    }
    #endregion Properties
}

public class EventBooleanArgs : EventArgs
{
    #region Fields
    private bool _booleanValue;
    #endregion Fields

    #region ConstructorsH
    public EventBooleanArgs(bool booleanValue)
    {
        _booleanValue = booleanValue;
    }
    #endregion Constructors

    #region Properties
    public bool BooleanValue
    {
        get { return _booleanValue; }
        set { _booleanValue = value; }
    }
    #endregion Properties
}

public class EventUserIDArgs : EventArgs
{
    #region Fields
    private string _userID;
    #endregion Fields

    #region ConstructorsH
    public EventUserIDArgs(string userID)
    {
        _userID = userID;
    }
    #endregion Constructors

    #region Properties
    public string UserID
    {
        get { return _userID; }
        set { _userID = value; }
    }
    #endregion Properties
}