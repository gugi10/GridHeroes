using RedBjorn.ProtoTiles;
using UnityEngine;

public interface ISpecialAbility 
{
    public bool DoSpecialAbility(HeroController source, MapEntity map);

    public Sprite GetSkillIcon();
    public string GetSkillAnimation();
}
