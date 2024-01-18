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

    private string sceneId;
    private string biomName;

    private void OnEnable()
    {
        openMap.onClick.AddListener(LoadMap);
    }

    private void OnDisable()
    {
        openMap.onClick.RemoveListener(LoadMap);
    }

    public void Initialize(MapData mapData)
    {
        var mapConfig = GameSession.Instance.GetConfig<MapsConfig>().GetMapsFromBiom(0).Find(val => val.GetMapName() == mapData.MapName);
        sceneId = mapData.MapName;
        image.sprite = mapConfig.GetImage();
        text.text = mapConfig.GetMapName();
    }

    private void LoadMap()
    {
        GameSession.Instance.GetService<MapService>().LoadMap(sceneId);
        //SceneLoader.LoadScene(sceneId);
    }
}
