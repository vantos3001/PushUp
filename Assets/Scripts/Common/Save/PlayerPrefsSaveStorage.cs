using UnityEngine;

namespace Common.Save
{
    public class PlayerPrefsSaveStorage
    {
        public bool Exists(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public string Load(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void Save(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
    }
}
