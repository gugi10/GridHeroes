using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileUtilities
{
    public static bool AreTilesInRange(Vector3 sourceTile, Vector3 targetTile, int range)
    {
        return (Mathf.Abs(targetTile.x - sourceTile.x) <= range
                && Mathf.Abs(targetTile.y - sourceTile.y) <= range && Mathf.Abs(targetTile.z - sourceTile.z) <= range);
    }
}
 