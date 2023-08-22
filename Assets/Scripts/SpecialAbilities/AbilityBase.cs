using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityBase : MonoBehaviour, ISpecialAbility
{
    [SerializeField] protected string animationId = "Attack02";
    [SerializeField] protected Sprite icon;
    protected bool readInput;

    public virtual void DoSpecialAbility(HeroController source, MapEntity map)
    {
        readInput = true;
    }

    public virtual Sprite GetSkillIcon()
    {
        return icon;
    }

}
