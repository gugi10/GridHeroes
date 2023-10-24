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

    private void Start()
    {
        deployment.Init(map, heroes, OnDeploymentFinished);
    }

    private void OnEnable()
    {
        turnSequenceController.onGameFinished += ResetLevel;
    }

    private void OnDisable()
    {
        turnSequenceController.onGameFinished -= ResetLevel;
    }

    private void OnDeploymentFinished(List<HeroController> spawnedHeroes)
    {
        turnSequenceController.Init(spawnedHeroes);
    }

    private void ResetLevel(bool playerWon)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        deployment.Init(map, heroes, OnDeploymentFinished);

    }
}
