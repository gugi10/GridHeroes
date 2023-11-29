using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Button openMap;

    SceneLoader.SceneEnum sceneId;

    private void OnEnable()
    {
        openMap.onClick.AddListener(LoadMap);
    }

    private void OnDisable()
    {
        openMap.onClick.RemoveListener(LoadMap);
    }

    public void Initialize(MapConfig mapConfig)
    {
        sceneId = mapConfig.GetSceneId();
        image.sprite = mapConfig.GetImage();
        text.text = mapConfig.GetMapName();
    }

    private void LoadMap()
    {
        SceneLoader.LoadScene(sceneId);
    }
}
