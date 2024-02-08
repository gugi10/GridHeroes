using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using SpecialAbilities;
using UnityEngine;

[RequireComponent(typeof(UnitAnimations))]
public class PushAbility : TargetableAbility
{
    private new readonly BasicProperties properties = new() { damage = 1, range = 1 };
    private UnitAnimations unitAnimation;

    protected override void Awake()
    {
        base.Awake();
        unitAnimation = GetComponent<UnitAnimations>();
    }

    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.PushStrike, properties = properties };
    }
    
    public override void ProcessInput()
    {
        if (_mapEntity == null)
        {
            return;
        }
        if (MyInput.GetOnWorldUp(_mapEntity.Settings.Plane()))
        {
            var clickPos = MyInput.GroundPosition(_mapEntity.Settings.Plane());
            TileEntity tile = _mapEntity.Tile(clickPos);
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

                source.LookAt(_mapEntity.WorldPosition(chosenTile));
                Vector3Int newPos = new Vector3Int((chosenTile.occupyingHero.currentTile.TilePos.x - source.currentTile.TilePos.x) + chosenTile.occupyingHero.currentTile.TilePos.x,
                    (chosenTile.occupyingHero.currentTile.TilePos.y - source.currentTile.TilePos.y)+ chosenTile.occupyingHero.currentTile.TilePos.y,
                    (chosenTile.occupyingHero.currentTile.TilePos.z - source.currentTile.TilePos.z)+ chosenTile.occupyingHero.currentTile.TilePos.z);
                var newTile = _mapEntity.Tile(newPos);
                if(newTile != null)
                    chosenTile.occupyingHero.Move(newTile);
                source?.onSpecialAbilityFinished();
            }
            unitAnimation.PlaySpecialAbillity(animationId);
        }
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
