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
    private const int MAX_ACTIONS = 5;



    private List<HeroController> heroControllerInstances;
    private List<PlayerInput> players = new List<PlayerInput>();
    private int activePlayer;
    private List<uint> playersRemainingActions = new List<uint> { MAX_ACTIONS, MAX_ACTIONS };

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
        playersRemainingActions[activePlayer] -= 1;
        NextPlayer();
        Debug.Log($"{playersRemainingActions[0]}, {playersRemainingActions[1]}");
        if(playersRemainingActions.All(x => x == 0))
        {
            FinishRound();
        }
    }

    private void FinishRound()
    {
        playersRemainingActions.ForEach(x => x = MAX_ACTIONS);
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

}
