using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public interface IPlayer
{
    public void Init(MapController map, List<HeroController> allHeroes, int playerId);
    public void SetActiveState(bool flag);
    public int Id { get; set; }

}

public class PlayerInput : MonoBehaviour, IPlayer
{
    public int Id { get; set; }
    [SerializeField] PathDrawer PathPrefab;
    private PathDrawer path;
    private MapController map;
    private List<HeroController> heroes = new List<HeroController>();
    private HeroController selectedHero;
    private List<List<HeroAction>> playerActions;


    private void OnEnable()
    {
        TurnSequenceController.Instance.onRoundStart += SetPlayerActions;
        TurnSequenceController.Instance.onTurnFinished += SetPlayerActions;
    }

    private void OnDisable()
    {
        TurnSequenceController.Instance.onRoundStart -= SetPlayerActions;
        TurnSequenceController.Instance.onTurnFinished -= SetPlayerActions;
    }

    private void Start()
    {

    }

    public void Init(MapController map, List<HeroController> ownedHeroes, int playerId)
    {
        this.map = map;
        heroes = ownedHeroes;
        heroes.ForEach(hero =>
        {
            hero.onSpecialAbilityStarted += BlockInput;
            hero.onSpecialAbilityFinished += FreeInput;
        }); 

        Id = playerId;

        path = Instantiate(PathPrefab, transform);
        path.InactiveState();
        path.IsEnabled = true;
        path.Hide();
    }

    private void BlockInput()
    {
        SetActiveState(false);
    }

    private void FreeInput()
    {
        SetActiveState(true);
    }

    private void SetPlayerActions(List<List<HeroAction>> actions)
    {
        playerActions = new List<List<HeroAction>>(actions);
    }

    void Update()
    {
        if (map == null)
        {
            return;
        }
        if (map.GetMapInput())
        {
            HandleWorldClick();
        }

        if (path != null)
        {
            PathUpdate();
        }
    }

    private void HandleWorldClick()
    {
        TileEntity tile = map.GetTile();
        if (tile == null)
            return;

        if (tile.IsOccupied && selectedHero == null)
        {
            foreach (var hero in heroes)
            {
                if (hero.currentTile.TilePos == tile.Data.TilePos)
                {
                    selectedHero = hero.SelectHero(Id);
                }
                else
                    hero.Unselect();
            }
            return;
        }

        if (tile.IsOccupied && selectedHero != null && tile.occupyingHero == selectedHero)
        {
            selectedHero.Unselect();
            selectedHero = null;
            return;
        }

        if (selectedHero != null && selectedHero.ControllingPlayerId == Id)
        {
            if (IsTargetingEnemy(tile) && HasAction(HeroAction.Attack))
            {
                if (selectedHero.Attack(tile))
                {
                    UnselectHero();
                }
            }
            else if (!tile.IsOccupied && HasAction(HeroAction.Move))
            {
                if (MyInput.GetOnWorldUp(map.mapEntity.Settings.Plane()))
                {
                    HandleMovePath();
                }

            }

            return;
        }
    }

    private void UnselectHero()
    {
        selectedHero.Unselect();
        selectedHero = null;
    }

    private bool IsTargetingEnemy(TileEntity tile)
    {
        return tile.IsOccupied && tile.occupyingHero != null && selectedHero.ControllingPlayerId != tile.occupyingHero.ControllingPlayerId;
    }

    private bool HasAction(HeroAction action)
    {
        return playerActions[selectedHero.ControllingPlayerId].Contains(action);
    }

    public void SetActiveState(bool flag)
    {
        this.enabled = flag;
    }

    void PathHide()
    {
        if (path)
        {
            path.Hide();
        }
    }

    void PathUpdate()
    {
        if (selectedHero == null)
        {
            if (path.gameObject.activeSelf)
                PathHide();
            return;
        }

        if (path && path.IsEnabled)
        {
            var tile = map.mapEntity.Tile(MyInput.GroundPosition(map.mapEntity.Settings.Plane()));
            if (tile != null && tile.Vacant && !tile.IsOccupied)
            {
                var path = map.mapEntity.PathPoints(selectedHero.transform.position, map.mapEntity.WorldPosition(tile.Position), (float)selectedHero?.GetHeroStats().Item1.Move);
                this.path.Show(path, map.mapEntity);
                this.path.ActiveState();
            }
            else
            {
                path.InactiveState();
            }
        }
    }
    void HandleMovePath()
    {
        var clickPos = MyInput.GroundPosition(map.mapEntity.Settings.Plane());
        var tile = map.mapEntity.Tile(clickPos);
        if (tile != null && tile.Vacant && !tile.IsOccupied)
        {
            this.path.IsEnabled = false;
            PathHide();
            var path = map.mapEntity.PathTiles(selectedHero.transform.position, clickPos, (float)selectedHero?.GetHeroStats().Item1.Move);
            this.path.IsEnabled = true;
            if (selectedHero.MoveByPath(path))
            {
                UnselectHero();
            }
        }
        else
        {
            Debug.LogError($"tile is null");
        }
    }
}
