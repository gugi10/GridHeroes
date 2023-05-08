using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnSequenceController : MonoBehaviour
{
    [SerializeField] MapController mapController;
    public static TurnSequenceController Instance { get; private set; }
    public List<HeroController> heroControllers { get; private set; }
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
        heroControllers = FindObjectsOfType<HeroController>().ToList();
    }

    private void Start()
    {
        mapController.SpawnHeroes(heroControllers);
    }
   
    public void FinishTurn(HeroController hero)
    {

    }
}
