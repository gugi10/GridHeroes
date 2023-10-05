using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ToggleGroup))]
public class HeroSelection : MonoBehaviour
{
    [SerializeField] HeroSelectToggle heroToggle;
    Action<HeroController> onHeroSelected;
    public void Init(List<HeroController> heroesToSpawn, Action<HeroController> onHeroSelected)
    {
        UpdateToggles();
        this.onHeroSelected = onHeroSelected;
        foreach (var hero in heroesToSpawn)
        {
            var heroToggleInstance = Instantiate(heroToggle, transform);
            heroToggleInstance.Init(hero, OnHeroSelectedCallback);
        }
    }

    private void UpdateToggles()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnHeroSelectedCallback(HeroController hero)
    {
        onHeroSelected?.Invoke(hero);
    }
}
