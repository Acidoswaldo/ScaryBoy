using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof (GameData))]
public class SaveSystem : MonoBehaviour
{
    GameData _data;
    public static SaveSystem instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            _data = GetComponent<GameData>();
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
     
    }
    public void Save()
    {
        Debug.Log("Saved");
        string json = JsonUtility.ToJson(_data.dataObject);
        File.WriteAllText(Application.persistentDataPath + "/save.txt", json);
    }

    public  void Load()
    {
        Debug.Log("Loaded");
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/save.txt");
            var saveObject = JsonUtility.FromJson<GameData.DataObject>(json);
            GameData.data.dataObject = saveObject;
        }
    }
}
