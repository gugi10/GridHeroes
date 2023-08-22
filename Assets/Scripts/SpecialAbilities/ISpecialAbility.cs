using RedBjorn.ProtoTiles;
using UnityEngine;
using System;

public interface ISpecialAbility 
{
    public void DoSpecialAbility(HeroController source, MapEntity map);

    public Sprite GetSkillIcon();
}
