namespace Common.Save
{
    public class GameStateProvider
    {
        public GameState State { get; private set; }

        public void SetState(GameState state)
        {
            State = state;
        }
    }
}
