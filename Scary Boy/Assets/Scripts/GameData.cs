using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData data;
    private void Awake()
    {
        if(data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(data!= this)
        {
            Destroy(gameObject);
        }
    }

    public DataObject dataObject;

    [System.Serializable]
    public class DataObject
    {
        public float highScore;
        public bool tutorialDone;
    }
}
