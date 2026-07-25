using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/SlowItem", order = 0)]
    public class SlowItem : AbstractItem
    {
        public float duration = 4f;

        // 켜져 있는 동안은 다시 못 쓴다 (연타 방지)
        public override bool CanUse(GameObject user)
            => !(user.TryGetComponent(out SlowMode slow) && slow.IsActive);

        public override void Use(GameObject target)
        {
            if (!target.TryGetComponent(out SlowMode slow))
                return;

            // 이미 켜져 있으면 다시 켜지 않는다. 연타로 시간이 계속 갱신되는 것 방지
            if (slow.IsActive)
                return;

            slow.Activate(duration);
        }

        // 지속 동안 슬롯에 남아 남은 시간이 표시된다
        public override bool KeepAfterUse(GameObject user) => true;

        public override float CooldownRatio01(GameObject user)
            => user.TryGetComponent(out SlowMode slow) ? slow.RemainingRatio : 0f;

        public override bool IsFinished(GameObject user)
            => !user.TryGetComponent(out SlowMode slow) || !slow.IsActive;
    }
}