using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroConfigs", menuName = "ScriptableObjects/HeroConfigs", order = 1)]

public class HeroesConfig : ScriptableObject
{
    public List<HeroController> heroPrefabs;
}
