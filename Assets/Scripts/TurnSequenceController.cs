using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnSequenceController : MonoBehaviour
{
    private const int NUMBER_OF_PLAYERS = 2;

    [SerializeField] Player player;
    [SerializeField] MapController mapController;

    public static TurnSequenceController Instance { get; private set; }
    public List<HeroController> heroControllers;
    private List<Player> players = new List<Player>();
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
    }

    private void Start()
    {
        for(int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            Player spawnedPlayer = Instantiate(player);
            players.Add(spawnedPlayer);
        }

        var halfOfList = heroControllers.Count / 2;

        for (int i = 0; i < halfOfList; i++)
        {
            heroControllers[i].ControllingPlayer = 0;
        }
        for (int i = halfOfList; i < heroControllers.Count; i++)
        {
            heroControllers[i].ControllingPlayer = 1;
        }

        //mapController.SpawnHeroes(heroControllers);
    }
   
    public void FinishTurn(HeroController hero)
    {

    }
}
