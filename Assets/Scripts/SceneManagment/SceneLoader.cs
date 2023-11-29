using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class SceneLoader
{
    private static SceneEnum current = SceneEnum.Hub;
    public enum SceneEnum
    {
        Hub,
        Map01,
        LoadingScene
    }
    public static Action loadingCallback;

    public static void LoadScene(SceneEnum scene)
    {
        current = scene;
        loadingCallback = () => SceneManager.LoadScene(scene.ToString());
        SceneManager.LoadSceneAsync(SceneEnum.LoadingScene.ToString());
    }
}
