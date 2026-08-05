using System.Collections.Generic;
using UnityEngine;

namespace Game.Monsters
{
    [CreateAssetMenu(fileName = "MonstersConfig", menuName = "Game/MonstersConfig")]
    public class MonstersConfig : ScriptableObject
    {
        public List<MonsterTierData> Tiers = ProxyDefaultMonsters.CreateTiers();
        public List<string> Prefixes = ProxyDefaultMonsters.CreatePrefixes();
        public List<MonsterData> Bosses = ProxyDefaultMonsters.CreateBosses();
    }
}
