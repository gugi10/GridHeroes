using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MapConfigs", menuName = "ScriptableObjects/MapConfigs", order = 2)]

public class MapsConfig : ScriptableObject
{
    [SerializeField] private List<MapConfig> mapConfigs;

    public List<MapConfig> GetMapConfigs()
    {
        return mapConfigs;
    }
}

[System.Serializable]
public class MapConfig
{
    [SerializeField] private Sprite image;
    [SerializeField] private string mapName;
    [SerializeField] private SceneLoader.SceneEnum sceneId;

    public string GetMapName()
    {
        return mapName;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public SceneLoader.SceneEnum GetSceneId()
    {
        return sceneId;
    }
}

