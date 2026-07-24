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
            //임태헌 잔상처리해
            target.TryGetComponent(out SlowMode slow);
            slow?.Activate(duration);
        }
    }
}