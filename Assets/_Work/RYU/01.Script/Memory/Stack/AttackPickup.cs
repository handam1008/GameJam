using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 바닥에 떨어진 공격.
    /// </summary>
    public class AttackPickup : StackPickup
    {
        [SerializeField, Min(0f)] private float damage = 10f;

        protected override AbstractStack CreateStack() => new AttackStack(damage);
    }
}
