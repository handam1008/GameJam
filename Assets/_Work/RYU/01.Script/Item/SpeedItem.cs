using _Work.PAP.Scripts.Agent;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/SpeedItem", order = 0)]
    public class SpeedItem : AbstractItem
    {
        public float amount = 10;
        public override void Use(GameObject target)
        {
            target.TryGetComponent(out AgentMovement agent);
            agent?.PlusSpeed(amount);
        }
    }
}