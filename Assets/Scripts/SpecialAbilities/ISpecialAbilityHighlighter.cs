using RedBjorn.ProtoTiles;
using UnityEngine;

public interface ISpecialAbilityHighlighter
{
    public Sprite GetSkillIcon();
    public AffectedTilesHiglight GetAffectedTiles();
    public void HighlightTargetedTile(TileEntity tile, MapController map);
    public void DisableHighlight(MapController map);
}

class SingleTargetFx : ISpecialAbilityHighlighter
{

    private AffectedTilesHiglight affectedTiles;
    private Sprite skillIcon;
    private HeroController source;
    private BasicProperties properties;
    public SingleTargetFx(AffectedTilesHiglight affectedTiles, Sprite skillIcon, HeroController source, BasicProperties properties)
    {
        this.affectedTiles = affectedTiles;
        this.skillIcon = skillIcon;
        this.source = source;
        this.properties = properties;
    }

    public Sprite GetSkillIcon() { return skillIcon;  }
    public AffectedTilesHiglight GetAffectedTiles() { return affectedTiles; }
    public void HighlightTargetedTile(TileEntity tile, MapController map)
    {
        if (TileUtilities.AreTilesInRange(source.currentTile.TilePos, tile.Position, properties.range))
            affectedTiles.HighlightTile(tile, map);
    }
    public void DisableHighlight(MapController map)
    {
        affectedTiles.DisableHiglight(map);
    }
}


class WhirlwindFx : ISpecialAbilityHighlighter
{
    private AffectedTilesHiglight affectedTiles;
    private Sprite skillIcon;
    private HeroController source;
    private MapEntity mapEntity;
    private BasicProperties properties;

    public WhirlwindFx(AffectedTilesHiglight affectedTiles, Sprite skillIcon, HeroController source, MapEntity mapEntity, BasicProperties properties)
    {
        this.affectedTiles = affectedTiles;
        this.skillIcon = skillIcon;
        this.source = source;
        this.mapEntity = mapEntity;
        this.properties = properties;
    }

    public Sprite GetSkillIcon() { return skillIcon; }
    public AffectedTilesHiglight GetAffectedTiles() { return affectedTiles; }
    public void HighlightTargetedTile(TileEntity tile, MapController map)
    {
        affectedTiles.HighlightTile(mapEntity.WalkableTiles(source.currentTile.TilePos, properties.range), map);
    }

    public void DisableHighlight(MapController map)
    {
        affectedTiles.DisableHiglight(map);
    }
}