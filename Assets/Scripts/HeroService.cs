using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroService : IService
{
    private List<HeroController> playerLineUp = new List<HeroController>();

    public void AddHeroToLineUp(HeroController hero)
    {
        playerLineUp.Add(hero);
    }

    public void RemoveHeroFromLineUp(int id)
    {
        playerLineUp.RemoveAt(id);
    }

    public List<HeroController> GetPlayerLineUp()
    {
        return playerLineUp;
    }
}
