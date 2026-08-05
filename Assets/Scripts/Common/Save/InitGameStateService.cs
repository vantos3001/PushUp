namespace Common.Save
{
    public class InitGameStateService
    {
        private readonly GameStateProvider _gameStateProvider;
        private readonly PlayerPrefsSaveStorage _playerPrefsSaveStorage;

        public InitGameStateService(GameStateProvider gameStateProvider, PlayerPrefsSaveStorage playerPrefsSaveStorage)
        {
            _gameStateProvider = gameStateProvider;
            _playerPrefsSaveStorage = playerPrefsSaveStorage;
        }

        public void Init()
        {
            var state = LoadGameState();
            _gameStateProvider.SetState(state);
        }

        private GameState LoadGameState()
        {
            if (!_playerPrefsSaveStorage.Exists(SaveService.GameStateSaveKey))
            {
                return new GameState();
            }

            var json = _playerPrefsSaveStorage.Load(SaveService.GameStateSaveKey);
            if (string.IsNullOrEmpty(json))
            {
                return new GameState();
            }

            var state = ProxySave.FromJson<GameState>(json);
            if (state == null)
            {
                return new GameState();
            }

            return state;
        }
    }
}
