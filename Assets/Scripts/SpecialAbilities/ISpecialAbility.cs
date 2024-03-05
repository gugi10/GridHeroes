using System;
using RedBjorn.ProtoTiles;
using SpecialAbilities;
using Unity.VisualScripting;
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

public class SpecialAbility : ISpecialAbility
{
    private IAbilityScore abilityScore;
    private ISpecialAbilityProcess abilityProcess;
    private ISpecialAbilityHighlighter abilityHighlight;
    
    public SpecialAbility(IAbilityScore abilityScore, ISpecialAbilityProcess abilityProcess, ISpecialAbilityHighlighter abilityHighlight)
    {
        this.abilityScore = abilityScore;
        this.abilityProcess = abilityProcess;
        this.abilityHighlight = abilityHighlight;
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
        //return abilityFx.GetSkillIcon();
        throw new NotImplementedException();
        return null;
    }

    public AffectedTilesHiglight GetAffectedTiles()
    {
        return abilityHighlight.GetAffectedTiles();
    }

    public void HighlightTargetedTile(TileEntity tile, MapController map)
    {
        abilityHighlight.HighlightTargetedTile(tile, map);
    }

    public void DisableHighlight(MapController map)
    {
        abilityHighlight.DisableHighlight(map);
    }
}