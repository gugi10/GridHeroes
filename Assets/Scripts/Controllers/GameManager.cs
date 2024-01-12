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
    [SerializeField] List<HeroListWrapper> heroes = new();
    int maxScenes = 1;

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
        heroes[0].HeroPrefabs = GameSession.Instance.GetService<HeroService>().GetPlayerLineUp();
        deployment.Init(map, heroes, OnDeploymentFinished);
    }
}
