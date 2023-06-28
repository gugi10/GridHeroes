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

    private List<HeroController> heroControllerInstances;

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

        heroControllerInstances = Enumerable.Range(0, HEROES_TO_SPAWN).Select(i => {
            var instance = Instantiate(heroPrefab);
            instance.ControllingPlayer = i % 2;
            return instance;
        }).ToList();
    }

    private void Start()
    {
        mapController.SpawnHeroes(heroControllerInstances);
    }

    public void FinishTurn(HeroController hero)
    {

    }
}
