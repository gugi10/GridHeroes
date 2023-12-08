using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionScreen : ScreenBase
{
    [SerializeField] private Button openLevelScreen;
    [SerializeField] private SelectedHeroesSection selectedHeroesSection;
    public override ScreenIdentifiers Identifier() => ScreenIdentifiers.HeroSelect;

    private void OnEnable()
    {
        openLevelScreen.onClick.AddListener(OpenLevelScreen);
    }

    private void OnDisable()
    {
        openLevelScreen.onClick.RemoveListener(OpenLevelScreen);
    }

    private void OpenLevelScreen()
    {
        if(selectedHeroesSection.GetSelectedHeroesCount() > 0)
            ScreensController.Instance.OpenScreen(ScreenIdentifiers.LevelSelect);
    }
}
