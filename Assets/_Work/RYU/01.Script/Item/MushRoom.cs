using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/MushRoom", order = 0)]
    public class MushRoom : AbstractItem
    {
        public float duration = 5f;

        public override void Use(GameObject target)
        {
            target.TryGetComponent(out GiantMode giant);
            giant?.Activate(duration);
        }
    }
}