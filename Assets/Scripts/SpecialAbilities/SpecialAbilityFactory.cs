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

    public ISpecialAbility2 BuildSpecialAbility(HeroId heroId)
    {
        return heroId switch
        {
            HeroId.EvilMage => new SpecialAbility2(new FireboltScore(new BasicProperties { damage = 1, range = 3})),
            HeroId.Crab => new SpecialAbility2(new WhirlwindScore(new BasicProperties { damage = 1, range = 1 }, mapEntity, source)),
            HeroId.BlackKnight => new SpecialAbility2(new PushScore(new BasicProperties { damage = 1, range = 1 })),
            HeroId.SpiderRanger => new SpecialAbility2(new FireboltScore(new BasicProperties { damage = 1, range = 3 })),
        };
    }
}
