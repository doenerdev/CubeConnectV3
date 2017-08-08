using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager> {

    public FirebaseAuthentication fbAuth;
    public LevelBrowserSortCategory SortCategory;
    public LevelBrowserSortType SortType;
    public string StartIndex;
    public int Range = 2;

    private bool loaded = false;

    // Use this for initialization
    void Awake()
    {
        /*
        UserGeneratedLevelData levelData = new UserGeneratedLevelData(4);
        StageAndLevelDataManager.Instance.AddUserGeneratedLevel(levelData);
        StageAndLevelDataManager.Instance.SaveUserGeneratedLevelDataHolder();

        StageAndLevelDataManager.Instance.LoadUserGeneratedLevelDataHolderAsync(GameManager.Instance.UserLevelsDataPath);*/
        base.Awake();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(
        "https://qube-4b14f.firebaseio.com/");

        DontDestroyOnLoad(this);
        Debug.Log("Done Firebase");

    }

    public void FirebaseConnectionChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log(e.Snapshot.Value);
    }

    private IEnumerator DownloadUserGeneratedLevelAsync(UserGeneratedLevelInfo levelInfo)
    {
        Debug.Log(levelInfo.LevelDataURL);
        WWW file = new WWW(levelInfo.LevelDataURL);
        yield return file;
        Debug.Log("Downlaoded file");

        /*levelInfo.FileLocation = GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", "") + "/" + levelcode + ".lvl";

        Directory.CreateDirectory(GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", ""));
        File.WriteAllBytes(GameManager.Instance.DownloadedUserLevelsDataPath.Replace("file:///", "") + "/" + levelcode + ".lvl", file.bytes);*/
        StageAndLevelDataManager.Instance.SaveDownloadedUserGeneratedLevel(levelInfo, file.bytes);
    }

    public void DownloadUserGeneratedLevel(UserGeneratedLevelInfo levelInfo)
    {
        StartCoroutine(DownloadUserGeneratedLevelAsync(levelInfo));
    }

    public void GenerateUniqueLevelID(UserGeneratedLevelData levelData, UserGeneratedLevelInfo levelInfo, Action<UserGeneratedLevelData, UserGeneratedLevelInfo, string> callback)
    {
        string uid = Guid.NewGuid().ToString().GetHashCode().ToString("x");
        FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild("LevelID").EqualTo(uid).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error Generating UID");
            }
            else if (task.IsCompleted)
            {

                List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                if (snapChildren.Count < 1)
                {
                    callback(levelData, levelInfo, uid);
                }
                else
                {
                    uid = Guid.NewGuid().ToString().GetHashCode().ToString("x");
                    GenerateUniqueLevelID(levelData, levelInfo, callback);
                }
            }
        });
    }

    public void UploadUserGeneratedLevel(UserGeneratedLevelData levelData, UserGeneratedLevelInfo levelInfo)
    {
        GenerateUniqueLevelID(levelData, levelInfo, UploadUserGeneratedLevel);
    }

    private void UploadUserGeneratedLevel(UserGeneratedLevelData levelData, UserGeneratedLevelInfo levelInfo, string levelcode)
    {
        byte[] custom_bytes = null;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        var data_ref_url = "gs://qube-4b14f.appspot.com/user-generated-levels/" + levelcode + ".lvl";
        StorageReference data_ref = storage.GetReferenceFromUrl(data_ref_url);
        string downloadURL = "";

        Dictionary<string, UserGeneratedLevelInfo> levelInfos = StageAndLevelDataManager.Instance.GetUserGeneratedLevelInfosList();
        if (levelInfos.ContainsKey(levelInfo.LevelCode))
        {
            levelInfo = levelInfos[levelInfo.LevelCode];
            levelInfos.Remove(levelInfo.LevelCode);
            levelInfo.LevelCode = levelcode;
            levelInfo.LevelDataURL = data_ref_url;
            levelInfos.Add(levelcode, levelInfo);
        }

        levelData.SyncWithLevelInfo(levelInfo);
        StageAndLevelDataManager.Instance.SaveUserGeneratedLevel(levelData, levelInfo.LevelCode,
            dataPath =>
            {
                UnityMainThreadDispatcher.Enqueue(() => {Debug.Log(dataPath);});
                levelInfo.FileLocation = dataPath;
                StageAndLevelDataManager.Instance.SaveUserGeneratedLevelInfoHolder(() =>
                {

                    levelData.SetLevelID(levelcode); //TODO set all the relevant data
                    Debug.Log(data_ref_url);

                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize<UserGeneratedLevelData>(ms, levelData);
                        custom_bytes = ms.ToArray();
                    }

                    // Upload the file to the path "images/rivers.jpg"
                    data_ref.PutBytesAsync(custom_bytes).ContinueWith((Task<StorageMetadata> task) => {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log(task.Exception.ToString());
                            // Uh-oh, an error occurred!
                        }
                        else
                        {
                            // Metadata contains file metadata such as size, content-type, and download URL.
                            StorageMetadata metadata = task.Result;
                            downloadURL = metadata.DownloadUrl.ToString();
                            Debug.Log("Finished uploading...");

                            //FirebaseLevelData fbLevelData = new FirebaseLevelData(levelData, data_ref_url);
                            Debug.Log("download url = " + downloadURL);
                            levelInfo.LevelDataURL = downloadURL;
                            levelInfos.FirstOrDefault(val => val.Key == levelcode).Value.LevelDataURL = downloadURL;
                            string json = JsonUtility.ToJson(levelInfo);
                            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
                            var childKey = reference.Child("Levels").Push().Key;
                            reference.Child("Levels").Child(childKey).SetRawJsonValueAsync(json);
                        }
                    });
                });

            });
    }

    public void SearchByLevelCode()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild("LevelName").EqualTo("test").GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error Downloading Level Infos");
            }
            else if (task.IsCompleted)
            {
                List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                Debug.Log("Results Count:" + snapChildren.Count);
            }
        });
    }

    public void GetPaginatedLevelInfos(LevelBrowserSortCategory sortCategory, LevelBrowserSortType sortType, string startIndex, int range, int pageIndex, string lastEntryKey, Action<List<DataSnapshot>, string, int, string> callback)
    {
        Debug.Log("Downlod for page:" + pageIndex + " with startIndex:" + startIndex);
        string sortChildNode = "LevelName"; //default sort category
        switch (sortCategory)
        {
            case LevelBrowserSortCategory.AuthorName:
                sortChildNode = "AuthorName";
                break;
        }

        if (sortType == LevelBrowserSortType.Ascending)
        {
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).StartAt(startIndex, lastEntryKey).LimitToFirst(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    string lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                        snapChildren.RemoveAt(0);
                    }

                    if (snapChildren.Count > 0)
                    {
                        lastIndex = snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString();
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                    Debug.Log("Successfull download");
                }
            });
        }
        else
        {
            if (string.IsNullOrEmpty(startIndex)) //TODO allow only alphanumeric input for levelNames and usernames
            {
                startIndex = LevelBrowser.LAST_ALPHABETICAL_STRING;
            }
            Debug.Log("StartIndex:" + startIndex);
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).EndAt(startIndex, lastEntryKey).LimitToLast(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    string lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                    snapChildren.Reverse();

                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                        snapChildren.RemoveAt(0);
                    }
             
                    if (snapChildren.Count > 0)
                    {
                        lastIndex = snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString();
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                }
            });
        }   
    }

    public void GetPaginatedLevelInfos(LevelBrowserSortCategory sortCategory, LevelBrowserSortType sortType, double startIndex, int range, int pageIndex, string lastEntryKey, Action<List<DataSnapshot>, double, int, string> callback)
    {
        string sortChildNode = "UserRating"; //default sort category

        if (sortType == LevelBrowserSortType.Ascending)
        {
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).StartAt(startIndex, lastEntryKey).LimitToFirst(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    double lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                
                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                       snapChildren.RemoveAt(0);
                    }
                    Debug.Log("Children Count:" + snapChildren.Count);
                 
                    if (snapChildren.Count > 0)
                    {
                        lastIndex = double.Parse(snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString());
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }    

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                    Debug.Log("Successfull download");
                }
            });
        }
        else
        {
            if (startIndex < 0) //TODO allow only alphanumeric input for levelNames and usernames
            {
                startIndex = Double.MaxValue;
            }
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).EndAt(startIndex, lastEntryKey).LimitToLast(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    double lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                    snapChildren.Reverse();

                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                        snapChildren.RemoveAt(0);
                    }

                    if (snapChildren.Count > 0)
                    {
                        lastIndex = double.Parse(snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString());
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                }
            });
        }
    }

    public void GetPaginatedLevelInfos(LevelBrowserSortCategory sortCategory, LevelBrowserSortType sortType, int startIndex, int range, int pageIndex, string lastEntryKey, Action<List<DataSnapshot>, int, int, string> callback)
    {
        string sortChildNode = "Date"; //default sort category
        Debug.Log(startIndex);
        if (sortType == LevelBrowserSortType.Ascending)
        {
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).StartAt(startIndex, lastEntryKey).LimitToFirst(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    int lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes

                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                        snapChildren.RemoveAt(0);
                    }

                    if (snapChildren.Count > 0)
                    {
                        lastIndex = int.Parse(snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString());
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                }
            });
        }
        else
        {
            if (startIndex < 0) //TODO allow only alphanumeric input for levelNames and usernames
            {
                startIndex = Int32.MaxValue;
            }
            FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild(sortChildNode).EndAt(startIndex, lastEntryKey).LimitToLast(range).GetValueAsync().ContinueWith((Task<DataSnapshot> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error Downloading Level Infos");
                }
                else if (task.IsCompleted)
                {
                    int lastIndex = startIndex;
                    string newLastEntryKey = null;
                    List<DataSnapshot> snapChildren = task.Result.Children.ToList(); //get the children nodes
                    snapChildren.Reverse();

                    if (pageIndex > 0 && snapChildren.Count > 0)
                    {
                        snapChildren.RemoveAt(0);
                    }

                    if (snapChildren.Count > 0)
                    {
                        lastIndex = int.Parse(snapChildren[snapChildren.Count - 1].Child(sortChildNode).Value.ToString());
                        newLastEntryKey = snapChildren[snapChildren.Count - 1].Key;
                    }

                    callback(snapChildren, lastIndex, pageIndex, newLastEntryKey);
                }
            });
        }
    }

    private T Deserialize<T>(byte[] param)
    {
        using (MemoryStream ms = new MemoryStream(param))
        {
            IFormatter br = new BinaryFormatter();
            return (T)br.Deserialize(ms);
        }
    }
}


public struct UserGeneratedLevelDBInfo
{
    public string LevelURL;
    public string AuthorID;
    public string AuthorName;

}