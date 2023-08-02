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

    [SerializeField] MapController mapController;
    [SerializeField] HeroController heroPrefab;

    public int ActivePlayer { get; private set; }

    public Action<List<List<HeroAction>>> onRoundStart;
    public Action<List<List<HeroAction>>> onTurnFinished;
    public Action<HeroController> onHeroSelected;
    public Action onHeroUnselected;

    private const int NUMBER_OF_PLAYERS = 2;
    private const int HEROES_TO_SPAWN = 6;
    private const int MAX_ACTIONS = 3;

    private List<HeroController> heroControllerInstances;
    private List<IPlayer> players = new();
    private List<List<HeroAction>> playersRemainingActions = new();

    private void Awake()
    {
        heroControllerInstances = Enumerable.Range(0, HEROES_TO_SPAWN).Select(i =>
        {
            var instance = Instantiate(heroPrefab);
            instance.ControllingPlayerId = i % 2;
            instance.Init(FinishTurn);
            instance.onHeroSelected += OnHeroSelectedCallback;
            instance.onHeroUnselected += OnHeroSelectedCallback;
            return instance;
        }).ToList();
    }

    private void Start()
    {
        playersRemainingActions.Add(GenerateActionList());
        var player = new GameObject().AddComponent<PlayerInput>();
        player.gameObject.name = $"Player_0";
        player.Init(mapController, heroControllerInstances, 0);
        players.Add(player);

        playersRemainingActions.Add(GenerateActionList());
        var ai = new GameObject().AddComponent<AIAgent>();
        ai.gameObject.name = $"AI";
        ai.Init(mapController, heroControllerInstances, 0);
        players.Add(ai);

        onRoundStart?.Invoke(playersRemainingActions);

        SetActivePlayer(UnityEngine.Random.Range(0, NUMBER_OF_PLAYERS));
        mapController.SpawnHeroesRandomly(heroControllerInstances);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NextPlayer();
        }
    }

    public void FinishTurn(HeroAction heroAction)
    {
        playersRemainingActions[ActivePlayer].Remove(heroAction);
        NextPlayer();
        Debug.Log($"{playersRemainingActions[0]}, {playersRemainingActions[1]}");
        onTurnFinished?.Invoke(playersRemainingActions);
        if (playersRemainingActions.All(x => x.Count() == 0))
        {
            FinishRound();
        }
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
        var actions = Enumerable.Range(0, MAX_ACTIONS).Select(_ => HeroActionChooser.ChooseRandomAction()).ToList();
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
