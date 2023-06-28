using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MapView))]
public class MapController : MonoBehaviour
{
    [SerializeField] private MapSettings MapSettings;
    private MapView mapView;
    private MapEntity map;

    private void OnValidate()
    {
        if(MapSettings == null)
            Debug.LogError($"MapSettings is null");
    }

    private void Initialize()
    {
        mapView = GetComponent<MapView>();
        map = new MapEntity(MapSettings, mapView);
    }
    
    //random spawner
    public void SpawnHeroes(List<HeroController> heroesToPosition)
    {
        Initialize();
        var mapSettingsTemp = map.Settings.Tiles;
        List<int> indexArray = Enumerable.Range(0, mapSettingsTemp.Count).ToList();
        foreach(var hero in heroesToPosition)
        {
            var randomTileIndex = Random.Range(0, indexArray.Count - 1);
            var index = indexArray[randomTileIndex];
            indexArray.RemoveAt(randomTileIndex);
            var tile = mapSettingsTemp[index];
            var meshRenderer = hero.GetComponent<MeshRenderer>();
            meshRenderer.material.color = new Color(1f, 1f, hero.ControllingPlayer * 1f);
            hero.SetupHero(new HeroController.HeroControllerPayload(map, tile));
        }
    }
}
