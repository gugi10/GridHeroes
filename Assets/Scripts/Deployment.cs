using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using RedBjorn.ProtoTiles;

public class Deployment : MonoBehaviour
{
    private Action<List<HeroController>> finishDeploymentCallback;
    private List<HeroListWrapper> heroes = new();
    private MapController map;
    private int spawnedHeroes;
    private List<HeroController> instantiatedHeroes = new();

    void Update()
    {
        if (map == null || heroes == null)
            return;
        if (map.GetMapInput())
        {
            HandleWorldClick();
        }
    }

    public void Init(MapController map, List<HeroListWrapper> heroes, Action<List<HeroController>> finishDeploymentCallback)
    {
        this.finishDeploymentCallback = finishDeploymentCallback;
        this.map = map;
        this.heroes = heroes;
    }

    private void HandleWorldClick()
    {
        TileEntity tile = map.GetTile();
        if (tile == null)
            return;
        if (tile.IsOccupied || tile.Preset.Prefab.GetComponent<DeploayableTile>()?.playerId != 0)
            return;
        var heroInstance = Instantiate(heroes[0].HeroPrefabs[spawnedHeroes], 
            map.GetMapEntity().WorldPosition(tile),Quaternion.identity);
        heroInstance.ControllingPlayerId = 0;
        heroInstance.SetupHero(map.GetMapEntity(), tile.Data);
        instantiatedHeroes.Add(heroInstance);
        spawnedHeroes++;
        if(spawnedHeroes == heroes[0].HeroPrefabs.Count)
        {
            SpawnAiHeroes();
            finishDeploymentCallback.Invoke(instantiatedHeroes);
        }
    }

    private void SpawnAiHeroes()
    {
        List<TileEntity> aiTiles = new();
        foreach(var val in map.GetMapEntity().Tiles)
        {
            if (val.Value.Preset.Prefab.GetComponent<DeploayableTile>()?.playerId == 1)
                aiTiles.Add(val.Value);
        }

        heroes[1].HeroPrefabs.ForEach(hero =>
        {
            var availableTiles = aiTiles.Where(val => !val.IsOccupied).ToList();
            var randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            var heroInstance = Instantiate(hero,
                map.GetMapEntity().WorldPosition(availableTiles[randomIndex]), Quaternion.identity);
            heroInstance.ControllingPlayerId = 1;
            heroInstance.SetupHero(map.GetMapEntity(), availableTiles[randomIndex].Data);
            instantiatedHeroes.Add(heroInstance);
        });
    }
}
