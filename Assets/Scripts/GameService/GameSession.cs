using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : Singleton<GameSession>
{
    [SerializeField] List<BaseConfig> configs;
    private List<IService> services = new();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        services = new List<IService>(){
            new HeroService(GetConfig<HeroesConfig>().heroPrefabs),
            new MapService(GetConfig<MapsConfig>())
            };
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

    public T GetConfig<T>()
        where T : BaseConfig
    {
        var config = configs.Find(x => x is T) as T;
        if (config == null)
            Debug.LogError($"Service: {typeof(T)} is not defined");

        return config;
    }
}
