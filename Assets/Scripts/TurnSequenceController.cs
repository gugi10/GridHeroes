using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Linq;

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
    [SerializeField] HeroController heroPrefab;
    [SerializeField] PlayerInput playerInputPrefab;

    public int ActivePlayer { get; private set; }

    public Action<List<List<HeroAction>>> onRoundStart;
    public Action<List<List<HeroAction>>> onTurnFinished;
    public Action<HeroController> onHeroSelected;
    public Action onHeroUnselected;

    private const int NUMBER_OF_PLAYERS = 2;
    private const int HEROES_TO_SPAWN = 6;
    private const int MAX_ACTIONS = 3;

    private List<HeroController> heroControllerInstances = new();
    private List<PlayerInput> players = new();
    private List<List<HeroAction>> playersRemainingActions = new();

    private void Awake()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Units.ForEach(hero =>
            {
                var instance = Instantiate(hero);
                instance.ControllingPlayerId = i;
                instance.Init(FinishTurn);
                instance.onHeroSelected += OnHeroSelectedCallback;
                instance.onHeroUnselected += OnHeroSelectedCallback;
                heroControllerInstances.Add(instance);
            });
        }
    }

    private void Start()
    {
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            playersRemainingActions.Add(GenerateActionList());
            var player = Instantiate(playerInputPrefab);
            player.gameObject.name = $"Player_{i}";
            player.Init(mapController, heroControllerInstances, i);
            players.Add(player);
        }
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
            player.enabled = player.Id == playerId;
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
