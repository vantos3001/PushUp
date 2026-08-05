using System.IO;
using Game.Campaigns;
using Game.Exercises;
using Game.Monsters;
using Game.Reps;
using Game.Sounds;
using UnityEditor;
using UnityEngine;

namespace Infrastructure.Editor
{
    public static class ConfigsCreator
    {
        private const string FolderPath = "Assets/Data/Resources/Configs";
        private const string MonstersConfigPath = FolderPath + "/MonstersConfig.asset";
        private const string CampaignConfigPath = FolderPath + "/CampaignConfig.asset";
        private const string TrackingConfigPath = FolderPath + "/TrackingConfig.asset";
        private const string ExercisesConfigPath = FolderPath + "/ExercisesConfig.asset";
        private const string SoundConfigPath = FolderPath + "/SoundConfig.asset";
        private const string ObsoleteTrainingConfigPath = FolderPath + "/TrainingConfig.asset";

        [InitializeOnLoadMethod]
        private static void ScheduleCreate()
        {
            EditorApplication.delayCall += Create;
        }

        [MenuItem("Tools/PushUp/Create Configs")]
        public static void Create()
        {
            Directory.CreateDirectory(FolderPath);
            AssetDatabase.Refresh();
            EnsureAsset<MonstersConfig>(MonstersConfigPath);
            EnsureAsset<CampaignConfig>(CampaignConfigPath);
            EnsureAsset<TrackingConfig>(TrackingConfigPath);
            EnsureAsset<ExercisesConfig>(ExercisesConfigPath);
            EnsureAsset<SoundConfig>(SoundConfigPath);
            AssetDatabase.DeleteAsset(ObsoleteTrainingConfigPath);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureAsset<T>(string path) where T : ScriptableObject
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) != null)
            {
                return;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
            Debug.Log($"Created config at {path}");
        }
    }
}
