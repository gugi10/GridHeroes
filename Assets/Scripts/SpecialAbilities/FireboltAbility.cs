using System.Collections.Generic;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using SpecialAbilities;
using UnityEngine;


[RequireComponent(typeof(UnitAnimations))]
public class FireboltAbility : AbilityBase, ITargetable
{
    [SerializeField] ProjectileAnimation projectileAnimation;
    public AffectedTilesHiglight AffectedTile { get; set; }
    private UnitAnimations unitAnimations;
    private BasicProperties properties = new() { damage = 1, range = 3 };
    private HeroController source;
    private MapEntity map;

    private void Awake()
    {
        unitAnimations = GetComponent<UnitAnimations>();
        AffectedTile = new AffectedTilesHiglight();
    }

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
        return new AbilitySpec { kind = AbilityKind.Bolt, properties = properties };
    }

    public override void PerformAbility(TileEntity chosenTile)
    {

        if (chosenTile == null)
            return;

        if (CanBeUsedOnTarget(chosenTile))
        {
            source.LookAt(map.WorldPosition(chosenTile));
            unitAnimations.PlaySpecialAbillity(animationId);
            projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 0.8f, CreateOnHit(chosenTile));
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
        ScoreModifiers modifiers = new ScoreModifiers {};

        if(target.GetHeroStats().current.Health <= this.properties.damage)
        {
            modifiers.enemiesKilled = 1;
        }
        modifiers.inflictedDamage = target.GetHeroStats().current.Health;

        return modifiers;
    }

    //TODO: To consider some common code for all targatbles
    public void HighlightTargetedTile(TileEntity tile, MapController map)
    {
        if(TileUtilities.AreTilesInRange(source.currentTile.TilePos, tile.Position, properties.range))
            AffectedTile.HighlightTile(tile, map);   
    }

    public void DisableHighlight(MapController map)
    {
        AffectedTile.DisableHiglight(map);
    }

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
