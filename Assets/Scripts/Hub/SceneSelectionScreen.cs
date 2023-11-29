using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectionScreen : ScreenBase
{
    public override ScreenIdentifiers Identifier() => ScreenIdentifiers.LevelSelect;

    [SerializeField] private MapSlot mapPrefab;
    [SerializeField] private MapsConfig mapsConfigObj;

    private void Awake()
    {
        foreach(var map in mapsConfigObj.GetMapConfigs())
        {
            var spawnedMap = Instantiate(mapPrefab, transform);
            spawnedMap.Initialize(map);
        }
    }
}
