using RedBjorn.ProtoTiles;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public int Id { get; private set; }
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

    public void Init(MapController map, List<HeroController> ownedHeroes, int playerId)
    {
        this.map = map;
        heroes = ownedHeroes;
        Id = playerId;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    private void HandleWorldClick()
    {
        TileEntity tile = map.GetTile();
        if (tile == null)
            return;

        if(tile.IsOccupied && selectedHero == null)
        {
            foreach (var hero in heroes)
            {
                if (hero.currentTile.TilePos == tile.Data.TilePos)
                    selectedHero = hero.SelectHero(Id);
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
            else if(!tile.IsOccupied && HasAction(HeroAction.Move))
            {
                if (selectedHero.Move(tile))
                {
                    UnselectHero();
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
}
