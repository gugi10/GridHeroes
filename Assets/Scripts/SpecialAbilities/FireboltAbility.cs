using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitAnimations))]
public class FireboltAbility : AbilityBase
{
    private int range = 3;
    private int damage = 1;
    private HeroController source;
    private MapEntity map;

    public override void DoSpecialAbility(HeroController source, MapEntity map)
    {
        base.DoSpecialAbility(source, map);

        this.source = source;
        this.map = map;
    }
    void Update()
    {
        if (!readInput)
            return;
        if (map == null)
        {
            return;
        }
        if (MyInput.GetOnWorldUp(map.Settings.Plane()))
        {
            PerformAbility();
        }
    }

    private void PerformAbility()
    {
        var clickPos = MyInput.GroundPosition(map.Settings.Plane());
        TileEntity tile = map.Tile(clickPos);

        if (tile == null)
            return;

        if (tile.IsOccupied)
        {
            if (TileUtilities.CheckTileRange(source.currentTile.TilePos, tile.Position, range) &&
                tile.occupyingHero != source && tile.occupyingHero.ControllingPlayerId != source.ControllingPlayerId)
            {
                tile.occupyingHero.DealDamage(damage);
                source.onSpecialAbilityFinished(); 
            }
            //TODO:Play particle effect

        }
    }
}
