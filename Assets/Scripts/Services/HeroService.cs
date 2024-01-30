using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Services;
using UnityEngine.Serialization;

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
        GameSession.Instance.GetService<SaveService>().SaveAllData();
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

    public HeroSaveModel GetHeroToSaveModel()
    {
        var heroIdStrings = new List<string>();
        availableHeroes.ForEach(val => heroIdStrings.Add(val.ToString()));
        return new HeroSaveModel(heroIdStrings);
    }

    public void LoadHeroSaveData(HeroSaveModel heroSaveModel)
    {
        foreach (var heroIdToParse in heroSaveModel.heroIds)
        {
            if (Enum.TryParse(heroIdToParse, out HeroId parsedId))
            {
                if(!availableHeroes.Contains(parsedId))
                    availableHeroes.Add(parsedId);
            }
            else
            {
                Debug.LogError($"Couldn't parse {heroIdToParse}");
            }
        }
    }
}

[Serializable]
public class HeroSaveModel
{
    public HeroSaveModel(List<string> heroIds)
    {
        this.heroIds = heroIds;
    }

    public List<string> heroIds;
}

