using System.Collections.Generic;
using System.IO;
using Game.Installers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;

namespace Infrastructure.Editor
{
    public static class GameSceneCreator
    {
        private const string ScenePath = "Assets/Scenes/Game.unity";

        [InitializeOnLoadMethod]
        private static void CreateIfMissing()
        {
            if (File.Exists(ScenePath.Replace("Assets", Application.dataPath)))
            {
                return;
            }

            EditorApplication.delayCall += Create;
        }

        [MenuItem("Tools/PushUp/Create Game Scene")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.07f, 0.09f, 0.17f);
            camera.orthographic = true;
            cameraObject.AddComponent<AudioListener>();

            var contextObject = new GameObject("SceneContext");
            var sceneContext = contextObject.AddComponent<SceneContext>();
            var installer = contextObject.AddComponent<GameInstaller>();
            sceneContext.Installers = new List<MonoInstaller> { installer };

            EditorSceneManager.SaveScene(scene, ScenePath);
            SetBuildScenes();
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            Debug.Log($"Game scene created at {ScenePath}");
        }

        private static void SetBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }
    }
}
