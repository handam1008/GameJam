using Systems;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Pattern data", menuName = "SO/EnemyPatternData", order = 0)]
    public class EnemyAttackPatternSO : ScriptableObject
    {
        public PoolItemSO bullet;
        public float degree;
        public int bulletCount;
        public float cooldown;
    }
}