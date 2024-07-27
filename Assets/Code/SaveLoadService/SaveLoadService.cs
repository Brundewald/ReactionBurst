using UnityEngine;

namespace MyProject.ReactionBurst.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string AppKey = "ReactionBurst/";

        public void Save() => PlayerPrefs.Save();

        public void ClearProgress() => PlayerPrefs.DeleteAll();

        public void SetData(string path, object target)
        {
            switch (target)
            {
                case int value:
                    PlayerPrefs.SetInt(AppKey + path, value);
                    break;
                case float value:
                    PlayerPrefs.SetFloat(AppKey + path, value);
                    break;
                case string value:
                    PlayerPrefs.SetString(AppKey + path, value);
                    break;
                default:
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(target);
                    PlayerPrefs.SetString(AppKey + path, json);
                    break;
            }
        }

        public T GetData<T>(string path)
        {
            var type = typeof(T);
            
            switch (type)
            {
                case not null when type == typeof(int):
                    return (T)(object) PlayerPrefs.GetInt(AppKey + path, 0);
                case not null when type == typeof(float):
                    return (T)(object) PlayerPrefs.GetFloat(AppKey + path, 0);
                case not null when type == typeof(string):
                    return (T)(object) PlayerPrefs.GetString(AppKey + path, string.Empty);
                case not null when type  == typeof(bool):
                    var value = PlayerPrefs.GetString(AppKey + path, bool.FalseString);
                    return (T)(object) bool.Parse(value);
                default:
                    var json = PlayerPrefs.GetString(AppKey + path, path);
                    return string.IsNullOrEmpty(json) ? default : JsonUtility.FromJson<T>(json);
            }
        }
    }   
}