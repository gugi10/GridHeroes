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


public interface ISpecialAbility2 : IAbilityScore, ISpecialAbilityProcess
{

}
public class SpecialAbility2 : MonoBehaviour, ISpecialAbility2
{
    private IAbilityScore abilityScore;
    private ISpecialAbilityProcess abilityProcess;

    public SpecialAbility2(IAbilityScore abilityScore, ISpecialAbilityProcess abilityProcess)
    {
        this.abilityScore = abilityScore;
        this.abilityProcess = abilityProcess;
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
}