using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class TurnSequenceController : MonoBehaviour
{
    public static TurnSequenceController Instance { get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<TurnSequenceController>();
            }
            return instance;
        }
        
    }
    private static TurnSequenceController instance;
    [SerializeField] List<HeroListWrapper> units = new();
    [SerializeField] MapController mapController;
    [SerializeField] PlayerInput playerInputPrefab;

    public int ActivePlayer { get; private set; }

    public Action<List<List<HeroAction>>> onRoundStart;
    public Action<List<List<HeroAction>>> onTurnFinished;
    public Action<HeroController> onHeroSelected;
    public Action onHeroUnselected;

    private const int NUMBER_OF_PLAYERS = 2;
    private const int MAX_ACTIONS = 5;

    private List<IPlayer> players = new();
    private List<HeroController> heroControllerInstances = new();
    private List<List<HeroAction>> playersRemainingActions = new();

    private void Awake()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Units.ForEach(hero =>
            {
                var instance = Instantiate(hero);
                instance.ControllingPlayerId = i;
                instance.Init(FinishTurn, () => { FinishTurn(HeroAction.Special); }, this.OnDie);
                instance.onHeroSelected += OnHeroSelectedCallback;
                instance.onHeroUnselected += OnHeroSelectedCallback;
                heroControllerInstances.Add(instance);
            });
        }
    }

    private void Start()
    {
        playersRemainingActions.Add(GenerateActionList());
        var player = Instantiate(playerInputPrefab);
        player.gameObject.name = $"Player_0";
        player.Init(mapController, heroControllerInstances, 0);
        players.Add(player);

        playersRemainingActions.Add(GenerateActionList());
        var ai = new GameObject().AddComponent<AIAgent>();
        ai.gameObject.name = $"AI";
        ai.Init(mapController, heroControllerInstances, 1);
        players.Add(ai);

        onRoundStart?.Invoke(playersRemainingActions);

        
        mapController.SpawnHeroesRandomly(heroControllerInstances);
        SetActivePlayer(UnityEngine.Random.Range(0, NUMBER_OF_PLAYERS));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NextPlayer();
        }
    }

    public List<HeroAction> GetPlayerRemainingActions(int playerId)
    {
        return playersRemainingActions[playerId];
    }


    public void FinishTurn(HeroAction heroAction)
    {
        // First we need to remove the corresponding action and then we MUST call
        // finish round before switching active player because it generates new actions.    
        // If we don't do it first the AI will try to make an action while its action table
        // will be empty which will cause out of bound exception.
        if (playersRemainingActions[ActivePlayer].Contains(heroAction))
            playersRemainingActions[ActivePlayer].Remove(heroAction);
        else
            playersRemainingActions[ActivePlayer].Remove(HeroAction.Special);

        Debug.Log($"{playersRemainingActions[0]}, {playersRemainingActions[1]}");
        if (playersRemainingActions.All(x => x.Count() == 0))
        {
            FinishRound();
        }

        NextPlayer();
        onTurnFinished?.Invoke(playersRemainingActions);
    }

    private void OnDie(HeroController hero)
    {
        heroControllerInstances.Remove(hero);
    }

    private void FinishRound()
    {
        playersRemainingActions = playersRemainingActions.Select(_ => GenerateActionList()).ToList();
        heroControllerInstances.ForEach(hero => hero.ResetActions());
        onRoundStart?.Invoke(playersRemainingActions);
    }

    private int CalculateNextPlayer()
    {
        return Mathf.Abs(ActivePlayer - 1);
    }

    private void NextPlayer()
    {
        SetActivePlayer(CalculateNextPlayer());
    }

    private void SetActivePlayer(int playerId)
    {
        Debug.Log($"Currently active player {playerId}");
        ActivePlayer = playerId;
        players.ForEach(player =>
        {
            player.SetActiveState(player.Id == playerId);
        });
    }

    private List<HeroAction> GenerateActionList()
    {
        var actions = Enumerable.Range(0, MAX_ACTIONS).Select(_ => HeroActionChooser.ChooseRandomAction ()).ToList();
        foreach (var action in actions)
        {
            Debug.Log($"{action}");
        }
        return actions;
    }

    private void OnHeroSelectedCallback(HeroController hero)
    {
        onHeroSelected?.Invoke(hero);
    }

    private void OnHeroSelectedCallback()
    {
        onHeroUnselected?.Invoke();
    }

}

[System.Serializable]
public class HeroListWrapper
{
    public List<HeroController> Units;
}
