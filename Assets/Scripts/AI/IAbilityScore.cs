using RedBjorn.ProtoTiles;
using static Unity.VisualScripting.Member;
using System.Collections.Generic;

public interface IAbilityScore
{
    public ScoreModifiers ScoreForTarget(HeroController target);
}



public class FireboltScore : IAbilityScore
{
    private BasicProperties properties;
    public FireboltScore(BasicProperties properties)
    {
        this.properties = properties;
    }
    public ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new ScoreModifiers { };

        if (target.GetHeroStats().current.Health <= properties.damage)
        {
            modifiers.enemiesKilled = 1;
        }
        modifiers.inflictedDamage = target.GetHeroStats().current.Health;

        return modifiers;
    }
}

public class WhirlwindScore : IAbilityScore
{
    private BasicProperties properties;
    private MapEntity mapEntity;
    private HeroController source;
    public WhirlwindScore(BasicProperties properties, MapEntity mapEntity, HeroController source)
    {
        this.properties = properties;
        this.mapEntity = mapEntity;
        this.source = source;
    }
    public ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new() { };

        HashSet<TileEntity> surroundingTiles = mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range);

        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
            {
                if (target.GetHeroStats().current.Health <= this.properties.damage)
                {
                    modifiers.enemiesKilled += 1;
                }
                modifiers.inflictedDamage += target.GetHeroStats().current.Health;
            }
        }

        return modifiers;
    }
}

public class PushScore : IAbilityScore
{
    private BasicProperties properties;
    public PushScore(BasicProperties properties)
    {
        this.properties = properties;
    }
    public ScoreModifiers ScoreForTarget(HeroController target)
    {
        ScoreModifiers modifiers = new ScoreModifiers { };

        if (target.GetHeroStats().current.Health <= this.properties.damage)
        {
            modifiers.enemiesKilled = 1;
        }
        modifiers.inflictedDamage = target.GetHeroStats().current.Health;
        // TODO: CONSIDER PUSH FOR MODIFIERS

        return modifiers;
    }
}