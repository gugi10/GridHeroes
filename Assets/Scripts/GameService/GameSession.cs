using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : Singleton<GameSession>
{
    [SerializeField] HeroesConfig heroesConfig;
    private List<IService> services = new();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        services.Add(new HeroService(heroesConfig.heroPrefabs));
        SceneLoader.LoadScene(SceneLoader.SceneEnum.Hub);
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
