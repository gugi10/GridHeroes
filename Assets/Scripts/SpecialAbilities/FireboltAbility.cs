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
        this.source = source;
        this.map = map;
    }
    public override void ProcessInput()
    {
        if (map == null)
        {
            return;
        }
        if (MyInput.GetOnWorldUp(map.Settings.Plane()))
        {
            var clickPos = MyInput.GroundPosition(map.Settings.Plane());
            TileEntity tile = map.Tile(clickPos);
            PerformAbility(tile);
        }
    }
    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { target = AbilityTarget.SingleEnemy, effect = AbilityEffect.Damage, range = this.range};
    }

    public override void PerformAbility(TileEntity chosenTile)
    {

        if (chosenTile == null)
            return;

        if (chosenTile.IsOccupied)
        {
            if (TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, range) &&
                chosenTile.occupyingHero != source && chosenTile.occupyingHero.ControllingPlayerId != source.ControllingPlayerId)
            {
                chosenTile.occupyingHero.DealDamage(damage);
                source.onSpecialAbilityFinished(); 
            }
            //TODO:Play particle effect

        }
    }

}
