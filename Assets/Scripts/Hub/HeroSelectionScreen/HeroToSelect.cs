using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HeroToSelect : MonoBehaviour
{
    public HeroController RepresentedHeroController { get; private set; }
    [SerializeField] private Button selectHero;
    [SerializeField] private TextMeshProUGUI heroName;
    public void Init(HeroController representedHero)
    {
        heroName.text = representedHero.name;
        RepresentedHeroController = representedHero;
    }

    public void SetCallback(Action<HeroToSelect> buttonCallback)
    {
        selectHero.onClick.RemoveAllListeners();
        selectHero.onClick.AddListener(() => buttonCallback(this));
    }

}
