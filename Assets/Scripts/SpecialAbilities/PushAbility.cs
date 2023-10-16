using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PushAbility : AbilityBase
{
    private Properties properties = new() { damage = 0, range = 1 };
    private HeroController source;
    private MapEntity map;
    public struct Properties
    {
        public int range;
        public int damage;
    }
    public override void DoSpecialAbility(HeroController source, MapEntity map)
    {
        this.source = source;
        this.map = map;
    }

    public override AbilitySpec GetAbilitySpec()
    {
        return new AbilitySpec { kind = AbilityKind.PushStrike, properties = properties };
    }
    public override void ProcessInput()
    {
        if (map == null)
        {
            return;
        }
        if (MyInput.GetOnWorldUp(map.Settings.Plane()))
        {
            var clickPos = MyInput.GroundPosition(map.Settings.Plane());
            TileEntity tile = map.Tile(clickPos);
            PerformAbility(tile);
        }
    }

    public override void PerformAbility(TileEntity chosenTile)
    {

        if (chosenTile == null)
            return;

        if (chosenTile.IsOccupied)
        {
            if (TileUtilities.AreTilesInRange(source.currentTile.TilePos, chosenTile.Position, properties.range) &&
                chosenTile.occupyingHero != source && chosenTile.occupyingHero.ControllingPlayerId != source.ControllingPlayerId)
            {

                source.LookAt(map.WorldPosition(chosenTile));
                Vector3Int newPos = new Vector3Int((chosenTile.occupyingHero.currentTile.TilePos.x - source.currentTile.TilePos.x) + chosenTile.occupyingHero.currentTile.TilePos.x,
                    (chosenTile.occupyingHero.currentTile.TilePos.y - source.currentTile.TilePos.y)+ chosenTile.occupyingHero.currentTile.TilePos.y,
                    (chosenTile.occupyingHero.currentTile.TilePos.z - source.currentTile.TilePos.z)+ chosenTile.occupyingHero.currentTile.TilePos.z);
                Debug.Log($"source tile pos:{source.currentTile.TilePos} target tile pos{chosenTile.occupyingHero.currentTile.TilePos} push tile pos{newPos}");
                var newTile = map.Tile(newPos);
                if(newTile != null)
                    chosenTile.occupyingHero.Move(newTile);
                source?.onSpecialAbilityFinished();
            }
            //TODO:Play particle effect

        }
    }
}
