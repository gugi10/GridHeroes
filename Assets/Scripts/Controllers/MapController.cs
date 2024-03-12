using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RedBjorn.ProtoTiles.Example;

[RequireComponent(typeof(MapView))]
public class MapController : MonoBehaviour
{
    public struct TileRepresentation
    {
        public AccessibleTile representation;
        public TileEntity entity;

        public TileRepresentation(AccessibleTile representation, TileEntity entity)
        {
            this.representation = representation;
            this.entity = entity;
        }
    }

    public List<DeploayableTile> DeployableTiles { get; private set; }
    public List<TileRepresentation> AccessibleTiles { get; private set; } = new List<TileRepresentation>();

    [SerializeField] private MapSettings MapSettings;
    
    private MapView mapView;
    private MapEntity mapEntity;

    private void Awake()
    {
        DeployableTiles = GetComponentsInChildren<DeploayableTile>().ToList();
        var accessibleTiles = GetComponentsInChildren<AccessibleTile>().ToList();
        foreach(AccessibleTile tile in accessibleTiles)
        {
            var entity = GetMapEntity().Tile(tile.transform.position);
            AccessibleTiles.Add(new TileRepresentation(tile, entity));
        }
    }

    [ContextMenu("TestObjCount")]
    public List<TileRepresentation> GetObjectivesControlledByPlayers()
    {
        return AccessibleTiles.
            Where(val => val.representation is ObjectiveTile && val.entity.occupyingHero != null).ToList();
    }
    private void OnValidate()
    {
        if(MapSettings == null)
            Debug.LogError($"MapSettings is null");
    }

    public MapEntity GetMapEntity()
    {
        if(mapEntity == null)
        {
            mapView = GetComponent<MapView>();
            mapEntity = new MapEntity(MapSettings, mapView);
            return mapEntity;
        }

        return mapEntity;
    }
    
    //random spawner
    public void SpawnHeroesRandomly(List<HeroController> heroesToPosition)
    {
        
        var mapSettingsTemp = GetMapEntity().Settings.Tiles.Where(x => x.MovableArea > 0).ToList();
        List<int> indexArray = Enumerable.Range(0, mapSettingsTemp.Count).ToList();
        foreach(var hero in heroesToPosition)
        {
            var randomTileIndex = Random.Range(0, indexArray.Count - 1);
            var index = indexArray[randomTileIndex];
            indexArray.RemoveAt(randomTileIndex);
            var tile = mapSettingsTemp[index];
            hero.SetColor(new Color(1f, 1f, (float)hero.ControllingPlayerId * 1f));
            hero.SetupHero(GetMapEntity(), tile);
        }
    }
    
    public bool GetMapInput()
    {
        return MyInput.GetOnWorldUpFree(GetMapEntity().Settings.Plane());
    }

    public bool GetAlternativeMapInput()
    {
        return MyInput.GetOnWorldUpFreeAlternative(GetMapEntity().Settings.Plane());
    }

    public TileEntity GetTile()
    {
        var clickPos = MyInput.GroundPosition(GetMapEntity().Settings.Plane());
        return GetMapEntity().Tile(clickPos);
    }

    public void EnableHighlightDeployment(bool value)
    {
       foreach(var tile in DeployableTiles)
        {
            tile.Highlight(value);
        }
    }
}
