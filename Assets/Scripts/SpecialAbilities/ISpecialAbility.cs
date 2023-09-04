using RedBjorn.ProtoTiles;
using UnityEngine;

public struct AbilitySpec
{
    public AbilityKind kind;
    public object properties;
}

public enum AbilityKind
{
    Bolt,
    Whirlwind,
}
public interface ISpecialAbility
{
    public void DoSpecialAbility(HeroController source, MapEntity map);

    public Sprite GetSkillIcon();

    public void ProcessInput();

    public void PerformAbility(TileEntity chosenTile);

    public AbilitySpec GetAbilitySpec();
}
