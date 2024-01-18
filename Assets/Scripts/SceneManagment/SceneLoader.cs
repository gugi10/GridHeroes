using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class SceneLoader
{
    private static SceneEnum current = SceneEnum.Hub;
    private static string currentMapString = $"Hub";
    public enum SceneEnum
    {
        Hub,
        LoadingScene,
        Map01,
        Map02,
    }

    public static Action loadingCallback;

    public static void LoadScene(SceneEnum scene)
    {
        current = scene;
        loadingCallback = () => SceneManager.LoadScene(scene.ToString());
        SceneManager.LoadScene(SceneEnum.LoadingScene.ToString());
    }

    public static void LoadScene(string scene)
    {
        currentMapString = scene;
        loadingCallback = () => SceneManager.LoadScene(scene);
        SceneManager.LoadScene("LoadingScene");
    }
}
