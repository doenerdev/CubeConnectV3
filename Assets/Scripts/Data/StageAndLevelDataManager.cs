using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

    private UserGeneratedLevelDataHolder _userGeneratedLevelDataHolder;
    private UserGeneratedLevelInfoHolder _userGeneratedLevelInfoHolder;
    private StageAndLevelData _stageAndLevelData;

    protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.gameObject);

        _stageAndLevelData = new StageAndLevelData();
        _userGeneratedLevelDataHolder = new UserGeneratedLevelDataHolder();
        _userGeneratedLevelInfoHolder = new UserGeneratedLevelInfoHolder();
        InitializeBackgroundWorker();

        //_userGeneratedLevelInfoHolder = new UserGeneratedLevelInfoHolder(); //TODO JUST FOR TESTING REMOVE LATER
        //SaveUserGeneratedLevelInfoHolder();
    }

    private void InitializeBackgroundWorker()
    {
        _asyncUserGeneratedLevelDataHolderLoader = new BackgroundWorker();
        _asyncUserGeneratedLevelDataHolderLoader.WorkerReportsProgress = false;
        _asyncUserGeneratedLevelDataHolderLoader.WorkerSupportsCancellation = true;
        _asyncUserGeneratedLevelDataHolderLoader.DoWork += (obj, e) => LoadUserGeneratedLevelDataHolderAsync((string)e.Argument);
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

    public void SaveUserGeneratedLevelInfoHolder(Action callback = null)
    {
        Task.Run(() =>
        {
            FileStream stream = new FileStream(GameManager.Instance.UserLevelsInfoPath.Replace("file:///", ""), FileMode.Create);
            ProtoBuf.Serializer.Serialize<UserGeneratedLevelInfoHolder>(stream, _userGeneratedLevelInfoHolder);
            stream.Close();
            RaiseSavingUserGeneratedLevelDataHolderComplete("Saving UserGeneratedLevelDataHolder complete");

            if (callback != null)
            {
                callback();
            }
        });
    }

    public void SaveUserGeneratedLevel(UserGeneratedLevelData levelData, string levelcode, Action<string> callback)
    {
        Task.Run(() =>
        {
            FileStream stream = new FileStream((GameManager.Instance.UserLevelsDataPath + "/" + levelcode + ".lvl").Replace("file:///", ""), FileMode.Create);
            ProtoBuf.Serializer.Serialize<UserGeneratedLevelData>(stream, levelData);
            UnityMainThreadDispatcher.Enqueue(() => {Debug.Log(levelData.CubeMap);});
            stream.Close();
            callback((GameManager.Instance.UserLevelsDataPath + "/" + levelcode + ".lvl"));
            //RaiseSavingUserGeneratedLevelDataHolderComplete("Saving UserGeneratedLevelDataHolder complete");
        });
    }

    public void SaveLevelAndStageData()
    {
        FileStream stream = new FileStream(GameManager.Instance.ProductionLevelsDataPath.Replace("file:///", ""), FileMode.Create);
        ProtoBuf.Serializer.Serialize(stream, _stageAndLevelData);
        stream.Close();
    }

    public void LoadUserGeneratedLevelDataHolderAsync(string path)
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

    public void LoadNeccessaryInitialLevelData()
    {
        StartCoroutine(LoadNeccessaryInitialLevelDataFromStreaming());
    }

    private IEnumerator LoadNeccessaryInitialLevelDataFromStreaming()
    {
        WWW levelFile = new WWW(GameManager.Instance.ProductionLevelsDataPath);
        yield return levelFile;
        byte[] levelFileBytes = levelFile.bytes;
        WWW infoFile = new WWW(GameManager.Instance.UserLevelsInfoPath);
        yield return infoFile;
        byte[] infoFileBytes = infoFile.bytes;

        Task.Run(() =>
        {
            StageAndLevelData stageAndLevelData = null;
            UserGeneratedLevelInfoHolder userGeneratedLevelInfoHolder = null;
            using (MemoryStream ms = new MemoryStream(levelFileBytes))
            {
                stageAndLevelData = ProtoBuf.Serializer.Deserialize<StageAndLevelData>(new BufferedStream(ms));
                ms.Close();
            }
            using (MemoryStream ms = new MemoryStream(infoFileBytes))
            {
                userGeneratedLevelInfoHolder = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelInfoHolder>(new BufferedStream(ms));
                ms.Close();
            }


            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (stageAndLevelData != null)
                {
                    _stageAndLevelData = stageAndLevelData;
                }
                Debug.Log(_stageAndLevelData.Stages.Count);
                if (userGeneratedLevelInfoHolder != null)
                {
                    _userGeneratedLevelInfoHolder = userGeneratedLevelInfoHolder;
                }
                RaiseLoadingNeccessaryInitialLevelDataFromStreamingComplete("RaiseLoadingNeccessaryInitialLevelDataFromStreamingComplete complete");
            });
        });
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

    public Dictionary<string, UserGeneratedLevelInfo> GetUserGeneratedLevelInfosList()
    {
        return _userGeneratedLevelInfoHolder.LevelInfos;
    } 

    public UserGeneratedLevelInfo GetUserGeneratedLevelInfoByLevelcode(string levelcode)
    {
        if (_userGeneratedLevelInfoHolder.LevelInfos.ContainsKey(levelcode))
        {
            return _userGeneratedLevelInfoHolder.LevelInfos[levelcode];
        }
        else
        {
            return null;
        }
    }

    /*public UserGeneratedLevelData GetUserGeneratedLevelDataByLevelcode(string levelcode)
    {
        
    }*/

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

    private void RaiseSavingUserGeneratedLevelDataHolderComplete(string message)
    {
        EventHandler<EventTextArgs> handler = SavingUserGeneratedLevelDataHolderComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    private void RaiseLoadingNeccessaryInitialLevelDataFromStreamingComplete(string message)
    {
        EventHandler<EventTextArgs> handler = LoadingNeccessaryInitialLevelDataFromStreamingComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    #region Events
    public event EventHandler<EventTextArgs> LoadingStageAndLevelDataComplete;
    public event EventHandler<EventTextArgs> LoadingUserGeneratedLevelDataHolderComplete;
    public event EventHandler<EventTextArgs> SavingUserGeneratedLevelDataHolderComplete;
    public event EventHandler<EventTextArgs> LoadingNeccessaryInitialLevelDataFromStreamingComplete;
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