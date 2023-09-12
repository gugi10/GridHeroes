using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnitAnimations))]
public class FireboltAbility : AbilityBase
{
    [SerializeField] ProjectileAnimation projectileAnimation;
    UnitAnimations unitAnimations;
    private Properties properties = new() { damage = 1, range = 3 };
    private HeroController source;
    private MapEntity map;

    private void Awake()
    {
        unitAnimations = GetComponent<UnitAnimations>();
    }

    public struct Properties
    {
        public int range;
        public int damage;
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

        if (chosenTile.IsOccupied)
        {
            if (TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, properties.range) &&
                chosenTile.occupyingHero != source && chosenTile.occupyingHero.ControllingPlayerId != source.ControllingPlayerId)
            {
                
                unitAnimations.PlaySpecialAbillity(animationId);
                projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 1.1f, CreateOnHit(chosenTile));
                 
            }
            //TODO:Play particle effect

        }
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
