using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HeroToSelect : MonoBehaviour
{
    public HeroId RepresentedHeroController { get; private set; }
    [SerializeField] private Button selectHero;
    [SerializeField] private TextMeshProUGUI heroName;
    private HeroService _heroService;
    
    private void Awake()
    {
        _heroService = GameSession.Instance.GetService<HeroService>();
    }

    public void Init(HeroId representedHero)
    {
        heroName.text = _heroService.GetHeroPrefab(representedHero).name;
        RepresentedHeroController = representedHero;
    }

    public void SetCallback(Action<HeroToSelect> buttonCallback)
    {
        selectHero.onClick.RemoveAllListeners();
        selectHero.onClick.AddListener(() => buttonCallback(this));
    }

}
