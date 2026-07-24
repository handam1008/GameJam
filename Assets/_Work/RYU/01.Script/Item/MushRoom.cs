using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/MushRoom", order = 0)]
    public class MushRoom : AbstractItem
    {
        public float duration = 5f;
        public float scale = 2.5f;
        
        public override void Use(GameObject target)
        {
            target.TryGetComponent(out GiantMode giantMode);
            giantMode?.Activate(duration, scale);
        }
    }
}