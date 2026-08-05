namespace Common.Save
{
    public class SaveService
    {
        public const string GameStateSaveKey = "game_state_save";

        private readonly GameStateProvider _gameStateProvider;
        private readonly PlayerPrefsSaveStorage _playerPrefsSaveStorage;

        public SaveService(GameStateProvider gameStateProvider, PlayerPrefsSaveStorage playerPrefsSaveStorage)
        {
            _gameStateProvider = gameStateProvider;
            _playerPrefsSaveStorage = playerPrefsSaveStorage;
        }

        public void SaveAll()
        {
            SaveGameState();
        }

        private void SaveGameState()
        {
            var json = ProxySave.ToJson(_gameStateProvider.State);
            _playerPrefsSaveStorage.Save(GameStateSaveKey, json);
        }
    }
}
