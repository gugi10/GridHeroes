using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class HeroToSelect : MonoBehaviour
{
    public HeroId RepresentedHeroController { get; private set; }
    [SerializeField] private Button selectHero;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private Image heroIcon;
    private HeroService _heroService;
    private HeroesConfig heroesConfig;
    
    private void Awake()
    {
        _heroService = GameSession.Instance.GetService<HeroService>();
        heroesConfig = GameSession.Instance.GetConfig<HeroesConfig>();
    }

    public void Init(HeroId representedHero)
    {
        heroIcon.sprite = heroesConfig.heroConfigData.FirstOrDefault(hero => hero.heroId == representedHero).heroIconSprite;
        RepresentedHeroController = representedHero;
    }

    public void SetCallback(Action<HeroToSelect> buttonCallback)
    {
        selectHero.onClick.RemoveAllListeners();
        selectHero.onClick.AddListener(() => buttonCallback(this));
    }

}
