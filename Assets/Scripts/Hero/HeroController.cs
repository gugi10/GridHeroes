using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct HeroStats
{
    public HeroStatisticSheet @base;
    public HeroStatisticSheet current;

    public HeroStats(HeroStatisticSheet original, HeroStatisticSheet current)
    {
        this.@base = original;
        this.current = current;
    }

    public HeroStats(HeroStatisticSheet stats)
    {
        this.@base = stats;
        this.current = stats;
    }
}

public enum HeroId
{
    EvilMage,
    Crab,
    BlackKnight
}

public class HeroController : MonoBehaviour
{
    public HeroId HeroId;
    public PlayerId ControllingPlayerId;
    public TileData currentTile { get; private set; }
    public TileEntity currentTileEntity { get; private set; }
    public Action<HeroController> onHeroSelected;
    public Action<HeroAction> onActionEvent { get; set; }
    public Action<HeroController> onDie { get; set; }
    public Action OnMoveStart { get; set; }
    public Action onHeroUnselected;
    public Action<ISpecialAbility> onSpecialAbilityStarted;
    public Action onSpecialAbilityFinished;
    public int RemainingActions { get; private set; }
    public ISpecialAbility[] specialAbilities { get; private set; }

    [SerializeField] private HeroStatisticSheet originalStats;
    [SerializeField] private Transform rotationNode;
    [SerializeField] private AreaOutline highLightPrefab;
    [SerializeField] private AreaOutline walkableAreaPrefab;

    private HeroStatisticSheet currentStats;
    private Color defaultColor;
    private MapEntity map;
    private Coroutine movingCoroutine;
    private AreaOutline area;
    private AreaOutline heroHighLight;
    private int walkAnimationSpeed = 2;

    private void Awake()
    {
        currentStats = new HeroStatisticSheet(originalStats);
        RemainingActions = currentStats.ActionLimit;

        area = Instantiate(walkableAreaPrefab, Vector3.zero, Quaternion.identity, transform);
        area.ActiveState();
        area.gameObject.SetActive(false);

        heroHighLight = Instantiate(highLightPrefab, Vector3.zero, Quaternion.identity, transform);
        specialAbilities = GetComponents<ISpecialAbility>();
    }

    public void Init(Action<HeroAction> onActionCallback, Action onSpecialAbilityFinished, Action<HeroController> onDie)
    {
        this.onActionEvent += onActionCallback;
        this.onSpecialAbilityFinished += onSpecialAbilityFinished;
        this.onDie += onDie;
    }

    public bool CanMakeTask(PossibleTask task)
    {
        switch (task.task.kind)
        {
            case TaskKind.UseAbility:
                switch (specialAbilities[0].GetAbilitySpec().kind)
                {
                    case AbilityKind.Whirlwind:
                    case AbilityKind.PushStrike:
                    case AbilityKind.Bolt:
                        return true;
                }
                return false;
            case TaskKind.MoveForward:
            case TaskKind.AttackEnemy:
                return true;
        }
        return false;
    }

    public void SetupHero(MapEntity map, TileData startingTile)
    {
        rotationNode.localEulerAngles = new Vector3(0, ControllingPlayerId == 0 ? 90 : -90, 0);
        this.map = map;
        currentTile = startingTile;
        currentTileEntity = map.Tile(startingTile.TilePos);
        
        var tile = map.Tile(currentTile.TilePos);
        transform.position = map.WorldPosition(tile);

        heroHighLight.Show(map.WalkableBorder(transform.position, 0), map);
        heroHighLight.transform.position = Vector3.zero;
        if (ControllingPlayerId == PlayerId.Human)
            heroHighLight.ActiveState();
        else
            heroHighLight.InactiveState();
        
        tile.OccupyTile(this);

        specialAbilities[0].DoSpecialAbility(this, map);
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

    public void DoSpecialAbility(int id)
    {
        onSpecialAbilityStarted?.Invoke(specialAbilities[id]);
        //onSpecialAbility.Invoke(specialAbilities[id].GetSkillAnimation());
        //specialAbilities[id].DoSpecialAbility(this, map);
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
                currentTileEntity = map.Tile(currentTile.TilePos);
                currentTileEntity.OccupyTile(this);
                OnMoveStart?.Invoke();
                movingCoroutine = StartCoroutine(Fly(targetTile));
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
        currentTileEntity = map.Tile(currentTile.TilePos);
        currentTileEntity.OccupyTile(this);
        OnMoveStart?.Invoke();
        movingCoroutine = StartCoroutine(Move(path));
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
                    LookAt(map.WorldPosition(targetTile));
                    targetTile.occupyingHero.DealDamage(currentStats.WeaponDamage);
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
            if(ControllingPlayerId == PlayerId.AI)
                if(GameSession.Instance.GetService<HeroService>().UnlockHero(HeroId))
                {
                    Debug.Log($"Just unlocked {HeroId}");
                }
            map.Tile(currentTile.TilePos).FreeTile();
            onDie?.Invoke(this);
            StartCoroutine(RemoveModel());
        }
    }

    public bool CheckTileRange(Vector3 sourceTile, Vector3 targetTile, int range)
    {
        return (Mathf.Abs(targetTile.x - sourceTile.x) <= range
                && Mathf.Abs(targetTile.y - sourceTile.y) <= range && Mathf.Abs(targetTile.z - sourceTile.z) <= range);
    }

    public HeroController SelectHero(PlayerId playerId)
    {
        area.gameObject.SetActive(true);
        area.transform.position = Vector3.zero;
        area.Show(map.WalkableBorder(transform.position, currentStats.Move), map);
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

    public HeroStats GetHeroStats()
    {
        return new HeroStats(originalStats, currentStats);
    }

    public void LookAt(Vector3 target)
    {
        rotationNode.LookAt(target);
    }
    private IEnumerator Fly(TileEntity targetTile)
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
        onActionEvent?.Invoke(HeroAction.Move);
    }

    IEnumerator Move(List<TileEntity> path)
    {
        var nextIndex = 0;
        transform.position = map.Settings.Projection(transform.position);

        while (nextIndex < path.Count)
        {
            var targetPoint = map.WorldPosition(path[nextIndex]);
            var stepDir = (targetPoint - transform.position) * walkAnimationSpeed;
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
        onActionEvent?.Invoke(HeroAction.Move);
    }

    IEnumerator RemoveModel()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
