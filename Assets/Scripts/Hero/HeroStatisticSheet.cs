using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "ScriptableObjects/HeroStatistics", order = 1)]
public class HeroStatisticSheet : ScriptableObject
{
    public int Move;
    public int WeaponRange;
    public int WeaponDamage;
    public int Health;
    public int ActionLimit;

    public HeroStatisticSheet(HeroStatisticSheet heroStats)
    {
        Move = heroStats.Move;
        WeaponRange = heroStats.WeaponRange;
        WeaponDamage = heroStats.WeaponDamage;
        Health = heroStats.Health;
        ActionLimit = heroStats.ActionLimit;
    }
}
