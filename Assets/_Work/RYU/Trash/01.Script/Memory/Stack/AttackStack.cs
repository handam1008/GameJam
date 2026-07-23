using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 바닥에서 주운 공격 데이터.
    /// </summary>
    public class AttackStack : AbstractStack
    {
        private readonly float _damage;
        private readonly Sprite _icon;

        /// <summary>어느 스킬에서 나왔는지. 같은 출처면 병합 대상으로 본다.</summary>
        private readonly object _source;

        public AttackStack(float damage, Sprite icon = null, object source = null)
        {
            _damage = damage;
            _icon = icon;
            _source = source;
        }

        public override string DisplayName => $"공격 {_damage}";
        public override Sprite Icon
        {
            get => _icon;
            set => value = _icon;
        }

        public override bool CanMergeWith(AbstractStack other)
        {
            return other is AttackStack otherAttack
                   && _source != null
                   && _source.Equals(otherAttack._source);
        }

        public override void Execute()
        {
            // 실제 피해 판정은 전투 규칙이 정해지면 여기에 붙인다.
            Debug.Log($"[Stack] 공격 실행. 피해 {_damage}");
        }
    }
}
