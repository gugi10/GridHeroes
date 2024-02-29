using RedBjorn.ProtoTiles.Example;
using RedBjorn.ProtoTiles;
using UnityEngine;
using System.Collections.Generic;
using SpecialAbilities;

public interface ISpecialAbilityProcess
{
    public AbilitySpec GetAbilitySpec();
    public void ProcessInput();
    public void PerformAbility(TileEntity chosenTile);
    public bool CanBeUsedOnTarget(TileEntity chosenTile);
}

public class FireboltProcess : ISpecialAbilityProcess
{
    private MapEntity map;
    private HeroController source;
    private BasicProperties properties;

    public FireboltProcess(MapEntity map, HeroController source, BasicProperties properties)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
    }

    public void ProcessInput()
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
    public void PerformAbility(TileEntity chosenTile)
    {

        if (chosenTile == null)
            return;

        if (CanBeUsedOnTarget(chosenTile))
        {
            source.LookAt(map.WorldPosition(chosenTile));
            // unitAnimations.PlaySpecialAbillity(animationId);
            // projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 0.8f, CreateOnHit(chosenTile));
        }
    }

    public bool CanBeUsedOnTarget(TileEntity chosenTile)
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

    public AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.Bolt, properties = properties };
    }
}

public class WhirlwindProcess : ISpecialAbilityProcess
{

    private MapEntity map;
    private HeroController source;
    private BasicProperties properties;

    private WhirlwindAbilityFX abilityFx;
    //To cosinder some other way of handling animation instead of passing it as parameter
    public WhirlwindProcess(MapEntity map, HeroController source, BasicProperties properties
        , WhirlwindAbilityFX abilityFx)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
        this.abilityFx = abilityFx;
    }

    public void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0))
            PerformAbility(null);
    }
    public void PerformAbility(TileEntity chosenTile)
    {
        HashSet<TileEntity> surroundingTiles = map.WalkableTiles(source.currentTile.TilePos, properties.range);

        //unitAnimations.PlaySpecialAbillity(animationId);
        abilityFx.StartAnimation();
        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
            {
                tile.occupyingHero.DealDamage(properties.damage);
            }
        }
        source?.onSpecialAbilityFinished();
    }

    public bool CanBeUsedOnTarget(TileEntity chosenTile)
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

    public AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.Whirlwind, properties = properties };
    }
}
public class PushProcess : ISpecialAbilityProcess
{

    private MapEntity map;
    private HeroController source;
    private BasicProperties properties;

    public PushProcess(MapEntity map, HeroController source, BasicProperties properties)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
    }

    public void ProcessInput()
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

    public void PerformAbility(TileEntity chosenTile)
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
                    (chosenTile.occupyingHero.currentTile.TilePos.y - source.currentTile.TilePos.y) + chosenTile.occupyingHero.currentTile.TilePos.y,
                    (chosenTile.occupyingHero.currentTile.TilePos.z - source.currentTile.TilePos.z) + chosenTile.occupyingHero.currentTile.TilePos.z);
                var newTile = map.Tile(newPos);
                if (newTile != null)
                    chosenTile.occupyingHero.Move(newTile);
                source?.onSpecialAbilityFinished();
            }
            // unitAnimations.PlaySpecialAbillity(animationId);
        }
    }


    public bool CanBeUsedOnTarget(TileEntity chosenTile)
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

    public AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.PushStrike, properties = properties };
    }
}
