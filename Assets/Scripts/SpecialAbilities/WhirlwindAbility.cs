using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FireboltAbility;

[RequireComponent(typeof(UnitAnimations))]
public class WhirlwindAbility : AbilityBase
{
    private Properties properties = new() { damage = 1, range = 1 };

    HeroController source;
    MapEntity map;
    UnitAnimations unitAnimations;
    public struct Properties
    {
        public int range;
        public int damage;
    }


    private void Awake()
    {
        unitAnimations = GetComponent<UnitAnimations>();
    }

    public override void DoSpecialAbility(HeroController source, MapEntity map)
    {
        this.source = source;
        this.map = map;
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
        HashSet<TileEntity> surroundingTiles = map.WalkableTiles(source.currentTile.TilePos, properties.range);

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
}
