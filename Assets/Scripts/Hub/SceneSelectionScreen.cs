using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectionScreen : ScreenBase
{
    public override ScreenIdentifiers Identifier() => ScreenIdentifiers.LevelSelect;

    [SerializeField] private MapSlot mapPrefab;
    [SerializeField] private MapsConfig mapsConfigObj;
    [SerializeField] private Transform mapsParent;
    [SerializeField] private Button heroSelectionScreen;

    private void Start()
    {
        //TODO: Later implement more than one biom spawn
        foreach(var map in GameSession.Instance.GetService<MapService>().GetMapsFromBiom(0))
        {
            var spawnedMap = Instantiate(mapPrefab, mapsParent);
            spawnedMap.Initialize(map);
        }
    }

    private void OnEnable()
    {
        heroSelectionScreen.onClick.AddListener(OpenHeroSelection);
    }

    private void OnDisable()
    {
        heroSelectionScreen.onClick.RemoveListener(OpenHeroSelection);
    }

    private void OpenHeroSelection()
    {
        ScreensController.Instance.OpenScreen(ScreenIdentifiers.HeroSelect);
    }
}
