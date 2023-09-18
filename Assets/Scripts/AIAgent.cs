using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIAgent : MonoBehaviour, IPlayer
{
    public int Id { get; set; }
    private MapController map;
    private List<HeroController> allHeroes = new List<HeroController>();

    public void Init(MapController map, List<HeroController> allHeroes, int playerId)
    {
        this.map = map;
        this.allHeroes = allHeroes;
        Id = playerId;
    }
    public void SetActiveState(bool flag)
    {
        if (!flag)
        {
            return;
        }

        var aiHeroes = allHeroes.Where(hero => hero.ControllingPlayerId == Id).ToList();
        var randomIdx = Random.Range(0, aiHeroes.Count);
        var randomAiHero = aiHeroes[randomIdx];
        Debug.Log($"Randomly chosen hero = {randomAiHero.gameObject.name}");


        if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Special))
        {
            var ability = randomAiHero.specialAbilities[0];
            var abilitySpec = ability.GetAbilitySpec();

            switch (abilitySpec.kind)
            {
                case AbilityKind.Whirlwind:
                    {
                        var properties = (WhirlwindAbility.Properties)abilitySpec.properties;
                        var foundEnemies = FindEnemiesInRange(randomAiHero, properties.range);
                        if (foundEnemies.Count < 2) {
                            break;
                        }

                        ability.DoSpecialAbility(randomAiHero, map.mapEntity);
                        ability.PerformAbility(map.mapEntity.Tile(foundEnemies.First().currentTile.TilePos));
                        return;
                    }
                case AbilityKind.Bolt:
                    {
                        var properties = (FireboltAbility.Properties)abilitySpec.properties;
                        var foundEnemy = FindEnemyInRange(randomAiHero, properties.range);
                        if (foundEnemy)
                        {
                            ability.DoSpecialAbility(randomAiHero, map.mapEntity);
                            ability.PerformAbility(map.mapEntity.Tile(foundEnemy.currentTile.TilePos));
                            return;
                        }
                        break;

                    }
            }

        }


        if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Attack) 
            || TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Special))
        {
            var foundEnemy = FindEnemyInRange(randomAiHero, randomAiHero.GetHeroStats().Item1.WeaponRange);
            if (foundEnemy)
            {
                if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Attack))
                {
                    var targetTile = map.mapEntity.Tile(foundEnemy.currentTile.TilePos);
                    randomAiHero.Attack(targetTile);
                    return;
                }
            }
        }

        if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Move)
            || TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Special))
        {
            //trzeba przeliczyc pathy dla kazdego wealkable tile'a i odfiltrowac te ktore sa w zasiegu iczy nic nie blokuje.
            var walkableTiles = map.mapEntity.WalkableTiles(randomAiHero.currentTile.TilePos, randomAiHero.GetHeroStats().Item1.Move).Where(x => !x.IsOccupied).ToList();
            var randomWalkableTileIdx = Random.Range(0, walkableTiles.Count);
            var selectedRandomTile = walkableTiles[randomWalkableTileIdx].Data.TilePos;
            Debug.Log($"selected Tile {selectedRandomTile} for {randomAiHero.gameObject.name}");
            var path = map.mapEntity.PathTiles
                (randomAiHero.transform.position, map.mapEntity.WorldPosition(walkableTiles[randomWalkableTileIdx].Data.TilePos), randomAiHero.GetHeroStats().Item1.Move);
            string pathstring = "";

            foreach (var tiles in path)
            {
                pathstring += $"{tiles.Data.TilePos}";
            }
            Debug.Log($"Path string {pathstring}");
            if (path != null || path.Count > 0)
                randomAiHero.MoveByPath(path);
            //randomAiHero.Move(walkableTiles[randomWalkableTileIdx]);
            return;
        }

        TurnSequenceController.Instance.FinishTurn(TurnSequenceController.Instance.GetPlayerRemainingActions(Id)[0]);
    }
    private HeroController FindEnemyInRange(HeroController aiHero, int range)
    {
        var enemies = allHeroes.Where(hero => hero.ControllingPlayerId != Id).ToList();
        var enemyInRange = enemies.FirstOrDefault(hero => TileUtilities.AreTilesInRange(hero.currentTile.TilePos, aiHero.currentTile.TilePos, range));
        // Debug.Log($"Player hero  in range= {playerHeroInRange.gameObject.name}");
        return enemyInRange;
    }
    private List<HeroController> FindEnemiesInRange(HeroController aiHero, int range)
    {
        var enemies = allHeroes.Where(hero => hero.ControllingPlayerId != Id).ToList();
        var enemiesInRange = enemies.FindAll(hero => TileUtilities.AreTilesInRange(hero.currentTile.TilePos, aiHero.currentTile.TilePos, range));
        // Debug.Log($"Player hero  in range= {playerHeroInRange.gameObject.name}");
        return enemiesInRange;
    }
}
