using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbilityBase : MonoBehaviour, ISpecialAbility
{
    [SerializeField] protected string animationId = "Attack02";
    [SerializeField] protected Sprite icon;

    public virtual void DoSpecialAbility(HeroController source, MapEntity map)
    {
        throw new System.NotImplementedException();
    }

    public virtual Sprite GetSkillIcon()
    {
        return icon;
    }

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

    public virtual bool CanBeUsedOnTarget(TileEntity chosenTile)
    {
        throw new System.NotImplementedException();
    }
}
