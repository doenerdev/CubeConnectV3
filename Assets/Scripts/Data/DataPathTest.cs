using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataPathTest : MonoBehaviour
{

    public Text text;
    public Text text2;

    private void Awake()
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/Own/");
        UserGeneratedLevelData levelData = new UserGeneratedLevelData(5);
        levelData.SetLevelName("TestLevel");
        FileStream stream = new FileStream(Application.persistentDataPath + "/Own/bla.lvl", FileMode.Create);
        ProtoBuf.Serializer.Serialize(stream, levelData);
        stream.Close();


        Directory.CreateDirectory(Application.persistentDataPath + "/Own/");

        FileStream fs = File.Open(Application.persistentDataPath + "/Own/bla.lvl", FileMode.Open);
        UserGeneratedLevelData laodedLevelData = ProtoBuf.Serializer.Deserialize<UserGeneratedLevelData>(fs);



        text.text = laodedLevelData.LevelName;

        text2.text = Application.persistentDataPath + "/Own/bla.lvl";
    }
}
