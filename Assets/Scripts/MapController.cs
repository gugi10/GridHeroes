using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RedBjorn.ProtoTiles.Example;

[RequireComponent(typeof(MapView))]
public class MapController : MonoBehaviour
{
    [SerializeField] private MapSettings MapSettings;
    private MapView mapView;
    public MapEntity map;

    private void OnValidate()
    {
        if(MapSettings == null)
            Debug.LogError($"MapSettings is null");
    }

    private void Awake()
    {
        mapView = GetComponent<MapView>();
        map = new MapEntity(MapSettings, mapView);
    }
    
    //random spawner
    public void SpawnHeroesRandomly(List<HeroController> heroesToPosition)
    {
        
        var mapSettingsTemp = map.Settings.Tiles.Where(x => x.MovableArea > 0).ToList();
        List<int> indexArray = Enumerable.Range(0, mapSettingsTemp.Count).ToList();
        foreach(var hero in heroesToPosition)
        {
            var randomTileIndex = Random.Range(0, indexArray.Count - 1);
            var index = indexArray[randomTileIndex];
            indexArray.RemoveAt(randomTileIndex);
            var tile = mapSettingsTemp[index];
            hero.SetColor(new Color(1f, 1f, hero.ControllingPlayerId * 1f));
            hero.SetupHero(map, tile);
        }
    }

    public bool GetMapInput()
    {
        return MyInput.GetOnWorldUp(map.Settings.Plane());
    }

    public TileEntity GetTile()
    {
        var clickPos = MyInput.GroundPosition(map.Settings.Plane());
        return map.Tile(clickPos);
    }
}
