using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectedHeroesSection : MonoBehaviour
{
    [SerializeField] private int maxHeroes = 10;
    [SerializeField] private HeroToSelect heroToSelectPrefab;
    private List<HeroToSelect> selectedHeroes = new List<HeroToSelect>();
    private HeroService heroService;

    private void Awake()
    {
        heroService = GameSession.Instance.GetService<HeroService>();

        foreach (HeroController hero in heroService.GetPlayerLineUp())
        {
            SpawnHero(hero);
        }
    }

    public void AddNewHero(HeroController heroToSelect)
    {
        if(maxHeroes <= selectedHeroes.Count())
        {
            Debug.Log($"Added maximum amount of heroes");
            return;
        }
        var heroToAdd = SpawnHero(heroToSelect);
        heroService.AddHeroToLineUp(heroToAdd.RepresentedHeroController);
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

    private HeroToSelect SpawnHero(HeroController heroToSelect)
    {
        var heroToAdd = Instantiate(heroToSelectPrefab, transform);
        heroToAdd.Init(heroToSelect);
        heroToAdd.SetCallback(RemoveHero);
        selectedHeroes.Add(heroToAdd);
        return heroToAdd;
    }
}
