using RedBjorn.ProtoTiles;

namespace SpecialAbilities
{
    public interface ITargetable
    {
        public AffectedTilesHiglight AffectedTile { get; set; }
        public void HiglightTargetedTile(TileEntity tile, MapController map);
        public void DisableHiglight(MapController map);
    }
}