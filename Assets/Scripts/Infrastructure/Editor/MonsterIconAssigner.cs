using Game.Monsters;
using UnityEditor;
using UnityEngine;

namespace Infrastructure.Editor
{
    public static class MonsterIconAssigner
    {
        private const string ConfigPath = "Assets/Data/Resources/Configs/MonstersConfig.asset";
        private const string IconsFolder = "Assets/Art/Monsters";

        [InitializeOnLoadMethod]
        private static void ScheduleAssign()
        {
            EditorApplication.delayCall += AssignSilent;
        }

        [MenuItem("Tools/PushUp/Assign Monster Icons")]
        public static void Assign()
        {
            var config = AssetDatabase.LoadAssetAtPath<MonstersConfig>(ConfigPath);
            if (config == null)
            {
                Debug.LogError($"MonstersConfig not found at {ConfigPath}, run Tools/PushUp/Create Configs first");
                return;
            }

            var assigned = AssignAll(config);
            if (assigned == 0)
            {
                Debug.Log("Monster icons: nothing new to assign");
                return;
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            Debug.Log($"Monster icons assigned: {assigned}");
        }

        private static void AssignSilent()
        {
            var config = AssetDatabase.LoadAssetAtPath<MonstersConfig>(ConfigPath);
            if (config == null)
            {
                return;
            }

            var assigned = AssignAll(config);
            if (assigned == 0)
            {
                return;
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            Debug.Log($"Monster icons assigned: {assigned}");
        }

        private static int AssignAll(MonstersConfig config)
        {
            var assigned = 0;

            for (var tier = 0; tier < config.Tiers.Count; tier++)
            {
                var monsters = config.Tiers[tier].Monsters;
                for (var i = 0; i < monsters.Count; i++)
                {
                    if (TryAssign(monsters[i], $"{IconsFolder}/tier{tier}_{i:D2}.png"))
                    {
                        assigned += 1;
                    }
                }
            }

            for (var i = 0; i < config.Bosses.Count; i++)
            {
                if (TryAssign(config.Bosses[i], $"{IconsFolder}/boss_{i:D2}.png"))
                {
                    assigned += 1;
                }
            }

            return assigned;
        }

        private static bool TryAssign(MonsterData monster, string path)
        {
            if (monster.Icon != null)
            {
                return false;
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                return false;
            }

            monster.Icon = sprite;

            return true;
        }
    }
}
