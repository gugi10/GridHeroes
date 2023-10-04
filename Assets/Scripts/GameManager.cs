using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TurnSequenceController turnSequenceController;
    [SerializeField] private Deployment deployment;
    [SerializeField] private MapController map;
    [SerializeField] List<HeroListWrapper> heroes = new();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        deployment.Init(map, heroes, OnDeploymentFinished);
        //turnSequenceController.Init(units);
    }

    private void OnDeploymentFinished(List<HeroController> spawnedHeroes)
    {
        turnSequenceController.Init(spawnedHeroes);
    }

}
