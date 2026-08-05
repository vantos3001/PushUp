using Game.Sounds.Services;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Infrastructure.Editor
{
    public static class DebugSoundMenu
    {
        private static SoundService _soundService;
        private static double _startTime;
        private static int _step;

        [MenuItem("Tools/PushUp/Test Sounds")]
        public static void TestSounds()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Test Sounds works only in play mode");
                return;
            }

            var context = Object.FindFirstObjectByType<SceneContext>();
            if (context == null)
            {
                Debug.LogError("SceneContext not found");
                return;
            }

            _soundService = context.Container.Resolve<SoundService>();
            _startTime = EditorApplication.timeSinceStartup;
            _step = 0;
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            var elapsed = EditorApplication.timeSinceStartup - _startTime;

            if (_step == 0)
            {
                _soundService.PlayDownSound();
                Debug.Log("Звук 1/3: успешно лёг");
                _step = 1;
                return;
            }

            if (_step == 1 && elapsed >= 0.9)
            {
                _soundService.PlayRepSound();
                Debug.Log("Звук 2/3: успешно встал");
                _step = 2;
                return;
            }

            if (_step == 2 && elapsed >= 1.8)
            {
                _soundService.PlayCancelSound();
                Debug.Log("Звук 3/3: отмена урона");
                _step = 3;
                EditorApplication.update -= OnUpdate;
            }
        }
    }
}
