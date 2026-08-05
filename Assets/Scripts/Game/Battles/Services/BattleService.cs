using System;
using Game.Levels;
using UnityEngine;

namespace Game.Battles.Services
{
    public class BattleService
    {
        public event Action HpChanged;
        public event Action Defeated;

        public LevelData Level { get; private set; }
        public int RepsDone { get; private set; }
        public int PendingDamage { get; private set; }
        public bool IsActive { get; private set; }

        public int RemainingHp
        {
            get
            {
                if (Level == null)
                {
                    return 0;
                }

                return Level.Hp - RepsDone;
            }
        }

        public void Begin(LevelData level, int repsDone)
        {
            Level = level;
            RepsDone = Mathf.Clamp(repsDone, 0, level.Hp);
            PendingDamage = 0;
            IsActive = RepsDone < level.Hp;
            HpChanged?.Invoke();
        }

        public void BeginPendingRep()
        {
            if (!IsActive || PendingDamage > 0)
            {
                return;
            }

            PendingDamage = 1;
            HpChanged?.Invoke();
        }

        public void CancelPendingRep()
        {
            if (PendingDamage == 0)
            {
                return;
            }

            PendingDamage = 0;
            HpChanged?.Invoke();
        }

        public void ApplyRep()
        {
            if (!IsActive)
            {
                return;
            }

            PendingDamage = 0;
            RepsDone += 1;
            HpChanged?.Invoke();

            if (RemainingHp > 0)
            {
                return;
            }

            IsActive = false;
            Defeated?.Invoke();
        }

        public void Stop()
        {
            PendingDamage = 0;
            IsActive = false;
        }
    }
}
