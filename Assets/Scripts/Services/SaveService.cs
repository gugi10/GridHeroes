using System.IO;
using UnityEngine;

namespace Services
{
    public class SaveService : IService
    {
        private HeroService _heroService;
        
        public void SaveAllData()
        {
            _heroService = GameSession.Instance.GetService<HeroService>();
            string json = JsonUtility.ToJson(_heroService.GetHeroToSaveModel());
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
            string loadPlayerData = File.ReadAllText(saveFilePath);
            var playerData = JsonUtility.FromJson<HeroSaveModel>(loadPlayerData);
            _heroService.LoadHeroSaveData(playerData); 
        }
    }
}