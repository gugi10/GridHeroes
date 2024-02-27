using RedBjorn.ProtoTiles;

public class SpecialAbilityFactory
{
    private HeroController source;
    private MapEntity mapEntity;

    public SpecialAbilityFactory(HeroController source, MapEntity mapEntity)
    {
        this.source = source;
        this.mapEntity = mapEntity;
    }

    public SpecialAbility[] BuildSpecialAbility(HeroId heroId)
    {
        var fireboltProperties = new BasicProperties { damage = 1, range = 3 };
        var fireboltScore = new FireboltScore(fireboltProperties);
        var fireboltProcess = new FireboltProcess(mapEntity, source, fireboltProperties);
        var fireboltFx = new SingleTargetTileHighlight(new AffectedTilesHiglight(),  source, fireboltProperties);

        var whirlwindProperties = new BasicProperties { damage = 1, range = 1 };
        var whirlwindScore = new WhirlwindScore(whirlwindProperties, mapEntity, source);
        var whirlwindProcess = new WhirlwindProcess(mapEntity, source, fireboltProperties);
        var whirlwindFx = new WhirlwindTileHighlight(new AffectedTilesHiglight(), source, mapEntity, whirlwindProperties);

        var pushProperties = new BasicProperties { damage = 1, range = 1 };
        var pushScore = new PushScore(pushProperties);
        var pushProcess = new PushProcess(mapEntity, source, fireboltProperties);
        var pushFx = new SingleTargetTileHighlight(new AffectedTilesHiglight(), source, pushProperties);

        if (heroId == HeroId.EvilMage || heroId == HeroId.SpiderRanger) {
            var abilities = new SpecialAbility[1];
            abilities[0] = new SpecialAbility(fireboltScore, fireboltProcess, fireboltFx);
            return abilities;
        }

        if (heroId == HeroId.Crab)
        {
            var abilities = new SpecialAbility[1];
            abilities[0] = new SpecialAbility(whirlwindScore, whirlwindProcess, whirlwindFx);
            return abilities;
        }

        // if (heroId == HeroId.BlackKnight)
        // {
            var blackKnightAbilities = new SpecialAbility[1];
            blackKnightAbilities[0] = new SpecialAbility(pushScore, pushProcess, pushFx);
            return blackKnightAbilities;
        // }
    }
}
