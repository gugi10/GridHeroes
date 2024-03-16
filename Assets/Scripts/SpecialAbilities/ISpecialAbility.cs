using System;
using RedBjorn.ProtoTiles;
using SpecialAbilities;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

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
    public Sprite GetSkillIcon();
}

public class SpecialAbility : ISpecialAbility
{
    private IAbilityScore abilityScore;
    private ISpecialAbilityProcess abilityProcess;
    private ISpecialAbilityHighlighter abilityHighlight;
    private SkillId skillId;
    private SkillIconsConfig _skillIconsConfig;
    private Sprite skillSprite;
    
    public SpecialAbility(IAbilityScore abilityScore, ISpecialAbilityProcess abilityProcess, ISpecialAbilityHighlighter abilityHighlight, SkillId skillId)
    {
        this.abilityScore = abilityScore;
        this.abilityProcess = abilityProcess;
        this.abilityHighlight = abilityHighlight;
        this.skillId = skillId;
        _skillIconsConfig = GameSession.Instance.GetConfig<SkillIconsConfig>();
        skillSprite = _skillIconsConfig.SkillIcons.FirstOrDefault(val => val.skillId == skillId).SkillSprite;

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
        return skillSprite;
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