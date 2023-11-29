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
    int sceneIndex;
    int maxScenes = 1;

    private void Start()
    {
        ResetLevel(false);
    }

    private void OnDeploymentFinished(List<HeroController> spawnedHeroes)
    {
        turnSequenceController.Init(spawnedHeroes);
    }

    private void ResetLevel(bool playerWon)
    {
        deployment.Init(map, heroes, OnDeploymentFinished);
        if (sceneIndex >= maxScenes)
        {
            sceneIndex = 0;
        }
    }
}
