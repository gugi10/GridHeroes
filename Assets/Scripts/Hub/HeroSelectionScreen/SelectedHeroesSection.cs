using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectedHeroesSection : MonoBehaviour
{
    [SerializeField] private int maxHeroes = 10;
    private List<HeroToSelect> selectedHeroes = new List<HeroToSelect>();
    private HeroService heroService;

    private void Awake()
    {
        heroService = GameSession.Instance.GetService<HeroService>();

        foreach (HeroController hero in heroService.GetPlayerLineUp())
        {
            var heroFromLineUp = new HeroToSelect();
            heroFromLineUp.Init(hero);
            AddHero(heroFromLineUp);
        }
    }

    public void AddHero(HeroToSelect heroToSelect)
    {
        if(maxHeroes <= selectedHeroes.Count())
        {
            Debug.Log($"Added maximum amount of heroes");
            return;
        }
        var heroToAdd = Instantiate(heroToSelect, transform);
        heroToAdd.SetCallback(RemoveHero);
        selectedHeroes.Add(heroToAdd);
        heroService.AddHeroToLineUp(heroToSelect.RepresentedHeroController);
    }

    public void RemoveHero(HeroToSelect heroToSelect)
    {
        if (selectedHeroes.Contains(heroToSelect))
        {
            selectedHeroes.Remove(heroToSelect);
            heroService.RemoveHeroFromLineUp(heroToSelect.RepresentedHeroController);
            Destroy(heroToSelect.gameObject);
        }
    }

    public int GetSelectedHeroesCount()
    {
        return selectedHeroes.Count();
    }
}
