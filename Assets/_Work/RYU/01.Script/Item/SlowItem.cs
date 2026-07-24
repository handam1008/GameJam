using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/SlowItem", order = 0)]
    public class SlowItem : AbstractItem
    {
        public float duration = 4f;

        public override void Use(GameObject target)
        {
            target.TryGetComponent(out SlowMode slow);
            slow?.Activate(duration);
        }

        // 지속 동안 슬롯에 남아 남은 시간이 표시된다
        public override bool KeepAfterUse(GameObject user) => true;

        public override float CooldownRatio01(GameObject user)
            => user.TryGetComponent(out SlowMode slow) ? slow.RemainingRatio : 0f;

        public override bool IsFinished(GameObject user)
            => !user.TryGetComponent(out SlowMode slow) || !slow.IsActive;
    }
}