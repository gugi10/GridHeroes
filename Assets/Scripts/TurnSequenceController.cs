using RedBjorn.ProtoTiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnSequenceController : MonoBehaviour
{
    public static TurnSequenceController Instance { get; private set; }

    [SerializeField] MapController mapController;
    [SerializeField] HeroController heroPrefab;

    private const int NUMBER_OF_PLAYERS = 2;
    private const int HEROES_TO_SPAWN = 6;
    private const int MAX_ACTIONS = 3;



    private List<HeroController> heroControllerInstances;
    private List<PlayerInput> players = new();
    private int activePlayer;
    private List<List<HeroAction>> playersRemainingActions = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        heroControllerInstances = Enumerable.Range(0, HEROES_TO_SPAWN).Select(i =>
        {
            var instance = Instantiate(heroPrefab);
            instance.ControllingPlayerId = i % 2;
            instance.Init(FinishTurn);
            return instance;
        }).ToList();
    }

    private void Start()
    {
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            playersRemainingActions.Add(GenerateActionList());
            var player = new GameObject().AddComponent<PlayerInput>();
            player.gameObject.name = $"Player_{i}";
            player.Init(mapController, heroControllerInstances, i);
            players.Add(player);
        }

        SetActivePlayer(Random.Range(0, NUMBER_OF_PLAYERS));
        mapController.SpawnHeroesRandomly(heroControllerInstances);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NextPlayer();
        }
    }

    private void FinishTurn(HeroAction heroAction)
    {
        playersRemainingActions[activePlayer].Remove(heroAction);
        NextPlayer();
        Debug.Log($"{playersRemainingActions[0]}, {playersRemainingActions[1]}");
        if(playersRemainingActions.All(x => x.Count() == 0))
        {
            FinishRound();
        }
    }

    private void FinishRound()
    {
        playersRemainingActions = playersRemainingActions.Select(_ => GenerateActionList()).ToList();
        heroControllerInstances.ForEach(hero => hero.ResetActions());
    }

    private int CalculateNextPlayer()
    {
        return Mathf.Abs(activePlayer - 1);
    }

    private void NextPlayer()
    {
        SetActivePlayer(CalculateNextPlayer());
    }

    private void SetActivePlayer(int playerId)
    {
        Debug.Log($"Currently active player {playerId}");
        activePlayer = playerId;
        players.ForEach(player =>
        {
            player.enabled = player.Id == playerId;
        });
    }

    private List<HeroAction> GenerateActionList()
    {
        var actions = Enumerable.Range(0, MAX_ACTIONS).Select(_ => HeroActionChooser.ChooseRandomAction()).ToList();
        foreach (var action in actions)
        {
            Debug.Log($"{action}");
        }
        return Enumerable.Range(0, MAX_ACTIONS).Select(_ => HeroActionChooser.ChooseRandomAction()).ToList();
    }

}
