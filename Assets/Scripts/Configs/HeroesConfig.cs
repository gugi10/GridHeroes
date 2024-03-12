using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HeroConfigs", menuName = "ScriptableObjects/HeroConfigs", order = 1)]

public class HeroesConfig : BaseConfig
{
    public List<HeroId> startingHeroes;
    public List<HeroConfigData> heroConfigData;
}

[System.Serializable]
public class HeroConfigData
{
    public HeroId heroId;
    public Sprite heroIconSprite;
    public HeroController heroPrefab;
}
