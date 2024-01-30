using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroService : IService
{
    public List<HeroId> availableHeroes { get; private set; }
    private List<HeroId> playerLineUp = new List<HeroId>();
    private HeroesConfig heroesConfig;
    private List<HeroId> justUnlockedHeroes = new List<HeroId>();

    public HeroService(HeroesConfig heroesConfig)
    {
        this.heroesConfig = heroesConfig;
        this.availableHeroes = new List<HeroId>(heroesConfig.startingHeroes);
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
        justUnlockedHeroes.Add(heroToUnlock);
        return true;
    }

    public List<HeroId> GetHeroesToClaim()
    {
        var toReturn = new List<HeroId>(justUnlockedHeroes);
        justUnlockedHeroes.Clear();
        justUnlockedHeroes = new List<HeroId>();
        return toReturn;
    }

    public List<HeroId> GetPlayerLineUp()
    {
        return playerLineUp;
    }
}
