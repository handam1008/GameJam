using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/SpeedItem", order = 0)]
    public class SpeedItem : AbstractItem
    {
        // 빨라지는 지속시간(초)
        public float duration = 4f;

        public override void Use(GameObject target)
        {
            if (!target.TryGetComponent(out SpeedMode speed))
                return;

            // 이미 켜져 있으면 다시 켜지 않는다. 연타로 시간이 계속 갱신되는 것 방지
            if (speed.IsActive)
                return;

            speed.Activate(duration);
        }

        // 지속 동안 슬롯에 남아 남은 시간이 표시된다
        public override bool KeepAfterUse(GameObject user) => true;

        public override float CooldownRatio01(GameObject user)
            => user.TryGetComponent(out SpeedMode speed) ? speed.RemainingRatio : 0f;

        public override bool IsFinished(GameObject user)
            => !user.TryGetComponent(out SpeedMode speed) || !speed.IsActive;
    }
}
