using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 바닥에서 주운 공격 데이터.
    /// </summary>
    public class AttackStack : AbstractStack
    {
        private readonly float _damage;

        public AttackStack(float damage)
        {
            _damage = damage;
        }

        public override string DisplayName => $"공격 {_damage}";

        public override void Execute()
        {
            // 실제 피해 판정은 전투 규칙이 정해지면 여기에 붙인다.
            Debug.Log($"[Stack] 공격 실행. 피해 {_damage}");
        }
    }
}
