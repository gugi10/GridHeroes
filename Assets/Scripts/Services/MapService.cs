using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MapService : IService
{
    private List<BiomData> playerBioms = new();
    private MapData currentMap;

    public MapService(MapsConfig mapsConfig)
    {
        //TODO: To powinno byc zastapione ladowaniem zapisanego pliku zawierajacego biomData,
        //config powinien sluzyc tylko do odczytywania przypisanych scen,ikon

        List<MapData> maps = new List<MapData>();
        var bioms = mapsConfig.GetBiomConfigs();
        for (int i = 0; i < bioms.Count; i ++)
        {
            for (int j = 0; j <  bioms[i].GetMaps().Count; j++)
            {
                var mapToAdd = bioms[i].GetMaps()[j];
                maps.Add(new MapData(mapToAdd.GetMapName(), false, false, j, i));
            }

            playerBioms.Add(new BiomData(maps, bioms[i].GetName(), false, false));
        }
        

        //Unlock first biom and first level
        playerBioms[0].IsUnlocked = true;
        playerBioms[0].MapData[0].IsUnlocked = true;
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

        currentMap.IsCompleted = true;
        if (currentMap.Id + 1 >= playerBioms[currentMap.BiomId].MapData.Count)
        {
            //Biom completed
            playerBioms[currentMap.BiomId].IsCompleted = true;
            if (currentMap.BiomId + 1 >= playerBioms.Count)
            {
                //Game finished easy
                return;
            }
            
            //unlock new biom
            playerBioms[currentMap.BiomId + 1].MapData[0].IsUnlocked = true;
            playerBioms[currentMap.BiomId + 1].IsUnlocked = true;
            return;
        }
        //unlock next map
        playerBioms[currentMap.BiomId].MapData[currentMap.Id + 1].IsUnlocked = true;
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
    public bool IsUnlocked;
    public bool IsCompleted;
    public int Id { get; private set; }
    public int BiomId { get; private set; }

    public MapData(string mapName, bool isUnlocked, bool isCompleted, int id, int biomId)
    {
        MapName = mapName;
        IsUnlocked = isUnlocked;
        IsCompleted = isCompleted;
        Id = id;
        BiomId = biomId;
    }
}

[System.Serializable]
public class BiomData
{
    public List<MapData> MapData { get; private set; }
    public string BiomName { get; private set; }
    public bool IsUnlocked; 
    public bool IsCompleted;
    public BiomData(List<MapData> mapData, string biomName, bool isUnlocked, bool isCompleted)
    {
        MapData = mapData;
        IsUnlocked = isUnlocked;
        BiomName = biomName;
        IsCompleted = isCompleted;
    }
}


