using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroConfigs", menuName = "ScriptableObjects/HeroConfigs", order = 1)]

public class HeroesConfig : BaseConfig
{
    public List<HeroController> heroPrefabs;
    public List<HeroId> startingHeroes;
}
