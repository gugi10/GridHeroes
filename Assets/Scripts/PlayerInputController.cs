using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private HeroController heroController;
    private MapEntity map;
    private TileData currentTile;

    public void Init(MapEntity map)
    {
        this.map = map;
    }
    void Update()
    {
        if (MyInput.GetOnWorldUp(map.Settings.Plane()))
        {
            HandleWorldClick();
        }
    }

    private void HandleWorldClick()
    {
        //TODO: perform this only on selected hero
        var clickPos = MyInput.GroundPosition(map.Settings.Plane());
        TileEntity tile = map.Tile(clickPos);
        if(tile != null)
        {
            heroController.Move(tile);
        }
        else
        {
            Debug.LogError($"tile clicked is null {clickPos}");
        }
    }

}
