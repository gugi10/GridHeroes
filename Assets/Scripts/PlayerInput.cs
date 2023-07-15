using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    public int Id { get; private set; }
    private MapController map;
    private List<HeroController> heroes = new List<HeroController>();
    private HeroController selectedHero;

    public void Init(MapController map, List<HeroController> ownedHeroes, int playerId)
    {
        this.map = map;
        this.heroes = ownedHeroes;
        Id = playerId;
    }
    void Update()
    {
        if (map == null)
        {
            return;
        }

        if (map.GetMapInput())
        {
            HandleWorldClick();
        }
    }

    private void HandleWorldClick()
    {
        TileEntity tile = map.GetTile();
        if (tile == null)
            return;

        if(tile.IsOccupied && selectedHero == null)
        {
            foreach (var hero in heroes)
            {
                if (hero.currentTile.TilePos == tile.Data.TilePos)
                    selectedHero = hero.SelectHero(Id);
                else
                    hero.Unselect();
            }
            return;
        }

        if (tile.IsOccupied && selectedHero != null && tile.occupyingHero == selectedHero)
        {
            selectedHero.Unselect();
            selectedHero = null;
            return;
        }

        if (selectedHero != null && selectedHero.ControllingPlayerId == Id)
        {
            if (selectedHero.PerformAction(tile)) {
                selectedHero.Unselect();
                selectedHero = null;
            };

            return;
        }
    }

    private void UnselectHero(HeroController hero)
    {
        hero.Unselect();
        selectedHero = null;
    }
}
