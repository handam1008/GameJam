using RYU.Combat;
using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 적 공격을 받아 메모리 오른쪽으로 밀어넣는다.
    /// MemoryController와 같은 오브젝트에 붙인다.
    /// </summary>
    [RequireComponent(typeof(MemoryController))]
    public class PlayerHitReceiver : MonoBehaviour, IDamageable
    {
        private MemoryController _memory;

        private void Awake()
        {
            _memory = GetComponent<MemoryController>();
        }

        public void TakeDamage(float amount)
        {
            // 대미지 크기와 상관없이 칸 하나가 가비지가 된다.
            _memory.TakeHit();
        }
    }
}
