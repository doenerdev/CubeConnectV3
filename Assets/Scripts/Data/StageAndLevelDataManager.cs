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

    public void RemoveUserGeneratedLevel(string levelcode)
    {
        var levelInfo = _userGeneratedLevelInfoHolder.LevelInfos.Where(e => e.Value.LevelCode == levelcode)
                .Select(e => (KeyValuePair<string, UserGeneratedLevelInfo>?)e)
                .FirstOrDefault();

        if (levelInfo != null)
        {
            _userGeneratedLevelInfoHolder.LevelInfos.Remove(levelcode);
            Debug.Log(levelInfo.Value.Value.FileLocation);
            if (File.Exists(levelInfo.Value.Value.FileLocation.Replace("file:///", "")))
            {
                Debug.Log("Deleted File");
                File.Delete(levelInfo.Value.Value.FileLocation.Replace("file:///", ""));
            }
            else
            {
                Debug.Log("File not found File");
            }
        }
    }

    public void SaveUserGeneratedLevelInfoHolder(Action callback = null)
    {
        Task.Run(() =>
        {
            Directory.CreateDirectory(GameManager.Instance.UserLevelsInfoFolderPath.Replace("file:///", ""));      
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

    public void SaveDownloadedUserGeneratedLevel(UserGeneratedLevelInfo levelInfo, byte[] file)
    {
        Task.Run(() =>
        {
            levelInfo.FileLocation = GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", "") + "/" + levelInfo.LevelCode + ".lvl";
            _userGeneratedLevelInfoHolder.LevelInfos.Add(levelInfo.LevelCode, levelInfo); //TODO make a security check if the levelcode already exists in the dic
            Directory.CreateDirectory(GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", ""));
            File.WriteAllBytes(GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", "") + "/" + levelInfo.LevelCode + ".lvl", file);
            SaveUserGeneratedLevelInfoHolder();
        });
    }

    public void SaveUserGeneratedLevel(UserGeneratedLevelData levelData, string levelcode, Action<string> callback)
    {
        Task.Run(() =>
        {
            Directory.CreateDirectory(GameManager.Instance.UserLevelsDataPath.Replace("file:///", ""));
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

        Task.Run(() =>
        {
            StageAndLevelData stageAndLevelData = null;
            UserGeneratedLevelInfoHolder userGeneratedLevelInfoHolder = null;
            using (MemoryStream ms = new MemoryStream(levelFileBytes))
            {
                stageAndLevelData = ProtoBuf.Serializer.Deserialize<StageAndLevelData>(new BufferedStream(ms));
                ms.Close();
            }
            UnityMainThreadDispatcher.Enqueue(() => { GameManager.Instance.UserLevelsInfoPath.Replace("file:///", ""); });
            if (File.Exists(GameManager.Instance.UserLevelsInfoPath.Replace("file:///", ""))) //if the file doesn't exist yet, create it
            {
                using (FileStream fs = File.Open(GameManager.Instance.UserLevelsInfoPath.Replace("file:///", ""), FileMode.Open))
                {
                    userGeneratedLevelInfoHolder = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelInfoHolder>(fs);
                    fs.Close();
                }
            }
            else
            {
                userGeneratedLevelInfoHolder = new UserGeneratedLevelInfoHolder();
                SaveUserGeneratedLevelInfoHolder();
            }

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (stageAndLevelData != null)
                {
                    _stageAndLevelData = stageAndLevelData;
                }
                if (userGeneratedLevelInfoHolder != null)
                {
                    _userGeneratedLevelInfoHolder = userGeneratedLevelInfoHolder;
                }
                RaiseLoadingNeccessaryInitialLevelDataFromStreamingComplete("RaiseLoadingNeccessaryInitialLevelDataFromStreamingComplete complete");
            });
        });
    }

    public void LoadUserGeneratedLevelAsync(string fileLocation, Action<UserGeneratedLevelData> callback = null)
    {
        using (FileStream fs = File.Open(fileLocation.Replace("file:///", ""), FileMode.Open))
        {
            UserGeneratedLevelData levelData = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelData>(new BufferedStream(fs));
            fs.Close();
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (callback != null)
                {
                    callback(levelData);
                }
            });

        }
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

    public Dictionary<string, UserGeneratedLevelInfo> GetUserGeneratedLevelInfosList()
    {
        return _userGeneratedLevelInfoHolder.LevelInfos;
    }

    public Dictionary<string, UserGeneratedLevelInfo> GetOwnUserGeneratedLevelInfosList()
    {
        if (Application.isEditor)
        {
            return
            _userGeneratedLevelInfoHolder.LevelInfos.Where(
                x => x.Value.AuthorID == FirebaseAuthentication.DESKTOP_USER_ID)
                .ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            return
            _userGeneratedLevelInfoHolder.LevelInfos.Where(
                x => x.Value.AuthorID == FirebaseAuthentication.Instance.CurrentUserInfo.UserID)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public Dictionary<string, UserGeneratedLevelInfo> GetDownloadedUserGeneratedLevelInfosList()
    {
        if (Application.isEditor)
        {
            return
          _userGeneratedLevelInfoHolder.LevelInfos.Where(
              x => x.Value.AuthorID != FirebaseAuthentication.DESKTOP_USER_ID)
              .ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            return
          _userGeneratedLevelInfoHolder.LevelInfos.Where(
              x => x.Value.AuthorID != FirebaseAuthentication.Instance.CurrentUserInfo.UserID)
              .ToDictionary(x => x.Key, x => x.Value);
        }      
    }

    public UserGeneratedLevelInfo GetUserGeneratedLevelInfoByLevelcode(string levelcode)
    {
        if (_userGeneratedLevelInfoHolder.LevelInfos.ContainsKey(levelcode))
        {
            return _userGeneratedLevelInfoHolder.LevelInfos[levelcode];
        }
        return null;
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