using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(UnitAnimations))]
public class WhirlwindAbility : AbilityBase
{
    private BasicProperties properties = new() { damage = 1, range = 1 };

    HeroController source;
    MapEntity mapEntity;
    UnitAnimations unitAnimations;
    AffectedTilesHiglight highlight;

    private void Awake()
    {
        unitAnimations = GetComponent<UnitAnimations>();
        highlight = new AffectedTilesHiglight();
    }

    public override void DoSpecialAbility(HeroController source, MapEntity map)
    {
        this.source = source;
        this.mapEntity = map;
    }

    public override void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0))
            PerformAbility(null);
    }

    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.Whirlwind, properties = properties };
    }

    public override void PerformAbility(TileEntity chosenTile)
    {
        HashSet<TileEntity> surroundingTiles = mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range);

        unitAnimations.PlaySpecialAbillity(animationId);
        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
            {
                tile.occupyingHero.DealDamage(properties.damage);
            }
        }
        source?.onSpecialAbilityFinished();
    }

    public override bool CanBeUsedOnTarget(TileEntity chosenTile)
    {
        return false;
    }

    public override ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new() { };

        HashSet<TileEntity> surroundingTiles = mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range);



        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
            {
                if (target.GetHeroStats().current.Health <= this.properties.damage)
                {
                    modifiers.enemiesKilled += 1;
                }
                modifiers.inflictedDamage += target.GetHeroStats().current.Health;
            }
        }

        return modifiers;
    }

    public override void HighlightAffectedTiles(MapController mapController)
    {
        highlight.HighlighTile(mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range), mapController);
    }

    public override void DisableHiglight(MapController map)
    {
        highlight.DisableHiglight(map);
    }
}
