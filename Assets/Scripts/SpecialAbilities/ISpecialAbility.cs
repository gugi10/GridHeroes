using RedBjorn.ProtoTiles;
using UnityEngine;

public struct AbilitySpec
{
    public AbilityKind kind;
    public BasicProperties properties;
}

public enum AbilityKind
{
    Bolt,
    Whirlwind,
    PushStrike,
}

public struct BasicProperties
{
    public int range;
    public int damage;
}

public interface ISpecialAbility
{
    public void DoSpecialAbility(HeroController source, MapEntity map);

    public Sprite GetSkillIcon();

    public void ProcessInput();

    public void PerformAbility(TileEntity chosenTile);

    public bool CanBeUsedOnTarget(TileEntity chosenTile);

    public ScoreModifiers ScoreForTarget(HeroController target);

    public AbilitySpec GetAbilitySpec();

    public void HighlightAffectedTiles(MapController map);

    public void DisableHiglight(MapController map);
}
