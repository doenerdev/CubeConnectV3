using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Storage;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine;

public class FirebaseConnectionTest : MonoBehaviour
{

    public FirebaseAuthentication fbAuth;

    private bool loaded = false;
    private System.IO.Stream stream;

	// Use this for initialization
	void Start ()
	{
     //   StageAndLevelDataManager.Instance.LoadUserGeneratedLevelDataHolderAsync(GameManager.Instance.UserLevelsDataPath);

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(
        "https://qube-4b14f.firebaseio.com/");

	    return;
        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference levelRef = storage.GetReferenceFromUrl("gs://qube-4b14f.appspot.com/user-generated-levels/new-level.lvl");

        const long maxAllowedSize = 1 * 1024 * 1024;
        levelRef.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                Debug.Log("Oh no");
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                UserGeneratedLevelData data = Deserialize<UserGeneratedLevelData>(fileContents);
                Debug.Log(data.LevelName);

            }
        });
    }

    public void UploadUserGeneratedLevel(UserGeneratedLevelData levelData)
    {
        byte[] custom_bytes = null;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        var data_ref_url = "gs://qube-4b14f.appspot.com/user-generated-levels/" + levelData.LevelName + "bla" + ".lvl";
        StorageReference data_ref = storage.GetReferenceFromUrl(data_ref_url);

        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, levelData);
            custom_bytes = ms.ToArray();
        }

        var level_metadata = new Firebase.Storage.MetadataChange
        {
            CustomMetadata = new Dictionary<string, string>
            {
                {"difficulty", levelData.Difficulty.ToString()},
                {"rating", "0"},
                { "qtyRatings", levelData.QtyRatings.ToString()},
                { "levelName", levelData.LevelName}
            }
        };

        // Upload the file to the path "images/rivers.jpg"
        data_ref.PutBytesAsync(custom_bytes, level_metadata).ContinueWith((Task<StorageMetadata> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                StorageMetadata metadata = task.Result;
                string download_url = metadata.DownloadUrl.ToString();
                Debug.Log("Finished uploading...");
                Debug.Log("download url = " + download_url);
            }
        });


        FirebaseLevelData fbLevelData = new FirebaseLevelData(levelData, data_ref_url);
        string json = JsonUtility.ToJson(fbLevelData);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var childKey = reference.Child("Levels").Push().Key;
        reference.Child("Levels").Child(childKey).SetRawJsonValueAsync(json);
    }

    private void DownloadDB()
    {
        //retrieving all levels from the levels master node
        FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild("AuthorName").GetValueAsync().ContinueWith((Task<DataSnapshot> task) => {
            if (task.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                var snapChildern = task.Result.Children; //get the children nodes
                var listChildern = snapChildern.ToList(); //cast the children to a list
                var value = listChildern[0].Child("LevelDataURL").Value; // get a single value from a level node

                Debug.Log(value);
                Debug.Log(listChildern.Count);
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("Levels").OrderByChild("AuthorID").ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        var snapChildern = args.Snapshot.Children; //get the children nodes
        var listChildern = snapChildern.ToList(); //cast the children to a list
        var value = listChildern[0].Child("LevelDataURL").Value; // get a single value from a level node

        Debug.Log(value);
        Debug.Log(listChildern.Count);
        // Do something with the data in args.Snapshot
    }

    private T Deserialize<T>(byte[] param)
    {
        using (MemoryStream ms = new MemoryStream(param))
        {
            IFormatter br = new BinaryFormatter();
            return (T)br.Deserialize(ms);
        }
    }

    private void DoSomething()
    {
        
    }

	// Update is called once per frame
	void Update ()
	{
	    if (loaded == true)
	    {
	        Debug.Log("loaded");
            Debug.Log(stream);
	    }

	    if (Input.GetKeyUp("r"))
	    {
	        DownloadDB();
	    }
	}
}

