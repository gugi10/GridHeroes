using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitAnimations))]
public class PushAbility : AbilityBase
{
    private BasicProperties properties = new() { damage = 0, range = 1 };
    private HeroController source;
    private MapEntity map;
    private UnitAnimations unitAnimation;

    private void Awake()
    {
        unitAnimation = GetComponent<UnitAnimations>();
    }

    public override void DoSpecialAbility(HeroController source, MapEntity map)
    {
        this.source = source;
        this.map = map;
    }

    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.PushStrike, properties = properties };
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

    public override void PerformAbility(TileEntity chosenTile)
    {

        if (chosenTile == null)
            return;

        if (chosenTile.IsOccupied)
        {
            if (TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, properties.range) &&
                chosenTile.occupyingHero != source && chosenTile.occupyingHero.ControllingPlayerId != source.ControllingPlayerId)
            {

                source.LookAt(map.WorldPosition(chosenTile));
                Vector3Int newPos = new Vector3Int((chosenTile.occupyingHero.currentTile.TilePos.x - source.currentTile.TilePos.x) + chosenTile.occupyingHero.currentTile.TilePos.x,
                    (chosenTile.occupyingHero.currentTile.TilePos.y - source.currentTile.TilePos.y)+ chosenTile.occupyingHero.currentTile.TilePos.y,
                    (chosenTile.occupyingHero.currentTile.TilePos.z - source.currentTile.TilePos.z)+ chosenTile.occupyingHero.currentTile.TilePos.z);
                var newTile = map.Tile(newPos);
                if(newTile != null)
                    chosenTile.occupyingHero.Move(newTile);
                source?.onSpecialAbilityFinished();
            }
            unitAnimation.PlaySpecialAbillity(animationId);
        }
    }


    public override bool CanBeUsedOnTarget(TileEntity chosenTile)
    {
        if (!chosenTile.IsOccupied)
        {
            return false;
        }

        if (!TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, properties.range))
        {
            return false;
        }

        if (chosenTile.occupyingHero == source || chosenTile.occupyingHero.ControllingPlayerId == source.ControllingPlayerId)
        {
            return false;
        }

        return true;
    }
    public override ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new ScoreModifiers { };

        if (target.GetHeroStats().current.Health <= this.properties.damage)
        {
            modifiers.enemiesKilled = 1;
        }
        modifiers.inflictedDamage = target.GetHeroStats().current.Health;
        // TODO: CONSIDER PUSH FOR MODIFIERS

        return modifiers;
    }
}
