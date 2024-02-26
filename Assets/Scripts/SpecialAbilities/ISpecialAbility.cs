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


public interface ISpecialAbility : IAbilityScore, ISpecialAbilityProcess, ISpecialAbilityHighlighter
{
}

public class SpecialAbility : MonoBehaviour, ISpecialAbility
{
    private IAbilityScore abilityScore;
    private ISpecialAbilityProcess abilityProcess;
    private ISpecialAbilityHighlighter abilityFx;

    public SpecialAbility(IAbilityScore abilityScore, ISpecialAbilityProcess abilityProcess, ISpecialAbilityHighlighter abilityFx)
    {
        this.abilityScore = abilityScore;
        this.abilityProcess = abilityProcess;
        this.abilityFx = abilityFx;
    }
    public void ProcessInput()
    {
        abilityProcess.ProcessInput();
    }

    public void PerformAbility(TileEntity chosenTile)
    {
        abilityProcess.PerformAbility(chosenTile);
    }

    public AbilitySpec GetAbilitySpec()
    {
        return abilityProcess.GetAbilitySpec();
    }

    public bool CanBeUsedOnTarget(TileEntity chosenTile)
    {
        return abilityProcess.CanBeUsedOnTarget(chosenTile);
    }

    public ScoreModifiers ScoreForTarget(HeroController target)
    {
        return abilityScore.ScoreForTarget(target);
    }

    public Sprite GetSkillIcon()
    {
        return abilityFx.GetSkillIcon();
    }

    public AffectedTilesHiglight GetAffectedTiles()
    {
        return abilityFx.GetAffectedTiles();
    }

    public void HighlightTargetedTile(TileEntity tile, MapController map)
    {
        abilityFx.HighlightTargetedTile(tile, map);
    }

    public void DisableHighlight(MapController map)
    {
        abilityFx.DisableHighlight(map);
    }
}