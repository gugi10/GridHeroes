using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroService : IService
{
    public List<HeroController> availableHeroes { get; private set; }
    private List<HeroController> playerLineUp = new List<HeroController>();

    public HeroService(List<HeroController> availableHeroes)
    {
        this.availableHeroes = availableHeroes;
    }

    public void AddHeroToLineUp(HeroController hero)
    {
        playerLineUp.Add(hero);
    }

    public void RemoveHeroFromLineUp(HeroController hero)
    {
        playerLineUp.Remove(hero);
    }

    public List<HeroController> GetPlayerLineUp()
    {
        return playerLineUp;
    }
}
