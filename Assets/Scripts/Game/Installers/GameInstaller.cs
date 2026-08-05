using System;
using Common.Save;
using Common.UI.Services;
using Game.Battles.Services;
using Game.Campaigns;
using Game.Campaigns.Services;
using Game.Exercises;
using Game.Exercises.Services;
using Game.Levels.Services;
using Game.Monsters;
using Game.Monsters.Services;
using Game.Poses.Services;
using Game.Reps;
using Game.Reps.Services;
using Game.Services;
using Game.Sounds;
using Game.Sounds.Services;
using Game.Stats.Services;
using Game.UI.Services;
using Game.Wallets.Services;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigs();
            BindSaveServices();
            BindLevelServices();
            BindRepServices();
            BindBattleServices();
            BindUIServices();
            BindEntrypoint();
        }

        private void BindConfigs()
        {
            Container.Bind<MonstersConfig>().FromInstance(LoadConfig<MonstersConfig>("Configs/MonstersConfig")).AsSingle();
            Container.Bind<CampaignConfig>().FromInstance(LoadConfig<CampaignConfig>("Configs/CampaignConfig")).AsSingle();
            Container.Bind<TrackingConfig>().FromInstance(LoadConfig<TrackingConfig>("Configs/TrackingConfig")).AsSingle();
            Container.Bind<ExercisesConfig>().FromInstance(LoadConfig<ExercisesConfig>("Configs/ExercisesConfig")).AsSingle();
            Container.Bind<SoundConfig>().FromInstance(LoadConfig<SoundConfig>("Configs/SoundConfig")).AsSingle();
        }

        private void BindSaveServices()
        {
            Container.Bind<GameStateProvider>().AsSingle();
            Container.Bind<PlayerPrefsSaveStorage>().AsSingle();
            Container.Bind<InitGameStateService>().AsSingle();
            Container.Bind<SaveService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AutoSaveService>().AsSingle();
        }

        private void BindLevelServices()
        {
            Container.Bind<ExerciseInfoService>().AsSingle();
            Container.Bind<MonsterNameService>().AsSingle();
            Container.Bind<LevelGenerationService>().AsSingle();
        }

        private void BindRepServices()
        {
            Container.Bind<ManualRepCounterService>().AsSingle();
            Container.Bind(typeof(CameraRepCounterService), typeof(ITickable), typeof(IDisposable))
                .To<CameraRepCounterService>().AsSingle();
            Container.Bind(typeof(PoseEstimationService), typeof(ITickable), typeof(IDisposable))
                .To<PoseEstimationService>().AsSingle();
            Container.Bind(typeof(PoseRepCounterService), typeof(IDisposable))
                .To<PoseRepCounterService>().AsSingle();
            Container.Bind(typeof(CompositeRepCounterService), typeof(IRepCounter), typeof(IDisposable))
                .To<CompositeRepCounterService>().AsSingle();
        }

        private void BindBattleServices()
        {
            Container.Bind<BattleService>().AsSingle();
            Container.Bind<StatsService>().AsSingle();
            Container.Bind<WalletService>().AsSingle();
            Container.BindInterfacesAndSelfTo<CampaignService>().AsSingle();
            Container.BindInterfacesAndSelfTo<SoundService>().AsSingle();
        }

        private void BindUIServices()
        {
            Container.Bind<UIWidgetFactory>().AsSingle();
            Container.Bind<UIRootService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIMenuService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIBattleService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIStatsService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UICampaignCompleteService>().AsSingle();
            Container.Bind<UIToastService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIFlowService>().AsSingle();
        }

        private void BindEntrypoint()
        {
            Container.BindInterfacesAndSelfTo<GameEntrypoint>().AsSingle();
        }

        private static T LoadConfig<T>(string path) where T : ScriptableObject
        {
            var config = Resources.Load<T>(path);
            if (config != null)
            {
                return config;
            }

            Debug.LogError($"Config not found at Resources/{path}, using defaults");

            return ScriptableObject.CreateInstance<T>();
        }
    }
}
