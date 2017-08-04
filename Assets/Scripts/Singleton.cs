using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    //Returns the instance of this singleton.
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }

            return _instance;
        }
    }

    //Create an instance add it to the scene and return it
    public static T Create()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of " + typeof(T) + " already exists");
            return null;
        }
        else
        {
            _instance = (T)FindObjectOfType(typeof(T));

            if (_instance != null)
            {
                Debug.LogError("An instance of " + typeof(T) + " already exists");
                return _instance;
            }

            GameObject go = new GameObject();
            _instance = go.AddComponent<T>() as T;
            go.name = typeof(T).ToString();
            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Destroying GameObject '" + gameObject.name + "'. An instance of " + typeof (T) +
                           " already exists");
            Destroy(this.gameObject);
        }
        else
        {
            Singleton<T>._instance = this.GetComponent<T>();
        }
    }
}