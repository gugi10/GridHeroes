using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using RedBjorn.ProtoTiles;
using System.Threading.Tasks;

public class Deployment : MonoBehaviour
{
    [SerializeField] HeroSelection heroSelection;
    private Action<List<HeroController>> finishDeploymentCallback;
    private List<HeroListWrapper> heroes = new();
    private MapController map;
    private int spawnedHeroes;
    private List<HeroController> instantiatedHeroes = new();
    private List<HeroController> deploymentHeroes = new();
    private HeroController selectedHero;

    void Update()
    {
        if (map == null || heroes == null)
            return;
        if (map.GetMapInput() && selectedHero != null)
        {
            HandleWorldClick();
        }
    }

    public void Init(MapController map, List<HeroListWrapper> heroes, Action<List<HeroController>> finishDeploymentCallback)
    {
        deploymentHeroes = new List<HeroController>(heroes[0].HeroPrefabs);
        this.finishDeploymentCallback = finishDeploymentCallback;
        this.map = map;
        this.heroes = heroes;
        heroSelection.Init(heroes[0].HeroPrefabs, SetSelectedHero);
    }

    private void SetSelectedHero(HeroController hero)
    {
        selectedHero = hero;
    }

    private void HandleWorldClick()
    {
        TileEntity tile = map.GetTile();
        if (tile == null)
            return;
        if (tile.IsOccupied || tile.Preset.Prefab.GetComponent<DeploayableTile>()?.playerId != 0)
            return;
        if (spawnedHeroes != heroes[0].HeroPrefabs.Count)
        {
            var heroInstance = Instantiate(selectedHero, map.GetMapEntity().WorldPosition(tile), Quaternion.identity);
            heroInstance.ControllingPlayerId = 0;
            heroInstance.SetupHero(map.GetMapEntity(), tile.Data);

            instantiatedHeroes.Add(heroInstance);
            deploymentHeroes.Remove(selectedHero);
            selectedHero = null;
            spawnedHeroes++;
            //After each spawn update the list of  available toggles
            heroSelection.Init(deploymentHeroes, SetSelectedHero);
        }
        if (spawnedHeroes == heroes[0].HeroPrefabs.Count)
        {
            SpawnAiHeroes();
            finishDeploymentCallback.Invoke(instantiatedHeroes);
            return;
        }
        /*var heroInstance = Instantiate(heroes[0].HeroPrefabs[spawnedHeroes], 
            map.GetMapEntity().WorldPosition(tile),Quaternion.identity);
        heroInstance.ControllingPlayerId = 0;
        heroInstance.SetupHero(map.GetMapEntity(), tile.Data);
        instantiatedHeroes.Add(heroInstance);
        spawnedHeroes++;
        if(spawnedHeroes == heroes[0].HeroPrefabs.Count)
        {
            SpawnAiHeroes();
            finishDeploymentCallback.Invoke(instantiatedHeroes);
        }*/
    }

    private void SpawnAiHeroes()
    {
        List<TileEntity> aiTiles = new();
        foreach (var val in map.GetMapEntity().Tiles)
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
