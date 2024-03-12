using RedBjorn.ProtoTiles.Example;
using RedBjorn.ProtoTiles;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private ISpecialAbilityFX specialAbilityFX;
    private TileEntity chosenTile;
    public FireboltProcess(MapEntity map, HeroController source, BasicProperties properties, ISpecialAbilityFX specialAbilityFX)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
        this.specialAbilityFX = specialAbilityFX;
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
        this.chosenTile = chosenTile;
        if (chosenTile == null)
            return;

        if (CanBeUsedOnTarget(chosenTile))
        {
            source.LookAt(map.WorldPosition(chosenTile));
            specialAbilityFX.StartAnimation(chosenTile, OnHit);
            // unitAnimations.PlaySpecialAbillity(animationId);
            // projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 0.8f, CreateOnHit(chosenTile));
        }
    }

    private void OnHit()
    {
        chosenTile.occupyingHero.DealDamage(properties.damage);
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

    private ISpecialAbilityFX abilityFx;
    //To cosinder some other way of handling animation instead of passing it as parameter
    public WhirlwindProcess(MapEntity map, HeroController source, BasicProperties properties
        , ISpecialAbilityFX abilityFx)
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
        abilityFx.StartAnimation(null, null);
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
    private ISpecialAbilityFX specialAbilityFX;

    public PushProcess(MapEntity map, HeroController source, BasicProperties properties, ISpecialAbilityFX specialAbilityFX)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
        this.specialAbilityFX = specialAbilityFX;
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
                chosenTile.occupyingHero.DealDamage(properties.damage);
                
                if (chosenTile.occupyingHero != null && chosenTile.occupyingHero.GetHeroStats().current.Health > 0)
                {
                    if (newTile != null)
                        chosenTile.occupyingHero.Move(newTile, true);
                }
                                    
                source?.onSpecialAbilityFinished();
            }
            specialAbilityFX.StartAnimation(chosenTile, null);
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

public class PullProcess : ISpecialAbilityProcess
{

    private MapEntity map;
    private HeroController source;
    private BasicProperties properties;
    private ISpecialAbilityFX specialAbilityFX;
    private TileEntity chosenTile;
    private TileEntity enemyTile;
    private ISpecialAbilityHighlighter highlighter;
    private MapController mapController;
    private bool isSelectingTileToPull;

    public PullProcess(MapEntity map, HeroController source, BasicProperties properties, ISpecialAbilityFX specialAbilityFX, ISpecialAbilityHighlighter highlighter)
    {
        this.map = map;
        this.source = source;
        this.properties = properties;
        this.specialAbilityFX = specialAbilityFX;
        this.highlighter = highlighter;
        this.mapController = mapController;
    }

    public void ProcessInput()
    {
        if (map == null)
        {
            return;
        }

        if (isSelectingTileToPull)
        {
            //From caluclated possible tiles we need to select one
            //Right now we do not have any indication to which tile we can pull
            if (MyInput.GetOnWorldUp(map.Settings.Plane()))
            {
                var clickPos = MyInput.GroundPosition(map.Settings.Plane());
                TileEntity tile = map.Tile(clickPos);
                if(tile.Position == newTile?.Position || tile.Position == newTile2?.Position || tile.Position == newTile3?.Position)
                {
                    PerformAbility(tile);
                    isSelectingTileToPull = false;
                }
                else
                {
                    isSelectingTileToPull = false;
                    Debug.LogError($"This tile is not possible to pull to {tile.Position}");
                }
            }
            return;
        }
        //First we need to detect input on enemy we want  to pull, we need to check all possible conditions here
        if (MyInput.GetOnWorldUp(map.Settings.Plane()))
        {
            var clickPos = MyInput.GroundPosition(map.Settings.Plane());
            TileEntity tile = map.Tile(clickPos);
            
            if (tile == null)
                return;
            //We need to calculate possible tiles to pull
            CalculatePossibleTiles(tile);
            isSelectingTileToPull = true;
        }
    }

    private TileEntity newTile;
    private TileEntity newTile2;
    private TileEntity newTile3;
    
    public void CalculatePossibleTiles(TileEntity targetTile)
    {
        if (!targetTile.IsOccupied || targetTile.occupyingHero.ControllingPlayerId != PlayerId.AI)
        {
            return;
        }
        //we need to set enemy tile in order to perform animation towards it
        enemyTile = targetTile;
        
        //Calculate possible tiles to which enemy can be pulled.
        //There are 3 possible combinations of two differences in vector3. Adjacent tile cannot differ by 3 values only 2 or 1. map.Tile() will return null if such tile does not exist
        var xDifference = Mathf.Clamp(source.currentTile.TilePos.x - targetTile.occupyingHero.currentTile.TilePos.x, -1, 1);
        var yDifference = Mathf.Clamp(source.currentTile.TilePos.y - targetTile.occupyingHero.currentTile.TilePos.y, -1, 1);
        var zDifference = Mathf.Clamp(source.currentTile.TilePos.z - targetTile.occupyingHero.currentTile.TilePos.z, -1, 1);
        newTile = map.Tile(new Vector3Int(targetTile.occupyingHero.currentTile.TilePos.x + xDifference,
           targetTile.occupyingHero.currentTile.TilePos.y + yDifference,
           targetTile.occupyingHero.currentTile.TilePos.z ));
        newTile2 = map.Tile(new Vector3Int(targetTile.occupyingHero.currentTile.TilePos.x ,
           targetTile.occupyingHero.currentTile.TilePos.y + yDifference,
           targetTile.occupyingHero.currentTile.TilePos.z + zDifference));
        newTile3 = map.Tile(new Vector3Int(targetTile.occupyingHero.currentTile.TilePos.x + xDifference,
            targetTile.occupyingHero.currentTile.TilePos.y ,
            targetTile.occupyingHero.currentTile.TilePos.z + zDifference));
    }
    public void PerformAbility(TileEntity chosenTile)
    {
        this.chosenTile = chosenTile;
        if (chosenTile == null)
            return;

        if (!chosenTile.IsOccupied)
        {
            specialAbilityFX.StartAnimation(chosenTile, DoPull);
        }
    }

    private async void DoPull()
    {
        //We are looking at enemy but moving it to selected tile
        source.LookAt(map.WorldPosition(enemyTile));
        var enemyHero = enemyTile.occupyingHero;
        enemyHero.Move(chosenTile, true);
        enemyHero.DealDamage(properties.damage);
        await Task.Delay(3000);
        source.onSpecialAbilityFinished();
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
