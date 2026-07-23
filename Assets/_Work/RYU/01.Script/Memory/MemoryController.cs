using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RYU.Memory
{
    /// <summary>
    /// 메모리 칸의 소유자. 공격은 TryConsume으로 메모리를 점유하고,
    /// GC 키 입력은 가비지를 비우면서 OnGarbageCollected를 발행한다.
    /// </summary>
    public class MemoryController : MonoBehaviour
    {
        [Header("Memory")]
        [SerializeField, Min(1)] private int capacity = 10;
        [SerializeField, Min(1)] private int attackCost = 1;

        [Header("Garbage Collect")]
        [SerializeField] private Key gcKey = Key.R;
        [SerializeField, Min(0f)] private float gcCooldown = 0.5f;

        [Header("Overflow")]
        [SerializeField, Min(0f)] private float overflowStunDuration = 1f;

        [Header("Debug")]
        [SerializeField] private bool useDebugAttackKey;
        [SerializeField] private Key debugAttackKey = Key.Z;

        private IMemorySpace _memory;
        private float _stunTimer;
        private float _gcTimer;

        /// <summary>메모리 상태가 바뀔 때마다 발행. UI 갱신용.</summary>
        public event Action OnMemoryChanged;

        /// <summary>GC 성공 시 비운 가비지 칸 수를 발행. 광역 피해 계산용.</summary>
        public event Action<int> OnGarbageCollected;

        /// <summary>메모리가 모자라 할당에 실패했을 때 발행.</summary>
        public event Action OnOverflow;

        public int Capacity => _memory.Capacity;
        public int GarbageCount => _memory.GarbageCount;
        public int FreeCount => _memory.FreeCount;
        public bool IsStunned => _stunTimer > 0f;

        public SlotState GetSlot(int index) => _memory.GetSlot(index);

        private void Awake()
        {
            _memory = new MemorySpace(capacity);
        }

        private void Update()
        {
            if (_stunTimer > 0f)
                _stunTimer -= Time.deltaTime;

            if (_gcTimer > 0f)
                _gcTimer -= Time.deltaTime;

            ReadInput();
        }

        /// <summary>
        /// 공격 등 메모리를 쓰는 행동에서 호출한다.
        /// 할당에 성공하면 true, 오버플로가 나면 false.
        /// </summary>
        public bool TryConsume()
        {
            return TryConsume(attackCost);
        }

        public bool TryConsume(int cost)
        {
            if (IsStunned)
                return false;

            if (!_memory.TryAllocate(cost))
            {
                _stunTimer = overflowStunDuration;
                OnOverflow?.Invoke();
                return false;
            }

            OnMemoryChanged?.Invoke();
            return true;
        }

        public void CollectGarbage()
        {
            if (IsStunned || _gcTimer > 0f)
                return;

            int cleared = _memory.CollectGarbage();
            if (cleared <= 0)
                return;

            _gcTimer = gcCooldown;
            OnMemoryChanged?.Invoke();
            OnGarbageCollected?.Invoke(cleared);
        }

        private void ReadInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard[gcKey].wasPressedThisFrame)
                CollectGarbage();

            if (useDebugAttackKey && keyboard[debugAttackKey].wasPressedThisFrame)
                TryConsume();
        }
    }
}
