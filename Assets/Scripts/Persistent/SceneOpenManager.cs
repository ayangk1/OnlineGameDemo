using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpenManager : SingletonMonoBehaviour<SceneOpenManager>
{
    public GameObject loadObj;

    private void Start()
    {
        loadObj.SetActive(false);
    }

    public void LoadScene(int from,int to)
    {
        StartCoroutine(EnterScene(from,to));
    }
    
    private IEnumerator EnterScene(int from,int to)
    {
        loadObj.SetActive(true);
        yield return SceneManager.UnloadSceneAsync(from);
        yield return SceneManager.LoadSceneAsync(to,LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
        loadObj.SetActive(false);
    }
}
