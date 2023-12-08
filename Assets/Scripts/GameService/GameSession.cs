using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private List<IService> services = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        services.Add(new HeroService());
        SceneLoader.LoadScene(SceneLoader.SceneEnum.Hub);
    }

    private void Start()
    {
        //Show popup at the end of the game to do is required to make the screensController load when the hub finishes loading
    }

    public T GetService<T>()
        where T : class, IService
    {
        var service = services.Find(x => x is T) as T;
        if (service == null)
            Debug.LogError($"Service: {typeof(T)} is not defined");

        return service;
    }
}
