using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour, IPlayer
{
    HeroController heroController;
    public int Id { get; set; }
    private MapController map;
    private List<HeroController> heroes = new List<HeroController>();

    public void Init(MapController map, List<HeroController> ownedHeroes, int playerId)
    {
        this.map = map;
        heroes = ownedHeroes;
        Id = playerId;
    }
    public void SetActiveState(bool flag)
    {
        this.enabled = flag;
    }
}
