using _Work.PAP.Scripts.Player;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/HealItem", order = 0)]
    public class HealItem : AbstractItem
    {
        public int amount = 1;


        public override void Use(GameObject target)
        {
            target.TryGetComponent(out PlayerHealth  playerHealth);
            playerHealth?.TakeHeal(amount);
        }
    }
}