using Game.Reps.Services;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Infrastructure.Editor
{
    public static class DebugRepMenu
    {
        [MenuItem("Tools/PushUp/Add Rep _F5")]
        public static void AddRep()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Add Rep works only in play mode");
                return;
            }

            var context = Object.FindFirstObjectByType<SceneContext>();
            if (context == null)
            {
                Debug.LogError("SceneContext not found");
                return;
            }

            context.Container.Resolve<ManualRepCounterService>().AddRep();
        }
    }
}
