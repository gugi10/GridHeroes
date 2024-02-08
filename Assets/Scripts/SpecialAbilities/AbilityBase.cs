using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbilityBase : MonoBehaviour, ISpecialAbility
{
    [SerializeField] protected string animationId = "Attack02";
    [SerializeField] protected Sprite icon;
    protected HeroController source;
    protected MapEntity _mapEntity;

    public virtual void InitSpecialAbility(HeroController source, MapEntity map)
    {
        this.source = source;
        _mapEntity = map;
    }
    
    public virtual bool CanBeUsedOnTarget(TileEntity chosenTile)
    {
        if (!chosenTile.IsOccupied)
        {
            return false;
        }

        if (!TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, GetAbilitySpec().properties.range))
        {
            return false;
        }

        if (chosenTile.occupyingHero == source || chosenTile.occupyingHero.ControllingPlayerId == source.ControllingPlayerId)
        {
            return false;
        }

        return true;
    }

    public virtual Sprite GetSkillIcon()
    {
        return icon;
    }

    //Below methods should be implemented by each specific ability
    public virtual void ProcessInput()
    {
        throw new System.NotImplementedException();
    }

    public virtual void PerformAbility(TileEntity chosenTile)
    {
        throw new System.NotImplementedException();
    }

    public virtual AbilitySpec GetAbilitySpec()
    {
        throw new System.NotImplementedException();
    }
    
    public virtual void HighlightAffectedTiles(TileEntity tile, MapController map)
    {
        throw new System.NotImplementedException();
    }
    public virtual ScoreModifiers ScoreForTarget(HeroController target)
    {
        throw new System.NotImplementedException();
    }

    public virtual void DisableHighlight(MapController map)
    {
        throw new System.NotImplementedException();
    }

    

    
}
