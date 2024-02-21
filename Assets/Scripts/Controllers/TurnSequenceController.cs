using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEditor;
using Random = System.Random;

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
    [SerializeField] PlayerInput playerInputPrefab;

    public PlayerId ActivePlayer { get; private set; }

    public Action<List<List<HeroAction>>> onRoundStart;
    public Action<List<List<HeroAction>>> onTurnFinished;
    public Action<LevelFinishedResults> onGameFinished;
    public Action<int, int> OnScoreUpdated;
    public Action<HeroController> onHeroSelected;
    public Action onHeroUnselected;

    public Tuple<int, int> Score;
    //List<HeroListWrapper> units = new();
    private readonly int NUMBER_OF_PLAYERS = Enum.GetNames(typeof(PlayerId)).Length;
    private const int MAX_ACTIONS = 5;

    private List<IPlayer> players = new();
    private List<HeroController> heroControllerInstances = new();
    private List<List<HeroAction>> playersRemainingActions = new();
    private void Awake()
    {
        Score = new Tuple<int, int>(0,0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NextPlayer();
        }
    }

    public void Init(List<HeroController> heroes)
    {
        
        heroes.ForEach(hero =>
        {
            //var instance = Instantiate(hero);
            //hero.ControllingPlayerId = i;
            SpecialAbilityFactory factory = new SpecialAbilityFactory(hero, mapController.GetMapEntity());
            hero.Init(FinishTurn, () => { FinishTurn(HeroAction.Special); }, this.OnDie, factory.BuildSpecialAbility(hero.HeroId));
            hero.onHeroSelected += OnHeroSelectedCallback;
            hero.onHeroUnselected += OnHeroSelectedCallback;
            heroControllerInstances.Add(hero);
        });

        playersRemainingActions.Add(GenerateActionList());
        var player = Instantiate(playerInputPrefab);
        player.gameObject.name = $"Player_0";
        player.Init(mapController, heroControllerInstances, 0);
        players.Add(player);
        
        playersRemainingActions.Add(GenerateActionList());
        //TODO: reconsider using assembly definitions inside of such scripts, it makes readability more difficultt
#if !MOCK_DATA
        var ai = new GameObject().AddComponent<AIAgent>();
        ai.gameObject.name = $"AI";
        ai.Init(mapController, heroControllerInstances, PlayerId.AI);
#else
        var ai = Instantiate(playerInputPrefab);
        ai.gameObject.name = $"Player_0";
        ai.Init(mapController, heroControllerInstances, 0);
#endif

        players.Add(ai);

        onRoundStart?.Invoke(playersRemainingActions);

        Array ids = Enum.GetValues(typeof(PlayerId));
        Random random = new();
        PlayerId id = (PlayerId) ids.GetValue(random.Next(ids.Length));
        SetActivePlayer(id);
    }

    public List<HeroAction> GetPlayerRemainingActions(PlayerId playerId)
    {
        return playersRemainingActions[(int)playerId];
    }


    public void FinishTurn(HeroAction heroAction)
    {
        // First we need to remove the corresponding action and then we MUST call
        // finish round before switching active player because it generates new actions.    
        // If we don't do it first the AI will try to make an action while its action table
        // will be empty which will cause out of bound exception.
        List<HeroController> remainingHeroes = heroControllerInstances.Where(val => val.gameObject.activeSelf).ToList();
        
        //Check if heroes of one players are dead
        bool playerWon = remainingHeroes.All(hero => hero.ControllingPlayerId == PlayerId.Human);
        bool aiWon = remainingHeroes.All(hero => hero.ControllingPlayerId == PlayerId.AI);
        if (playerWon)
        {
            onGameFinished?.Invoke(new LevelFinishedResults { winner = 0} );
            return;
        }
        if (aiWon)
        {
            onGameFinished?.Invoke(new LevelFinishedResults { winner = 1 });
            return;
        }

        if (playersRemainingActions[(int)ActivePlayer].Contains(heroAction))
            playersRemainingActions[(int)ActivePlayer].Remove(heroAction);
        else if(heroAction != HeroAction.Special)
            playersRemainingActions[(int)ActivePlayer].Remove(HeroAction.Special);



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
        var playerScore = mapController.GetObjectivesControlledByPlayers()
            .Count(val => val.entity.occupyingHero.ControllingPlayerId == PlayerId.Human);
        var aiScore = mapController.GetObjectivesControlledByPlayers()
            .Count(val => val.entity.occupyingHero.ControllingPlayerId == PlayerId.AI);
        Score = new Tuple<int, int>(Score.Item1 + playerScore, Score.Item2 + aiScore);
        OnScoreUpdated?.Invoke(Score.Item1, Score.Item2);
        bool playerWon = Score.Item1 >= 6;
        bool aiWon = Score.Item1 >= 6;
        if (playerWon)
        {
            onGameFinished?.Invoke(new LevelFinishedResults { winner = 0} );
            return;
        }
        if (aiWon)
        {
            onGameFinished?.Invoke(new LevelFinishedResults { winner = 1 });
            return;
        }
        playersRemainingActions = playersRemainingActions.Select(_ => GenerateActionList()).ToList();
        heroControllerInstances.ForEach(hero => hero.ResetActions());
        onRoundStart?.Invoke(playersRemainingActions);
    }

    private PlayerId CalculateNextPlayer()
    {
        return (PlayerId) Mathf.Abs((int) ActivePlayer - 1);
    }

    private void NextPlayer()
    {
        SetActivePlayer(CalculateNextPlayer());
    }

    private void SetActivePlayer(PlayerId playerId)
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
    public List<HeroController> HeroPrefabs;
}

public struct LevelFinishedResults
{
    public int winner;

    public bool HasPlayerWon()
    {
        return winner == 0;
    }
}