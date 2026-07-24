using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/ShieldItem", order = 0)]
    public class ShieldItem : AbstractItem
    {
        public float duration = 4f;

        public override void Use(GameObject target)
        {
            target.TryGetComponent(out ShieldMode shield);
            shield?.Activate(duration);
        }
    }
}