using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ScreenIdentifiers
{
    LevelSelect,
    HeroSelect
}

public enum PopupIdentifiers
{
    EndGamePopup
}

public abstract class ScreenBase : MonoBehaviour
{
    public abstract ScreenIdentifiers Identifier();
}

public abstract class PopupBase : MonoBehaviour
{
    public abstract PopupIdentifiers Identifier();

    public abstract void OnEnter(object data);
}

public class ScreensController : Singleton<ScreensController>
{
    [SerializeField] List<ScreenBase> screens;
    [SerializeField] List<PopupBase> popups;
    [SerializeField] ScreenBase startingScreen;
    List<ScreenBase> spawnedScreens = new();
    List<PopupBase> spawnedPopups = new();
    private ScreenBase currentScreen;
    private PopupBase activePopup;

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

    //TODO: Popups should behave as queue, not important rn
    public void OpenPopup(PopupIdentifiers id, object payload = null)
    {
        var popupToOpen = popups.FirstOrDefault(popup => popup.Identifier() == id);

        if (popupToOpen == null)
        {
            Debug.LogError($"No Popup with such ID {id}");
            return;
        }

        if (activePopup == null)
        {
            SpawnPopup(popupToOpen);
            return;
        }

        if (activePopup.Identifier() == id)
        {
            Debug.Log($"Popup {id} is currently opeend");
            return;
        }

        var foundPopup = spawnedPopups.FirstOrDefault(x => x.Identifier() == id);

        if (foundPopup)
        {
            foundPopup.gameObject.SetActive(true);
            activePopup.gameObject.SetActive(false);
            activePopup = foundPopup;
            return;
        }

        currentScreen.gameObject.SetActive(false);
        SpawnPopup(popupToOpen, payload);
    }

    private void SpawnScreen(ScreenBase screenToSpawn)
    {
        var spawnedScreen = Instantiate(screenToSpawn);
        spawnedScreens.Add(spawnedScreen);
        currentScreen = spawnedScreen;
    }

    private void SpawnPopup(PopupBase popupToSpawn, object payload = null)
    {
        var spawnedPopup = Instantiate(popupToSpawn);
        spawnedPopup.GetComponent<Canvas>().sortingOrder = 3;
        spawnedPopup.OnEnter(payload);
        spawnedPopups.Add(spawnedPopup);
        activePopup = spawnedPopup;
    }
}
