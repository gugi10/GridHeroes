using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroState
{
    Idle,
    SpecialAbility,
    Dead
}

public class HeroController : MonoBehaviour
{
    public int ControllingPlayerId;
    public TileData currentTile { get; private set; }
    public Action<HeroController> onHeroSelected;
    public Action<HeroAction> onActionEvent { get; set; }
    public Action onDie { get; set; }
    public Action OnMoveStart { get; set; }
    public Action onHeroUnselected;
    public int RemainingActions { get; private set; }

    [SerializeField] private HeroStatisticSheet originalStats;
    [SerializeField] private Transform rotationNode;
    [SerializeField] private AreaOutline areaPrefab;

    private Action<HeroAction> onActionCallback { get; set; }
    private HeroStatisticSheet currentStats;
    private Color defaultColor;
    private MapEntity map;
    private Coroutine movingCoroutine;
    private AreaOutline area;
    private AreaOutline heroHighLight;
    private ISpecialAbility[] specialAbilities;
    private HeroState heroState;

    private void Awake()
    {
        currentStats = originalStats;
        RemainingActions = currentStats.ActionLimit;

        area = Instantiate(areaPrefab, Vector3.zero, Quaternion.identity, transform);
        area.gameObject.SetActive(false);

        heroHighLight = Instantiate(areaPrefab, Vector3.zero, Quaternion.identity, transform);
        specialAbilities = GetComponents<ISpecialAbility>();
        heroState = HeroState.Idle;
    }

    public void Init(Action<HeroAction> onActionCallback)
    {
        this.onActionCallback = onActionCallback;
    }

    public void SetupHero(MapEntity map, TileData startingTile)
    {

        currentTile = startingTile;
        this.map = map;
        area.Show(map.WalkableBorder(transform.position, currentStats.Move), map);
        heroHighLight.Show(map.WalkableBorder(transform.position, 0), map);
        if (ControllingPlayerId % 2 == 0)
            heroHighLight.ActiveState();
        else
            heroHighLight.InactiveState();
        var tile = map.Tile(currentTile.TilePos);
        transform.position = map.WorldPosition(tile);
        tile.OccupyTile(this);
    }

    /*public bool PerformAction(TileEntity targetTile)
    {
        if (remainingActions <= 0)
        {
            return false;

        }

        bool actionResult;
        if (targetTile.IsOccupied && targetTile.occupyingHero != null && ControllingPlayerId != targetTile.occupyingHero.ControllingPlayerId)
        {
            actionResult = Attack(targetTile);
        }
        else
        {
            actionResult = Move(targetTile);
        }

        remainingActions = actionResult ? remainingActions - 1 : remainingActions;

        Debug.Log($"Action result = {actionResult}, Remaining actions = {remainingActions}");
        return actionResult;
    }*/

    public void DoSpecialAbility()
    {
        heroState = HeroState.SpecialAbility;
        specialAbilities[0].DoSpecialAbility(this, map);
        heroState = HeroState.Idle;
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
            if (Mathf.Abs(targetTile.Position.x - currentTile.TilePos.x) <= currentStats.Move
                && Mathf.Abs(targetTile.Position.y - currentTile.TilePos.y) <= currentStats.Move && Mathf.Abs(targetTile.Position.z - currentTile.TilePos.z) <= currentStats.Move)
            {
                map.Tile(currentTile.TilePos).FreeTile();
                currentTile = targetTile.Data;
                OnMoveStart?.Invoke();
                movingCoroutine = StartCoroutine(MoveAnimation(targetTile));
                //transform.position = map.WorldPosition(targetTile);

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

    public bool MoveByPath(List<TileEntity> path)
    {
        if (map == null || currentTile == null || path == null)
        {
            Debug.LogError($"Hero Controller map or currentTile is null to use MOVE function you need to specify them first.");
            return false;
        }
        if (movingCoroutine != null)
            StopCoroutine(movingCoroutine);
        map.Tile(currentTile.TilePos).FreeTile();
        currentTile = path[path.Count - 1].Data;
        OnMoveStart?.Invoke();
        movingCoroutine = StartCoroutine(Moving(path));
        return true;
    }

    public bool Attack(TileEntity targetTile)
    {

        if (CheckTileRange(currentTile.TilePos, targetTile.Data.TilePos, currentStats.WeaponRange))
        {
            if (targetTile.IsOccupied)
            {
                if (targetTile.occupyingHero != null && targetTile.occupyingHero != this)
                {
                    targetTile.occupyingHero.DealDamage(currentStats.WeaponDamage);
                    onActionCallback?.Invoke(HeroAction.Attack);
                    onActionEvent?.Invoke(HeroAction.Attack);
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
            Debug.LogError($"Cannot perform attack your weapon range is {currentStats.WeaponRange} and target is too far");
            return false;
        }

    }

    public void DealDamage(int damage)
    {
        currentStats.Health -= damage;
        if(currentStats.Health <= 0)
        {
            map.Tile(currentTile.TilePos).FreeTile();
            onDie?.Invoke();
            StartCoroutine(RemoveModel());
        }
    }

    public bool CheckTileRange(Vector3 sourceTile, Vector3 targetTile, int range)
    {
        return (Mathf.Abs(targetTile.x - sourceTile.x) <= range
                && Mathf.Abs(targetTile.y - sourceTile.y) <= range && Mathf.Abs(targetTile.z - sourceTile.z) <= range);
    }

    public HeroController SelectHero(int playerId)
    {
        area.gameObject.SetActive(true);
        if (ControllingPlayerId == playerId)
        {
            //meshRender.material.color = Color.green;
        }
        else
        {
            //meshRender.material.color = Color.red;
        }
        onHeroSelected?.Invoke(this);

        return this;
    }

    public void Unselect()
    {
        area.gameObject.SetActive(false);
        //meshRender.material.color = defaultColor;
        onHeroUnselected?.Invoke();
    }

    //Debug method to see which unit is selected
    public void SetColor(Color color)
    {
        defaultColor = color;
        //meshRender.material.color = color;
    }

    public void ResetActions()
    {
        RemainingActions = currentStats.ActionLimit;
    }

    public Tuple<HeroStatisticSheet, HeroStatisticSheet> GetHeroStats()
    {
        return new Tuple<HeroStatisticSheet, HeroStatisticSheet>(currentStats, originalStats);
    }

    private IEnumerator MoveAnimation(TileEntity targetTile)
    {
        var targetPoint = map.WorldPosition(targetTile);
        var stepDir = (targetPoint - transform.position) * 1;
        if (map.RotationType == RotationType.LookAt)
        {
            rotationNode.rotation = Quaternion.LookRotation(stepDir, Vector3.up);
        }
        else if (map.RotationType == RotationType.Flip)
        {
            rotationNode.rotation = map.Settings.LookAt(stepDir);
        }

        var reached = stepDir.sqrMagnitude < 0.01f;
        while (!reached)
        {
            transform.position += stepDir * Time.deltaTime * 1f;
            reached = Vector3.Dot(stepDir, (targetPoint - transform.position)) < 0f;
            yield return null;
        }

        targetTile.OccupyTile(this);
        transform.position = targetPoint;
        onActionCallback?.Invoke(HeroAction.Move);
        onActionEvent?.Invoke(HeroAction.Move);
    }

    IEnumerator Moving(List<TileEntity> path)
    {
        var nextIndex = 0;
        transform.position = map.Settings.Projection(transform.position);

        while (nextIndex < path.Count)
        {
            var targetPoint = map.WorldPosition(path[nextIndex]);
            var stepDir = (targetPoint - transform.position) * 1;
            if (map.RotationType == RotationType.LookAt)
            {
                rotationNode.rotation = Quaternion.LookRotation(stepDir, Vector3.up);
            }
            else if (map.RotationType == RotationType.Flip)
            {
                rotationNode.rotation = map.Settings.Flip(stepDir);
            }
            var reached = stepDir.sqrMagnitude < 0.01f;
            while (!reached)
            {

                transform.position += stepDir * Time.deltaTime;
                reached = Vector3.Dot(stepDir, (targetPoint - transform.position)) < 0f;
                yield return null;
            }
            transform.position = targetPoint;
            nextIndex++;
        }
        path[path.Count - 1].OccupyTile(this);
        onActionCallback?.Invoke(HeroAction.Move);
        onActionEvent?.Invoke(HeroAction.Move);
    }

    IEnumerator RemoveModel()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
