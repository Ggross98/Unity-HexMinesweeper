using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static volatile T instance;
    private static GameObject _container;
    private static object syncRoot = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        T[] instance1 = FindObjectsOfType<T>();
                        if (instance1 != null)
                        {
                            for (var i = 0; i < instance1.Length; i++)
                            {
                                Destroy(instance1[i].gameObject);
                            }
                        }
                    }
                }
                GameObject go = new GameObject(typeof(T).Name);
                _container = go;
                DontDestroyOnLoad(go);
                instance = go.AddComponent<T>();
            }
            return instance;
        }

    }


    public virtual void Awake()
    {
        T t = gameObject.GetComponent<T>();
        if (t == null)
            t = gameObject.AddComponent<T>();
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);

            instance = t;
        }
        if (instance != t)
        {
            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
            if (monos.Length > 1)
            {
                Destroy(t);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}
