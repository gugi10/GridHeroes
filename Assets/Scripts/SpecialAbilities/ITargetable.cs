using RedBjorn.ProtoTiles;

namespace SpecialAbilities
{
    public interface TODOITargetable
    {
        public AffectedTilesHiglight AffectedTile { get; set; }
        public void HighlightTargetedTile(TileEntity tile, MapController map);
        public void DisableHighlight(MapController map);
    }
}