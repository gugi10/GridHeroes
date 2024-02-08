using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SpecialAbilities;

[RequireComponent(typeof(UnitAnimations))]
public class WhirlwindAbility : NovaAbility
{
    private new readonly BasicProperties properties = new() { damage = 1, range = 1 };
    private UnitAnimations unitAnimations;

    protected override void Awake()
    {
        base.Awake();
        unitAnimations = GetComponent<UnitAnimations>();
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
        HashSet<TileEntity> surroundingTiles = _mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range);

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

    public override ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new() { };

        HashSet<TileEntity> surroundingTiles = _mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range);



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
}
