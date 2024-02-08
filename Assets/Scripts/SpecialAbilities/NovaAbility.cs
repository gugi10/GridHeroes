using RedBjorn.ProtoTiles;

namespace SpecialAbilities
{
    public class NovaAbility : AbilityBase
    {
        public AffectedTilesHiglight AffectedTile { get; set; }

        protected virtual void Awake()
        {
            AffectedTile = new AffectedTilesHiglight();
        }
        
        public override void HighlightAffectedTiles(TileEntity tile, MapController mapController)
        {
            AffectedTile.HighlightTile(_mapEntity.WalkableTiles(source.currentTile.TilePos, GetAbilitySpec().properties.range), mapController);
        }

        public override void DisableHighlight(MapController map)
        {
            AffectedTile.DisableHiglight(map);
        }
    }
}