using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HeroSelectToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI label;
    private HeroController hero;
    private Action<HeroController> onHeroSelectedCallback;
    public void OnEnable()
    {
        toggle.onValueChanged.AddListener(OnToggleClicked);
    }

    public void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleClicked);

    }
    public void Init(HeroController referencedHero, Action<HeroController> onHeroSelectedCallback)
    {
        toggle.group = GetComponentInParent<ToggleGroup>();
        hero = referencedHero;
        label.text = hero.name;
        this.onHeroSelectedCallback = onHeroSelectedCallback;
    }

    private void OnToggleClicked(bool value)
    {
        if (!value)
            return;

        onHeroSelectedCallback?.Invoke(hero);
    }
}
