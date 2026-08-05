using System;

namespace Game.Reps.Services
{
    public class ManualRepCounterService : IRepCounter
    {
        public event Action Rep;

        public void AddRep()
        {
            Rep?.Invoke();
        }
    }
}
