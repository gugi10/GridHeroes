using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public int ControllingPlayer;

    [SerializeField] HeroStatisticSheet stats;
    HeroControllerPayload payload;
    HeroStatus heroStatus;
    TileData currentTile;

    public void SetupHero(HeroControllerPayload payload)
    {
        
        this.payload = payload;
        currentTile = payload.StartingTile;
        heroStatus = new HeroStatus(stats);
        Move(payload.Map.Tile(currentTile.TilePos));
        //TODO: added here so move spawn fucntion does not consume action point
        heroStatus = new HeroStatus(stats);
    }

    public bool Move(TileEntity targetTile)
    {
        if (payload == null)
        {
            Debug.LogError($"Hero Controller payload is null to use MOVE function you need to specify payload first.");
            return false;
        }
        if (targetTile.Vacant && !targetTile.IsOccupied)
        {
            if (Mathf.Abs(targetTile.Position.x - currentTile.TilePos.x) <= stats.Move
                && Mathf.Abs(targetTile.Position.y - currentTile.TilePos.y) <= stats.Move && Mathf.Abs(targetTile.Position.z - currentTile.TilePos.z) <= stats.Move)
            {
                if (currentTile != null)
                    payload.Map.Tile(currentTile.TilePos).FreeTile();
                currentTile = targetTile.Data;
                transform.position = payload.Map.WorldPosition(targetTile);
                targetTile.OccupyTile(this);
                UpdateActionPoints(-1);
                return true;
            }
            else
            {
                Debug.LogError($"Tile out of hero movement range");
                return false;
            }
        }
        else
        {
            Attack(targetTile);
            Debug.LogError($"Vacant is false or Is occupied");
        }
        return false;
    }

    public bool Attack(TileEntity targetTile)
    {
        if(heroStatus.CurrentStats.ActionPoints <= 0)
        {
            Debug.LogError($"Cannot perform action, out of action points");
        }
        if (CheckTileRange(currentTile.TilePos, targetTile.Data.TilePos, stats.WeaponRange))
        {
            if (targetTile.IsOccupied)
            {
                if (targetTile.occupingHero != null)
                {
                    targetTile.occupingHero.DealDamage(stats.WeaponDamage);
                    UpdateActionPoints(-1);
                    return true;
                }
                else
                {
                    Debug.LogError($"Target hex is not occupied by hero");
                    return false;
                }
            }
            Debug.LogError($"Cannot attack unoccupied hexes");
            return false;
        }
        else
        {
            Debug.LogError($"Cannot perform attack your weapon range is {stats.WeaponRange} and target is too far");
            return false;
        }

    }

    public void ResetActionPoints()
    {
        heroStatus.CurrentStats.ActionPoints = stats.ActionPoints;
    }

    public void DealDamage(int damage)
    {
        Debug.Log($"{gameObject.name} is dealt {damage} damage");
    }

    public bool CheckTileRange(Vector3 sourceTile, Vector3 targetTile, int range)
    {
        return (Mathf.Abs(targetTile.x - sourceTile.x) <= range
                && Mathf.Abs(targetTile.y - sourceTile.y) <= range && Mathf.Abs(targetTile.z - sourceTile.z) <= range);
    }

    private void UpdateActionPoints(int difference)
    {
        heroStatus.CurrentStats.ActionPoints += difference;
        if(heroStatus.CurrentStats.ActionPoints <= 0)
        {
            Debug.Log($"Finished turned");
        }
    }

    public class HeroStatus
    {
        public HeroStatus(HeroStatisticSheet currentStats)
        {
            CurrentStats = currentStats;
        }

        public HeroStatisticSheet CurrentStats { get; private set; }
    }

    public class HeroControllerPayload
    {
        public MapEntity Map { get; private set; }
        public TileData StartingTile { get; private set; }
        public HeroControllerPayload(MapEntity map, TileData startingTile)
        {
            Map = map;
            StartingTile = startingTile;
        }
    }
}
