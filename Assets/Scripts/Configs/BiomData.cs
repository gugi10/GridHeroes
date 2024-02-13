using System.Collections.Generic;

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