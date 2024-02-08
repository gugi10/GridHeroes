using System.Collections.Generic;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using SpecialAbilities;
using UnityEngine;


[RequireComponent(typeof(UnitAnimations))]
public class FireboltAbility : TargetableAbility
{
    [SerializeField] ProjectileAnimation projectileAnimation;
    private new readonly BasicProperties properties = new() { damage = 2, range = 3 };

    private UnitAnimations unitAnimations;

    protected override void Awake()
    {
        base.Awake();
        unitAnimations = GetComponent<UnitAnimations>();
    }

    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.Bolt, properties = properties };
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

        if (CanBeUsedOnTarget(chosenTile))
        {
            source.LookAt(_mapEntity.WorldPosition(chosenTile));
            unitAnimations.PlaySpecialAbillity(animationId);
            projectileAnimation.PlayProjectile(_mapEntity.WorldPosition(chosenTile.Data.TilePos), 0.8f, CreateOnHit(chosenTile));
        }
    }

    public override ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new ScoreModifiers {};

        if(target.GetHeroStats().current.Health <= this.properties.damage)
        {
            modifiers.enemiesKilled = 1;
        }
        modifiers.inflictedDamage = target.GetHeroStats().current.Health;

        return modifiers;
    }

    //Projectile stuff
    private void OnHit(TileEntity chosenTile)
    {
        chosenTile.occupyingHero.DealDamage(properties.damage);
        source.onSpecialAbilityFinished();
    }

    private System.Action CreateOnHit(TileEntity chosenTile)
    {
        return () => { OnHit(chosenTile); };
    }
}
