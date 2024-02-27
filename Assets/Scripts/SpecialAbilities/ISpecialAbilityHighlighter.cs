using RedBjorn.ProtoTiles;
using UnityEngine;

public interface ISpecialAbilityHighlighter
{
    public AffectedTilesHiglight GetAffectedTiles();
    public void HighlightTargetedTile(TileEntity tile, MapController map);
    public void DisableHighlight(MapController map);
}

public class SingleTargetTileHighlight : ISpecialAbilityHighlighter
{

    private AffectedTilesHiglight affectedTiles { get; }
    private HeroController source { get; }
    private BasicProperties properties { get; }
    public SingleTargetTileHighlight(AffectedTilesHiglight affectedTiles, HeroController source, BasicProperties properties)
    {
        this.affectedTiles = affectedTiles;
        this.source = source;
        this.properties = properties;
    }

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


public class WhirlwindTileHighlight : ISpecialAbilityHighlighter
{
    private AffectedTilesHiglight affectedTiles { get; }
    private HeroController source { get; }
    private MapEntity mapEntity { get; }
    private BasicProperties properties { get; }

    public WhirlwindTileHighlight(AffectedTilesHiglight affectedTiles, HeroController source, MapEntity mapEntity, BasicProperties properties)
    {
        this.affectedTiles = affectedTiles;
        this.source = source;
        this.mapEntity = mapEntity;
        this.properties = properties;
    }

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