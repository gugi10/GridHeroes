using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Services
{
    public class SaveService : IService
    {
        private HeroService _heroService;
        private MapService _mapService;
        
        public void SaveAllData()
        {
            _heroService = GameSession.Instance.GetService<HeroService>();
            _mapService = GameSession.Instance.GetService<MapService>();
            CompleteSaveModel saveModel =
                new CompleteSaveModel(_mapService.GetPlayerMapSaveData(),_heroService.GetHeroToSaveModel() );
            string json = JsonConvert.SerializeObject(saveModel);
            Debug.Log($"Try to save {json}");
            var saveFilePath = Application.persistentDataPath + "/PlayerData.json";
            Debug.Log($"Json save {json}, path {saveFilePath}");
            File.WriteAllText(saveFilePath, json);
        }

        public void LoadDataAndUpdateservices()
        {
            var saveFilePath = Application.persistentDataPath + "/PlayerData.json";
            
            if (!File.Exists(saveFilePath))
                return;
            
            _heroService = GameSession.Instance.GetService<HeroService>();
            _mapService = GameSession.Instance.GetService<MapService>();
            string loadPlayerData = File.ReadAllText(saveFilePath);
            var playerData = JsonConvert.DeserializeObject<CompleteSaveModel>(loadPlayerData);
            //var playerData = JsonUtility.FromJson<CompleteSaveModel>(loadPlayerData);
            if (playerData == null)
            {
                Debug.Log($"Could not parse save data");
                return;
            }

            _heroService.LoadHeroSaveData(playerData.Heroes); 
            _mapService.LoadMapSavedData(playerData.Bioms);
                //_mapService.
        }
    }

    [System.Serializable]
    public class CompleteSaveModel
    {
        public List<BiomData> Bioms;
        public HeroSaveModel Heroes;

        public CompleteSaveModel(List<BiomData> bioms, HeroSaveModel heroes)
        {
            Bioms = bioms;
            Heroes = heroes;
        }
    }
}