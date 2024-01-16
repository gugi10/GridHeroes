using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AffectedTilesHiglight
{
    public void HighlighTile(HashSet<TileEntity> tilesCollection, MapController map)
    {
        foreach (var tile in tilesCollection)
        {
            HighlightTile(tile, map);
        }
    }

    public void HighlightTile(TileEntity tile, MapController map)
    {
        var foundTile = map.AccessibleTiles.FirstOrDefault(val => val.entity?.Data.TilePos == tile?.Data.TilePos);
        if (foundTile.representation != null && foundTile.entity != null)
            foundTile.representation.PlayAnimation();
    }

    //TODO: Right now it is suboptimal it could be improved by storing specific tiles on which the animation is playing
    public void DisableHiglight(MapController map)
    {
        foreach(var tile in map.AccessibleTiles)
        {
            tile.representation.StopAnimation();
        }
    }
}
