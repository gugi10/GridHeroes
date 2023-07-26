using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IHeroAction
{
    public bool MakeAction();
}

public interface IHeroActionAnimation
{
    public bool MakeAnimation();
}

public class HeroAttackAction : IHeroAction
{
    private TileEntity targetTile;
    private TileData currentTile;
    private HeroStatisticSheet currentStats;
    private HeroController controllingHero;

    public HeroAttackAction(TileEntity targetTile, TileData currentTile, HeroStatisticSheet currentStats, HeroController controllingHero)
    {
        this.targetTile = targetTile;
        this.currentTile = currentTile;
        this.currentStats = currentStats;
        this.controllingHero = controllingHero;
    }

    public bool MakeAction()
    {
        if (Utility.CheckTileRange(currentTile.TilePos, targetTile.Data.TilePos, currentStats.WeaponRange))
        {
            if (targetTile.IsOccupied)
            {
                if (targetTile.occupyingHero != null && targetTile.occupyingHero != controllingHero)
                {
                    targetTile.occupyingHero.DealDamage(currentStats.WeaponDamage);

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
}

public class HeroActionWithMoveAnimation : IHeroActionAnimation
{
    private MapEntity map;
    private Animator animator;
    private int walkingHash;
    private Transform transform;
    private Transform rotationNode;
    private HeroController heroController;
    private TileEntity targetTile;

    public HeroActionWithMoveAnimation(MapEntity map, Animator animator, int walkingHash, Transform transform, Transform rotationNode, HeroController heroController,
        TileEntity targetTile)
    {
        this.map = map;
        this.animator = animator;
        this.walkingHash = walkingHash;
        this.transform = transform;
        this.rotationNode = rotationNode;
        this.heroController = heroController;
        this.targetTile = targetTile;
    }

    public bool MakeAnimation()
    {
        bool isWalking = animator.GetBool(walkingHash);

        if (isWalking)
            return false;

        heroController.StartCoroutine(MoveAnimation(targetTile));
        return true;
    }

    private IEnumerator MoveAnimation(TileEntity targetTile)
    {
        animator.SetBool(walkingHash, true);

        var targetPoint = map.WorldPosition(targetTile);
        var stepDir = (targetPoint - transform.position) * 8;
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

        targetTile.OccupyTile(heroController);
        transform.position = targetPoint;
        animator.SetBool(walkingHash, false);
    }
}

public class HeroMoveAction : IHeroAction
{
    private TileEntity targetTile;
    private TileData currentTile;
    private MapEntity map;
    private HeroAttackAction attackAction;
    private HeroStatisticSheet currentStats;

    public HeroMoveAction(TileEntity targetTile, TileData currentTile, MapEntity map, HeroAttackAction attackAction, HeroStatisticSheet currentStats)
    {
        this.targetTile = targetTile;
        this.currentTile = currentTile;
        this.map = map;
        this.attackAction = attackAction;
        this.currentStats = currentStats;
    }

    public bool MakeAction()
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

                //targetTile.OccupyTile(this);
                //onActionCallback?.Invoke(HeroAction.Move);

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
            attackAction.MakeAction();
            Debug.LogError($"Vacant is false or Is occupied");
        }
        return false;
    }
}

public class HeroActionWithActionUpdate : IHeroAction
{
    IHeroAction action;
    HeroStatisticSheet heroData;

    public HeroActionWithActionUpdate(IHeroAction action, HeroStatisticSheet heroData)
    {
        this.action = action;
        this.heroData = heroData;
    }

    public bool MakeAction()
    {
        if (action.MakeAction())
        {
            heroData.ActionLimit--;
            return true;
        }
        return false;
    }
}

public class HeroController : MonoBehaviour
{
    public int ControllingPlayerId;
    public TileData currentTile { get; private set; }
    public Action<HeroController> onHeroSelected;
    public Action onHeroUnselected;
    public int RemainingActions { get; private set; }

    [SerializeField] private HeroStatisticSheet originalStats;
    [SerializeField] private Transform rotationNode;
    [SerializeField] private MeshRenderer meshRender;
    [SerializeField] private AreaOutline areaPrefab;

    private HeroStatisticSheet currentStats;
    private Color defaultColor;
    private MapEntity map;
    private Action<HeroAction> onActionCallback;
    private Coroutine movingCoroutine;
    private Animator animator;
    private AreaOutline area;
    private AreaOutline heroHighLight;

    private int walkingHash;
    private int attackHash;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        walkingHash = Animator.StringToHash("IsWalking");
        attackHash = Animator.StringToHash("Attack");

        currentStats = new HeroStatisticSheet(originalStats);
        RemainingActions = currentStats.ActionLimit;

        area = Instantiate(areaPrefab, Vector3.zero, Quaternion.identity, transform);
        area.gameObject.SetActive(false);
        area.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        heroHighLight = Instantiate(areaPrefab, Vector3.zero, Quaternion.identity, transform);
        heroHighLight.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

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

    public bool Move(TileEntity targetTile)
    {
        var heroAttackAction = new HeroAttackAction(targetTile, currentTile, onActionCallback, currentStats, this);
        var moveAction = new HeroMoveAction(targetTile, currentTile, map, heroAttackAction, currentStats);
        var fullAction = new HeroActionWithActionUpdate(moveAction, currentStats);

        if (fullAction.MakeAction())
            return new HeroActionWithMoveAnimation(map, animator, walkingHash, transform, rotationNode, this, onActionCallback, targetTile).MakeAnimation();

        return false;
    }

    public bool Attack(TileEntity targetTile)
    {
        var heroAttackAction = new HeroAttackAction(targetTile, currentTile, onActionCallback, currentStats, this);
        var fullAction = new HeroActionWithActionUpdate(heroAttackAction, currentStats);

        return fullAction.MakeAction();
    }

    public void DealDamage(int damage)
    {
        currentStats.Health -= damage;
        if(currentStats.Health <= 0)
        {
            gameObject.SetActive(false);
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
            meshRender.material.color = Color.green;
        }
        else
        {
            meshRender.material.color = Color.red;
        }
        onHeroSelected?.Invoke(this);

        return this;
    }

    public void Unselect()
    {
        area.gameObject.SetActive(false);
        meshRender.material.color = defaultColor;
        onHeroUnselected?.Invoke();
    }

    //Debug method to see which unit is selected
    public void SetColor(Color color)
    {
        defaultColor = color;
        meshRender.material.color = color;
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
        animator.SetBool(walkingHash, true);

        var targetPoint = map.WorldPosition(targetTile);
        var stepDir = (targetPoint - transform.position) * 8;
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
        animator.SetBool(walkingHash, false);
    }
}


