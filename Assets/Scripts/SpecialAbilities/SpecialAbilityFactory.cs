using RedBjorn.ProtoTiles;
using SpecialAbilities;

public class SpecialAbilityFactory
{
    private HeroController source;
    private MapEntity mapEntity;

    public SpecialAbilityFactory(HeroController source, MapEntity mapEntity)
    {
        this.source = source;
        this.mapEntity = mapEntity;
    }

    public ISpecialAbility[] BuildSpecialAbility(HeroId heroId)
    {
        var fireboltProperties = new BasicProperties { damage = 1, range = 3 };
        var fireboltScore = new FireboltScore(fireboltProperties);
        var fireboltFx = new FireboltFx(source, mapEntity, fireboltProperties, "Attack02");
        var fireboltProcess = new FireboltProcess(mapEntity, source, fireboltProperties, fireboltFx);
        var fireboltHighlight = new SingleTargetTileHighlight(new AffectedTilesHiglight(),  source, fireboltProperties);

        var whirlwindProperties = new BasicProperties { damage = 1, range = 1 };
        var whirlwindScore = new WhirlwindScore(whirlwindProperties, mapEntity, source);
        var whirlwindFx = new WhirlwindAbilityFX(source, "Attack02");
        var whirlwindProcess = new WhirlwindProcess(mapEntity, source, whirlwindProperties, whirlwindFx);
        var whirlwindHighlight = new WhirlwindTileHighlight(new AffectedTilesHiglight(), source, mapEntity, whirlwindProperties);
        
        var pushProperties = new BasicProperties { damage = 1, range = 1 };
        var pushScore = new PushScore(pushProperties);
        var pushFx = new PushAbilityFx(source, "Attack03");
        var pushProcess = new PushProcess(mapEntity, source, pushProperties, pushFx);
        var pushHighlight = new SingleTargetTileHighlight(new AffectedTilesHiglight(), source, pushProperties);
        
        var webPullProperties = new BasicProperties { damage = 0, range = 3 };
        var webPullScore = new FireboltScore(webPullProperties);
        var webPullFx = new WebPullFx(source, mapEntity, webPullProperties, "Attack03");
        var webPullProcess = new PullProcess(mapEntity, source, webPullProperties, webPullFx);
        var webPullHighlight = new SingleTargetTileHighlight(new AffectedTilesHiglight(),  source, webPullProperties);

        if (heroId == HeroId.EvilMage) {
            var abilities = new SpecialAbility[1];
            abilities[0] = new SpecialAbility(fireboltScore, fireboltProcess, fireboltHighlight);
            return abilities;
        }

        if (heroId == HeroId.SpiderRanger)
        {
            var abilities = new SpecialAbility[1];
            abilities[0] = new SpecialAbility(webPullScore, webPullProcess, webPullHighlight);
            return abilities;
        }

        if (heroId == HeroId.Crab)
        {
            var abilities = new SpecialAbility[1];
            abilities[0] = new SpecialAbility(whirlwindScore, whirlwindProcess, whirlwindHighlight);
            return abilities;
        }

        // if (heroId == HeroId.BlackKnight)
        // {
            var blackKnightAbilities = new SpecialAbility[1];
            blackKnightAbilities[0] = new SpecialAbility(pushScore, pushProcess, pushHighlight);
            return blackKnightAbilities;
        // }
    }
}
