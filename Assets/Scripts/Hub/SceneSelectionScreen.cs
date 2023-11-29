using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectionScreen : ScreenBase
{
    public override ScreenIdentifiers Identifier() => ScreenIdentifiers.LevelSelect;

    [SerializeField] SceneLoader.SceneEnum scene = SceneLoader.SceneEnum.Map01;
    [SerializeField] Button loadMapButton;

    private void OnEnable()
    {
        loadMapButton.onClick.AddListener(LoadScene);
    }

    private void OnDisable()
    {
        loadMapButton.onClick.RemoveListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneLoader.LoadScene(SceneLoader.SceneEnum.Map01);
    }
}
