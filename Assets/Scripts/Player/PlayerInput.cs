using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Playables;

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
    private bool abilityInputIsProcessing;
    private ISpecialAbility processedAbility;
    private bool playerIsActive;


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

    public void Init(MapController map, List<HeroController> ownedHeroes, int playerId)
    {
        this.map = map;
        heroes = ownedHeroes;
        heroes.ForEach(hero =>
        {
            hero.onSpecialAbilityStarted += StartSpecialAbility;
            hero.onSpecialAbilityFinished += StopSpecialAbility;
        }); 

        Id = playerId;

        path = Instantiate(PathPrefab, transform);
        path.InactiveState();
        path.IsEnabled = true;
        path.Hide();
    }

    private void StartSpecialAbility(ISpecialAbility ability)
    {
        abilityInputIsProcessing = true;
        processedAbility = ability;
    }

    private void StopSpecialAbility()
    {
        abilityInputIsProcessing = false;
        processedAbility = null;
    }

    private void SetPlayerActions(List<List<HeroAction>> actions)
    {
        UnselectHero();
        playerActions = new List<List<HeroAction>>(actions);
    }

    void Update()
    {
        if (!playerIsActive)
        {
            return;
        }

        if (abilityInputIsProcessing)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                abilityInputIsProcessing = false;
                return;
            }    

            processedAbility.ProcessInput();
            return;
        }

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
        //Select hero
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
        //Unselect hero
        if (tile.IsOccupied && selectedHero != null && tile.occupyingHero == selectedHero)
        {
            UnselectHero();
            return;
        }
        //Attack or move
        if (selectedHero != null && selectedHero.ControllingPlayerId == Id)
        {
            if (IsTargetingEnemy(tile) && (HasAction(HeroAction.Attack) || HasAction(HeroAction.Special)))
            {
                if (selectedHero.Attack(tile))
                {
                    UnselectHero();
                }
            }
            else if (!tile.IsOccupied && (HasAction(HeroAction.Move) || HasAction(HeroAction.Special)))
            {
                //Return if tile is too far
                if (!TileUtilities.AreTilesInRange(selectedHero.currentTile.TilePos, tile.Position, selectedHero.GetHeroStats().current.Move))
                {
                    return;
                }

                if (MyInput.GetOnWorldUp(map.GetMapEntity().Settings.Plane()))
                {
                    HandleMovePath();
                }

            }

            return;
        }
    }

    private void UnselectHero()
    {
        if (selectedHero == null)
            return;

        PathHide();
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
        this.playerIsActive = flag;
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
        if (selectedHero == null || selectedHero.ControllingPlayerId != Id || selectedHero.GetHeroStats().current.Health <= 0)
        {
            if (path.gameObject.activeSelf)
                PathHide();
            return;
        }

        if (path && path.IsEnabled)
        {
            Debug.Log($"Show path");
            var tile = map.GetMapEntity().Tile(MyInput.GroundPosition(map.GetMapEntity().Settings.Plane()));
            if (tile != null && tile.Vacant && !tile.IsOccupied)
            {
                var path = map.GetMapEntity().PathPoints(selectedHero.transform.position, map.GetMapEntity().WorldPosition(tile.Position), (float)selectedHero?.GetHeroStats().current.Move);
                this.path.Show(path, map.GetMapEntity());
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
        var clickPos = MyInput.GroundPosition(map.GetMapEntity().Settings.Plane());
        var tile = map.GetMapEntity().Tile(clickPos);
        if (tile != null && tile.Vacant && !tile.IsOccupied)
        {
            this.path.IsEnabled = false;
            PathHide();
            var path = map.GetMapEntity().PathTiles(selectedHero.transform.position, clickPos, (float)selectedHero?.GetHeroStats().current.Move);
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
