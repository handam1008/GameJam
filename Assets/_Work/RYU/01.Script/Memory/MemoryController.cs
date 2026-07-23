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
        [Tooltip("메모리가 꽉 찬 채로 이 시간이 지나면 죽는다.")]
        [SerializeField, Min(0.1f)] private float overflowDeathTime = 3f;


        private IMemorySpace _memory;
        private float _stunTimer;
        private float _gcTimer;
        private float _overflowTimer;

        /// <summary>메모리 상태가 바뀔 때마다 발행. UI 갱신용.</summary>
        public event Action OnMemoryChanged;

        /// <summary>GC 성공 시 비운 가비지 칸 수를 발행. 광역 피해 계산용.</summary>
        public event Action<int> OnGarbageCollected;

        /// <summary>메모리가 모자라 할당에 실패했을 때 발행.</summary>
        public event Action OnOverflow;

        /// <summary>꽉 찬 상태를 못 풀고 시간이 다 됐을 때 발행.</summary>
        public event Action OnDeath;

        public int Capacity => Memory.Capacity;

        /// <summary>비어 있지 않은 칸 수. 데이터와 가비지를 모두 센다.</summary>
        public int UsedCount => Memory.UsedCount;

        public int GarbageCount => Memory.GarbageCount;
        public int FreeCount => Memory.FreeCount;
        public bool IsStunned => _stunTimer > 0f;

        /// <summary>빈칸이 하나도 없는 상태.</summary>
        public bool IsOverflowing => FreeCount == 0;

        public bool IsDead { get; private set; }

        /// <summary>죽음까지 얼마나 왔는지. 0이면 안전, 1이면 사망 직전.</summary>
        public float OverflowProgress =>
            IsOverflowing ? Mathf.Clamp01(_overflowTimer / overflowDeathTime) : 0f;

        public SlotState GetSlot(int index) => Memory.GetSlot(index);

        public AbstractStack GetItem(int index) => Memory.GetItem(index);

        /// <summary>
        /// 바닥에서 주운 것을 스택 맨 아래에 넣는다.
        /// 빈칸이 없으면 줍지 않고 false를 돌려준다.
        /// </summary>
        public bool TryPickUp(AbstractStack item)
        {
            if (!Memory.TryPushBottom(item))
                return false;

            OnMemoryChanged?.Invoke();
            return true;
        }

        /// <summary>스캐너가 지나가며 칸을 비운다. 쓴 데이터와 가비지 모두 사라진다.</summary>
        public void ClearSlot(int index)
        {
            Memory.Clear(index);
            OnMemoryChanged?.Invoke();
        }

        /// <summary>
        /// 적에게 맞으면 아래에서부터 가비지가 아닌 첫 칸이 가비지가 된다.
        /// 무엇이 들어 있었든 사라진다.
        /// </summary>
        public void TakeHit()
        {
            if (Memory.Corrupt() < 0)
                return;

            OnMemoryChanged?.Invoke();
        }


        /// <summary>
        /// 다른 컴포넌트가 Awake 순서와 무관하게 접근할 수 있도록 첫 호출 시점에 만든다.
        /// </summary>
        private IMemorySpace Memory => _memory ??= new MemorySpace(capacity);

        /// <summary>
        /// 꽉 찬 채로 버틴 시간을 잰다. 한 칸이라도 비면 타이머가 처음으로 돌아간다.
        /// </summary>
        private void TickOverflow()
        {
            if (!IsOverflowing)
            {
                _overflowTimer = 0f;
                return;
            }

            _overflowTimer += Time.deltaTime;
            if (_overflowTimer < overflowDeathTime)
                return;

            IsDead = true;
            OnDeath?.Invoke();
        }

        private void Update()
        {
            if (IsDead)
                return;

            TickOverflow();

            if (_stunTimer > 0f)
                _stunTimer -= Time.deltaTime;

            if (_gcTimer > 0f)
                _gcTimer -= Time.deltaTime;
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

            if (!Memory.TryAllocate(cost))
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

            int cleared = Memory.CollectGarbage();
            if (cleared <= 0)
                return;

            _gcTimer = gcCooldown;
            OnMemoryChanged?.Invoke();
            OnGarbageCollected?.Invoke(cleared);
        }
    }
}
