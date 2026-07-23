using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/ShieldItem", order = 0)]
    public class ShieldItem : AbstractItem
    {
        public override void Use(GameObject target)
        {
            //플레이어 주변 쉴드
        }
    }
}