using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MapConfigs", menuName = "ScriptableObjects/MapConfigs", order = 2)]

public class MapsConfig : BaseConfig
{
    [SerializeField] private List<BiomConfig> biomConfigs;

    public List<BiomConfig> GetBiomConfigs()
    {
        return biomConfigs;
    }

    public List<MapConfig> GetMapsFromBiom(int id)
    {
        return biomConfigs[id].GetMaps();
    }
}

[System.Serializable]
public class MapConfig
{
    [SerializeField] private Sprite image;
    [SerializeField] private string mapName;
    [SerializeField] private SceneLoader.SceneEnum sceneId;

    public string GetMapName() => mapName;

    public Sprite GetImage() => image;

    public SceneLoader.SceneEnum GetSceneId() => sceneId;
}

[System.Serializable]
public class BiomConfig
{
    [SerializeField] private List<MapConfig> maps;
    [SerializeField] private string name;

    public List<MapConfig> GetMaps() => maps;
    public string GetName() => name;
}

