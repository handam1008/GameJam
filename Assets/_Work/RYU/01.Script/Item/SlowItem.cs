using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/SlowItem", order = 0)]
    public class SlowItem : AbstractItem
    {
        public override void Use(GameObject target)
        {
            //총알만 느려지게 해야함
        }
    }
}