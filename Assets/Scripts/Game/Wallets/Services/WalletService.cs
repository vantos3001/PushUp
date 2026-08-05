using System;
using Common.Save;

namespace Game.Wallets.Services
{
    public class WalletService
    {
        private readonly GameStateProvider _gameStateProvider;

        public event Action GoldChanged;

        public int Gold => _gameStateProvider.State.Gold;

        public WalletService(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _gameStateProvider.State.Gold += amount;
            GoldChanged?.Invoke();
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (_gameStateProvider.State.Gold < amount)
            {
                return false;
            }

            _gameStateProvider.State.Gold -= amount;
            GoldChanged?.Invoke();

            return true;
        }
    }
}
