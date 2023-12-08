using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ScreenIdentifiers
{
    LevelSelect,
    HeroSelect,
    VictoryScreen
}

public abstract class ScreenBase : MonoBehaviour
{
    public abstract ScreenIdentifiers Identifier();
}

public class ScreensController : Singleton<ScreensController>
{
    [SerializeField] List<ScreenBase> screens;
    [SerializeField] ScreenBase startingScreen;
    List<ScreenBase> spawnedScreens = new();
    private ScreenBase currentScreen;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void OpenScreen(ScreenIdentifiers id)
    {
        var screenToOpen = screens.FirstOrDefault(screen => screen.Identifier() == id);

        if (screenToOpen == null)
        {
            Debug.LogError($"No screen with such ID {id}");
            return;
        }

        if(currentScreen == null)
        {
            SpawnScreen(screenToOpen);
            return;
        }

        if (currentScreen.Identifier() == id)
        {
            Debug.Log($"Screen {id} is currently opeend");
            return;
        }

        var foundScreen = spawnedScreens.FirstOrDefault(x => x.Identifier() == id);

        if(foundScreen)
        {
            foundScreen.gameObject.SetActive(true);
            currentScreen.gameObject.SetActive(false);
            currentScreen = foundScreen;
            return;
        }

        currentScreen.gameObject.SetActive(false);
        SpawnScreen(screenToOpen);
    }

    private void SpawnScreen(ScreenBase screenToSpawn)
    {
        var spawnedScreen = Instantiate(screenToSpawn);
        spawnedScreens.Add(spawnedScreen);
        currentScreen = spawnedScreen;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            OpenScreen(ScreenIdentifiers.LevelSelect);
        if (Input.GetKeyDown(KeyCode.B))
            OpenScreen(ScreenIdentifiers.HeroSelect);
    }
}
