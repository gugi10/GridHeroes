using System;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using UnityEngine;

namespace SpecialAbilities
{
    public class TargetableAbility : AbilityBase
    {
        public AffectedTilesHiglight AffectedTile { get; set; }


        protected virtual void Awake()
        {
            AffectedTile = new AffectedTilesHiglight();
        }

        public override void HighlightAffectedTiles(TileEntity tile, MapController map)
        {
            if (tile == null || map == null)
            {
                Debug.LogError($"Targetable abillity higlight failed because tile or map is null");
                return;
            }
            
            if(TileUtilities.AreTilesInRange(source.currentTile.TilePos, tile.Position, GetAbilitySpec().properties.range))
                AffectedTile.HighlightTile(tile, map);   
        }

        public override void DisableHighlight(MapController map)
        {
            AffectedTile.DisableHiglight(map);
        }
        
        public override void ProcessInput()
        {
            if (_mapEntity == null)
            {
                return;
            }
            if (MyInput.GetOnWorldUp(_mapEntity.Settings.Plane()))
            {
                var clickPos = MyInput.GroundPosition(_mapEntity.Settings.Plane());
                TileEntity tile = _mapEntity.Tile(clickPos);
                PerformAbility(tile);
            }
        }
    }
}