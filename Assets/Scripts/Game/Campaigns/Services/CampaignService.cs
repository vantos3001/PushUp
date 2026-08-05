using System;
using Common.Save;
using Game.Battles.Services;
using Game.Levels;
using Game.Levels.Services;
using Game.Poses.Services;
using Game.Reps;
using Game.Stats.Services;
using Game.Wallets.Services;
using UnityEngine;

namespace Game.Campaigns.Services
{
    public class CampaignService : IDisposable
    {
        private readonly CampaignConfig _campaignConfig;
        private readonly GameStateProvider _gameStateProvider;
        private readonly SaveService _saveService;
        private readonly LevelGenerationService _levelGenerationService;
        private readonly BattleService _battleService;
        private readonly IRepCounter _repCounter;
        private readonly PoseRepCounterService _poseRepCounterService;
        private readonly StatsService _statsService;
        private readonly WalletService _walletService;

        public event Action LevelCompleted;
        public event Action CampaignCompleted;

        public CampaignTrackData ActiveTrack { get; private set; }
        public bool IsRunning { get; private set; }
        public int LastGoldReward { get; private set; }

        public CampaignService(
            CampaignConfig campaignConfig,
            GameStateProvider gameStateProvider,
            SaveService saveService,
            LevelGenerationService levelGenerationService,
            BattleService battleService,
            IRepCounter repCounter,
            PoseRepCounterService poseRepCounterService,
            StatsService statsService,
            WalletService walletService)
        {
            _campaignConfig = campaignConfig;
            _gameStateProvider = gameStateProvider;
            _saveService = saveService;
            _levelGenerationService = levelGenerationService;
            _battleService = battleService;
            _repCounter = repCounter;
            _poseRepCounterService = poseRepCounterService;
            _statsService = statsService;
            _walletService = walletService;
        }

        public void Init()
        {
            _repCounter.Rep += OnRep;
            _poseRepCounterService.DepthReached += OnDepthReached;
            _poseRepCounterService.CycleCanceled += OnCycleCanceled;
            _battleService.Defeated += OnDefeated;
        }

        public void Begin(string trackId)
        {
            var track = GetTrack(trackId);
            if (track == null)
            {
                Debug.LogError($"Campaign track '{trackId}' is not defined in CampaignConfig");
                return;
            }

            ActiveTrack = track;
            IsRunning = true;
            StartCurrentLevel();
        }

        public void StartCurrentLevel()
        {
            if (ActiveTrack == null)
            {
                Debug.LogError("StartCurrentLevel called without active track");
                return;
            }

            var trackState = GetTrackState(ActiveTrack.Id);
            var level = _levelGenerationService.GetLevel(trackState.LevelIndex, ActiveTrack.ExerciseId);
            _battleService.Begin(level, trackState.RepsDoneInLevel);
        }

        public void GiveUp()
        {
            IsRunning = false;
            _battleService.Stop();

            if (ActiveTrack != null)
            {
                GetTrackState(ActiveTrack.Id).RepsDoneInLevel = 0;
            }

            _saveService.SaveAll();
        }

        public LevelData GetCurrentLevel(string trackId)
        {
            var track = GetTrack(trackId);
            if (track == null)
            {
                return null;
            }

            var trackState = GetTrackState(trackId);

            return _levelGenerationService.GetLevel(trackState.LevelIndex, track.ExerciseId);
        }

        public CampaignTrackState GetTrackState(string trackId)
        {
            var tracks = _gameStateProvider.State.Tracks;

            for (var i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].Id != trackId)
                {
                    continue;
                }

                return tracks[i];
            }

            var trackState = new CampaignTrackState { Id = trackId };
            tracks.Add(trackState);

            return trackState;
        }

        private CampaignTrackData GetTrack(string trackId)
        {
            var tracks = _campaignConfig.Tracks;

            for (var i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].Id != trackId)
                {
                    continue;
                }

                return tracks[i];
            }

            return null;
        }

        private void OnDepthReached()
        {
            if (!IsRunning)
            {
                return;
            }

            _battleService.BeginPendingRep();
        }

        private void OnCycleCanceled()
        {
            if (!IsRunning)
            {
                return;
            }

            _battleService.CancelPendingRep();
        }

        private void OnRep()
        {
            if (!IsRunning)
            {
                return;
            }

            if (!_battleService.IsActive)
            {
                return;
            }

            _gameStateProvider.State.TotalReps += 1;
            _statsService.AddReps(_battleService.Level.ExerciseId, 1);
            _battleService.ApplyRep();

            if (_battleService.IsActive)
            {
                GetTrackState(ActiveTrack.Id).RepsDoneInLevel = _battleService.RepsDone;
            }

            _saveService.SaveAll();
        }

        private void OnDefeated()
        {
            if (!IsRunning)
            {
                return;
            }

            LastGoldReward = GetGoldReward(_battleService.Level);
            _walletService.Add(LastGoldReward);

            _gameStateProvider.State.TotalMonstersKilled += 1;

            var trackState = GetTrackState(ActiveTrack.Id);
            trackState.RepsDoneInLevel = 0;
            trackState.LevelIndex += 1;

            var campaignCompleted = trackState.LevelIndex >= _levelGenerationService.TotalLevels;
            if (campaignCompleted)
            {
                trackState.LevelIndex = 0;
                trackState.Prestige += 1;
            }

            _saveService.SaveAll();

            if (campaignCompleted)
            {
                CampaignCompleted?.Invoke();
                return;
            }

            LevelCompleted?.Invoke();
        }

        private int GetGoldReward(LevelData level)
        {
            if (level == null || ActiveTrack == null)
            {
                return 0;
            }

            var gold = Mathf.CeilToInt(level.Hp * ActiveTrack.GoldShare);
            if (level.IsBoss)
            {
                gold *= _campaignConfig.BossRewardMultiplier;
            }

            return Mathf.Max(1, gold);
        }

        public void Dispose()
        {
            _repCounter.Rep -= OnRep;
            _poseRepCounterService.DepthReached -= OnDepthReached;
            _poseRepCounterService.CycleCanceled -= OnCycleCanceled;
            _battleService.Defeated -= OnDefeated;
        }
    }
}
