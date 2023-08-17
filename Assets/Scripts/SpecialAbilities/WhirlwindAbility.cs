using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindAbility : MonoBehaviour, ISpecialAbility
{
    public int range = 1;
    public int damage = 1;

    [Header("Visual settings")]
    [SerializeField] string animationId = "Attack02";
    [SerializeField] private Sprite icon;
    public bool DoSpecialAbility(HeroController source, MapEntity map)
    {
        HashSet<TileEntity> surroundingTiles = map.WalkableTiles(source.currentTile.TilePos, range);
        foreach (var tile in surroundingTiles)
        {
            if (tile.IsOccupied && tile?.occupyingHero.ControllingPlayerId != source.ControllingPlayerId && tile?.occupyingHero != source)
                tile.occupyingHero.DealDamage(damage);
        }
        return false;
    }

    public string GetSkillAnimation()
    {
        return animationId;
    }

    public Sprite GetSkillIcon()
    {
        return icon;
    }
}
