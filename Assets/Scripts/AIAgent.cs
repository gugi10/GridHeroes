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

        var foundEnemy = FindEnemyInRange(randomAiHero);
        if (foundEnemy)
        {
            if(TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Attack))
            {
                var targetTile = map.mapEntity.Tile(foundEnemy.currentTile.TilePos);
                randomAiHero.Attack(targetTile);
                return;
            }
        }

        var walkableTiles = map.mapEntity.WalkableTiles(randomAiHero.currentTile.TilePos, randomAiHero.GetHeroStats().Item1.Move).Where(tile => !tile.IsOccupied).ToList();
        var randomWalkableTileIdx = Random.Range(0, walkableTiles.Count);
        randomAiHero.Move(walkableTiles[randomWalkableTileIdx]);
    }

    

    private HeroController FindEnemyInRange(HeroController aiHero)
    {
        var playerHeroes = allHeroes.Where(hero => hero.ControllingPlayerId != Id).ToList();
        var playerHeroInRange = playerHeroes.FirstOrDefault(hero => TileUtilities.AreTilesInRange(hero.currentTile.TilePos, aiHero.currentTile.TilePos, aiHero.GetHeroStats().Item1.WeaponRange));
        // Debug.Log($"Player hero  in range= {playerHeroInRange.gameObject.name}");
        return playerHeroInRange;
    }
}
