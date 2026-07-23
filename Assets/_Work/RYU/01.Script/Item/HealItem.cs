using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/HealItem", order = 0)]
    public class HealItem : AbstractItem
    {
        public float amount = 30f;


        public override void Use(GameObject target)
        {
            //플레이어 회복
        }
    }
}