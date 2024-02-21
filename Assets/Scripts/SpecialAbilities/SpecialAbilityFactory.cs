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

    public ISpecialAbility2[] BuildSpecialAbility(HeroId heroId)
    {
        var fireboltProperties = new BasicProperties { damage = 1, range = 3 };
        var fireboltScore = new FireboltScore(fireboltProperties);
        var fireboltProcess = new FireboltProcess(mapEntity, source, fireboltProperties);

        var whirlwindProperties = new BasicProperties { damage = 1, range = 1 };
        var whirlwindScore = new WhirlwindScore(whirlwindProperties, mapEntity, source);
        var whirlwindProcess = new WhirlwindProcess(mapEntity, source, fireboltProperties);

        var pushProperties = new BasicProperties { damage = 1, range = 1 };
        var pushScore = new PushScore(pushProperties);
        var pushProcess = new PushProcess(mapEntity, source, fireboltProperties);

        if (heroId == HeroId.EvilMage || heroId == HeroId.SpiderRanger) {
            var abilities = new SpecialAbility2[1];
            abilities[0] = new SpecialAbility2(fireboltScore, fireboltProcess);
            return abilities;
        }

        if (heroId == HeroId.Crab)
        {
            var abilities = new SpecialAbility2[1];
            abilities[0] = new SpecialAbility2(whirlwindScore, whirlwindProcess);
            return abilities;
        }

        // if (heroId == HeroId.BlackKnight)
        // {
            var blackKnightAbilities = new SpecialAbility2[1];
            blackKnightAbilities[0] = new SpecialAbility2(pushScore, pushProcess);
            return blackKnightAbilities;
        // }
    }
}
