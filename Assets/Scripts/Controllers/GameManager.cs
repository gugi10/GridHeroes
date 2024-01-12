using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TurnSequenceController turnSequenceController;
    [SerializeField] private Deployment deployment;
    [SerializeField] private MapController map;
#if MOCK_DATA
    [SerializeField] List<HeroListWrapper> heroes = new();
#else
    List<HeroListWrapper> heroes = new();
#endif 

    private void Start()
    {
        ResetLevel(false);
    }

    private void OnDeploymentFinished(List<HeroController> spawnedHeroes)
    {
        turnSequenceController.Init(spawnedHeroes);
        turnSequenceController.onGameFinished = OnGameFinished;
    }

    private void OnGameFinished(LevelFinishedResults results)
    {
        ScreensController.Instance.OpenPopup(PopupIdentifiers.EndGamePopup, new VictoryPopup.Payload(results.HasPlayerWon()));
    }

    private void ResetLevel(bool playerWon)
    {
#if !MOCK_DATA
        heroes[0].HeroPrefabs = GameSession.Instance.GetService<HeroService>().GetPlayerLineUp();
#endif
        deployment.Init(map, heroes, OnDeploymentFinished);
    }
}