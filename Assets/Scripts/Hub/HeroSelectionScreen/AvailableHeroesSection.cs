using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableHeroesSection : MonoBehaviour
{
    [SerializeField] private SelectedHeroesSection selectedHeroesSection;
    [SerializeField] private HeroToSelect representedHeroPrefab;

    private List<HeroToSelect> representedHeroes = new List<HeroToSelect>();
    private HeroService heroService;
    private void Awake()
    {
        heroService = GameSession.Instance.GetService<HeroService>();
        Init(heroService.availableHeroes);
    }

    private void Init(List<HeroController> availableHeroes)
    {
        foreach(HeroController hero in availableHeroes)
        {
            var spawnedHero = Instantiate(representedHeroPrefab, transform);
            spawnedHero.Init(hero);
            spawnedHero.SetCallback(OnHeroSelected);
            representedHeroes.Add(spawnedHero);
        }
    }
    
    private void OnHeroSelected(HeroToSelect selectedHero)
    {
        selectedHeroesSection.AddHero(selectedHero);
    }
}
