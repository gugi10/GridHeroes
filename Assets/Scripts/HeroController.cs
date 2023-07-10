using RedBjorn.ProtoTiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroController : MonoBehaviour
{
    public int ControllingPlayerId;
    public TileData currentTile { get; private set; }

    [SerializeField] private HeroStatisticSheet stats;
    private MeshRenderer meshRender;
    private Color defaultColor;
    private MapEntity map;
    private Action onActionCallback;

    private void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
    }

    public void Init(Action onActionCallback)
    {
        this.onActionCallback = onActionCallback;
    }

    public void SetupHero(MapEntity map, TileData startingTile)
    {

        currentTile = startingTile;
        this.map = map;
        var tile = map.Tile(currentTile.TilePos);
        transform.position = map.WorldPosition(tile);
        tile.OccupyTile(this);
    }

    public bool Move(TileEntity targetTile)
    {
        if (map == null || currentTile == null)
        {
            Debug.LogError($"Hero Controller map or currentTile is null to use MOVE function you need to specify them first.");
            return false;
        }
        if (targetTile.Vacant && !targetTile.IsOccupied)
        {
            if (Mathf.Abs(targetTile.Position.x - currentTile.TilePos.x) <= stats.Move
                && Mathf.Abs(targetTile.Position.y - currentTile.TilePos.y) <= stats.Move && Mathf.Abs(targetTile.Position.z - currentTile.TilePos.z) <= stats.Move)
            {
                map.Tile(currentTile.TilePos).FreeTile();
                currentTile = targetTile.Data;
                transform.position = map.WorldPosition(targetTile);
                targetTile.OccupyTile(this);

                onActionCallback?.Invoke();
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

        if (CheckTileRange(currentTile.TilePos, targetTile.Data.TilePos, stats.WeaponRange))
        {
            if (targetTile.IsOccupied)
            {
                if (targetTile.occupyingHero != null)
                {
                    targetTile.occupyingHero.DealDamage(stats.WeaponDamage);
                    onActionCallback?.Invoke();
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

    public void DealDamage(int damage)
    {
        Debug.Log($"{gameObject.name} is dealt {damage} damage");
    }

    public bool CheckTileRange(Vector3 sourceTile, Vector3 targetTile, int range)
    {
        return (Mathf.Abs(targetTile.x - sourceTile.x) <= range
                && Mathf.Abs(targetTile.y - sourceTile.y) <= range && Mathf.Abs(targetTile.z - sourceTile.z) <= range);
    }

    public HeroController SelectHero(int playerId)
    {
        if (ControllingPlayerId == playerId)
        {
            meshRender.material.color = Color.green;
        }
        else
        {
            meshRender.material.color = Color.red;
        }
        return this;
    }

    public void Unselect()
    {
        meshRender.material.color = defaultColor;
    }

    //Debug method
    public void SetColor(Color color)
    {
        defaultColor = color;
        meshRender.material.color = color;
    }

}
