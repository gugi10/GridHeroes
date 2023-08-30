using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitAnimations))]
public class WhirlwindAbility : AbilityBase
{
    public int range = 1;
    public int damage = 1;

    HeroController source;
    MapEntity map;
    UnitAnimations unitAnimations;

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
            PerformAbility();
    }

    private void PerformAbility()
    {
        HashSet<TileEntity> surroundingTiles = map.WalkableTiles(source.currentTile.TilePos, range);
        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
            {
                unitAnimations.PlaySpecialAbillity(animationId);
                tile.occupyingHero.DealDamage(damage);
            }
        }
        source?.onSpecialAbilityFinished();
    }
}
