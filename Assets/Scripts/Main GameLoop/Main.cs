using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Main : MonoBehaviour ,ISingleton
{
    void Awake()
    {
        SingletonLized();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {

    }

    void OnSceneUnloaded(Scene arg0)
    {

    }


    void Start () 
    {
        InitUIMain();
        InitObjectPool();
        InitMusicPlayer();

	}

    public void SingletonLized()
    {
        if (FindObjectsOfType<Main>().Length != 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void InitObjectPool()
    {
        gameObject.AddComponent<ObjectPool.ObjectPool>();
    }

    void InitMusicPlayer()
    {
        gameObject.AddComponent<MusicPlayer>();
    }

    void InitUIMain()
    {
        //GameObject uiMain = GameObject.FindGameObjectWithTag("UI");
        //gameObject.AddComponent<UIMain>();
    }

}
