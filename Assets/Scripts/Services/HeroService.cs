using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroService : IService
{
    public List<HeroId> availableHeroes { get; private set; }
    private List<HeroId> playerLineUp = new List<HeroId>();
    private HeroesConfig heroesConfig;

    public HeroService(HeroesConfig heroesConfig)
    {
        this.heroesConfig = heroesConfig;
        this.availableHeroes = heroesConfig.startingHeroes;
    }

    public void AddHeroToLineUp(HeroId hero)
    {
        playerLineUp.Add(hero);
    }

    public void RemoveHeroFromLineUp(HeroId hero)
    {
        playerLineUp.Remove(hero);
    }

    public HeroController GetHeroPrefab(HeroId id)
    {
        return heroesConfig.heroPrefabs.FirstOrDefault(val => val.HeroId == id);
    }

    public bool UnlockHero(HeroId heroToUnlock)
    {
        if (availableHeroes.Contains(heroToUnlock))
            return false;
        availableHeroes.Add(heroToUnlock);
        return true;
    }

    public List<HeroId> GetPlayerLineUp()
    {
        return playerLineUp;
    }
}
