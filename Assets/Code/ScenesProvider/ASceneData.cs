using UnityEngine;

namespace MyProject.ReactionBurst.Shared.ScenesProvider
{
    public class ASceneData
    {
        private string _jsonData;

        public void SetData(object data)
        {
            _jsonData = JsonUtility.ToJson(data, false);
            PlayerPrefs.SetString($"ASceneData_{data.GetType().Name}", _jsonData);
        }

        public T GetData<T>() where T : class
        {
            var key = $"ASceneData_{typeof(T).Name}";
            if (string.IsNullOrEmpty(_jsonData) && PlayerPrefs.HasKey(key))
            {
                _jsonData = PlayerPrefs.GetString(key, _jsonData);
                PlayerPrefs.DeleteKey(key);
            }

            if (string.IsNullOrEmpty(_jsonData))
                return null;
            return JsonUtility.FromJson<T>(_jsonData);
        }
    }
}