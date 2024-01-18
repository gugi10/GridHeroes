using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapService : IService
{
    private List<BiomData> playerBioms = new();
    private MapData currentMap;

    public MapService(MapsConfig mapsConfig)
    {
        //TODO: To powinno byc zastapione ladowaniem zapisanego pliku zawierajacego biomData,
        //config powinien sluzyc tylko do odczytywania przypisanych scen,ikon

        List<MapData> maps = new List<MapData>();
        foreach(var biom in mapsConfig.GetBiomConfigs())
        {
            foreach(var map in biom.GetMaps())
            {
                maps.Add(new MapData(map.GetMapName(), false, false));
            }
            playerBioms.Add(new BiomData(new List<MapData>(maps), biom.GetName(), false));
            maps.Clear();
        }

        //Unlock first biom and first level
        playerBioms[0] = new BiomData(playerBioms[0].MapData, playerBioms[0].BiomName, true);
        playerBioms[0].MapData[0] = new MapData(playerBioms[0].MapData[0].MapName, true, false);
    }

    public List<MapData> GetMapsFromBiom(int id)
    {
        return playerBioms[id].MapData;
    }

    public void CompleteCurrentMap()
    {
        if(currentMap == null)
        {
            Debug.LogError($"No map was loaded in this session");
            return;
        }

        currentMap = new MapData(currentMap.MapName, true, true);

    }

    //TODO: Later use bionName to find proper one
    public void LoadMap(string mapName, string biomName = null)
    {
        var mapToLoad = playerBioms[0].MapData.Find(val => val.MapName == mapName);
        if(!mapToLoad.IsUnlocked)
        {
            Debug.LogError($"Cannot load this map its locked, check unlock status before calling this");
            return;
        }
        currentMap = mapToLoad;
        SceneLoader.LoadScene(mapToLoad.MapName);
    }
}

[System.Serializable]
public class MapData
{
    public string MapName { get; private set; }
    public bool IsUnlocked { get; private set; }
    public bool IsCompleted { get; private set; }

    public MapData(string mapName, bool isUnlocked, bool isCompleted)
    {
        MapName = mapName;
        IsUnlocked = isUnlocked;
        IsCompleted = isCompleted;
    }
}

[System.Serializable]
public class BiomData
{
    public List<MapData> MapData { get; private set; }
    public bool IsUnlocked { get; private set; }
    public string BiomName { get; private set; }
    public BiomData(List<MapData> mapData, string biomName, bool isUnlocked)
    {
        MapData = mapData;
        IsUnlocked = isUnlocked;
        BiomName = biomName;
    }
}


