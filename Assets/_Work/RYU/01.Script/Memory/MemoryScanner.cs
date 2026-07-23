using System;
using _Work.PAP.Scripts.Systems;
using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 일정한 틱으로 도는 프로그램 카운터.
    ///
    /// 시계는 게임 내내 멈추지 않는다. 스택이 비어 있어도 틱은 계속 센다.
    /// 스캐너는 한 틱 동안 한 칸에 머무르고, 틱이 끝나는 순간 그 칸을 처리한 뒤 다음 칸으로 옮긴다.
    /// 다음 칸이 몇 칸 아래에 있든, 꼭대기로 되돌아가든 이동은 항상 한 틱이다.
    /// 그래서 어떤 규칙이 붙어도 박자가 흔들리지 않는다.
    ///
    /// 비어 있는 동안에도 틱이 돌기 때문에, 새로 쌓으면 다음 틱에 맞춰 중간부터 이어진다.
    /// </summary>
    public class MemoryScanner : MonoBehaviour
    {
        [SerializeField] private MemoryController memory;

        [Tooltip("1초에 몇 틱이 지나갈지. 게임 전체의 박자를 정한다.")]

        /// <summary>이번 틱이 얼마나 지났는지. 0에서 1 사이.</summary>

        /// <summary>지금 머무르고 있는 칸. 훑을 게 없으면 -1.</summary>
        private int _current = -1;

        /// <summary>훑을 게 있어서 스캔선이 떠 있는 상태.</summary>
        public bool IsActive => _current >= 0;

        /// <summary>스캔선이 지금 있는 칸. 없으면 -1.</summary>
        public int CurrentIndex => _current;

        /// <summary>그 칸의 어디쯤인지. 0이면 위쪽 끝, 1이면 아래쪽 끝.</summary>
        public float Fraction => GameManager.Instance.targetSpeed;

        /// <summary>칸이 실행됐을 때 그 칸의 인덱스를 발행한다.</summary>
        public event Action<int> OnSlotExecuted;

        /// <summary>회차가 끝나 꼭대기로 돌아갈 때 발행한다.</summary>
        public event Action OnCycleReset;

        private void Reset()
        {
            memory = GetComponent<MemoryController>();
        }

        private void Awake()
        {
            if (memory == null)
                memory = GetComponent<MemoryController>();
        }

        private void Start()
        {
            // 이번 틱에 어느 방향으로 갈지(MoveStack.Execute 등)를 AgentMovement.TryStep보다 먼저 확정해야 한다.
            GameManager.Instance.OnBeforeMoveEvent += Advance;
        }

        private void Update()
        {
            if (memory == null)
                return;


            // 프레임이 길어 틱을 여러 번 넘겼어도 넘긴 만큼 모두 처리한다.

            AcquireIfIdle();
        }

        /// <summary>
        /// 비어 있다가 뭔가 생기면 다음 틱을 기다리지 않고 지금 위상에 바로 붙는다.
        /// 틱이 절반 지났으면 스캔선도 그 칸의 절반 지점에서 시작한다.
        /// </summary>
        private void AcquireIfIdle()
        {
            if (_current >= 0)
                return;

            int top = FindTopOccupied();
            if (top < 0)
                return;

            _current = top;

            OnCycleReset?.Invoke();
        }

        /// <summary>틱 경계. 머물던 칸을 처리하고 다음 칸으로 옮긴다.</summary>
        private void Advance()
        {
            if (_current >= 0)
                Resolve(_current);

            _current = ChooseNext();
        }

        private void Resolve(int index)
        {
            SlotState state = memory.GetSlot(index);

            // 가비지는 지나가며 치운다. 실행 횟수에는 세지 않는다.
            if (state == SlotState.Garbage)
            {
                memory.ClearSlot(index);
                return;
            }

            if (state != SlotState.Data)
                return;

            AbstractStack item = memory.GetItem(index);

            // 겹쳐 쌓인 능력은 실제로 칸을 여러 개 차지한다. 그 능력을 담은 칸을 하나 지나칠 때마다
            // 이 칸은 (실행 여부와 상관없이) 항상 비워지지만, 쌓인 개수만큼 다 지나가야(마지막 통과에서만)
            // 실제로 Execute가 불린다.
            bool executed = item == null || item.Pass();

            if (executed)
                OnSlotExecuted?.Invoke(index);

            // 지나친 칸은 사라진다.
            memory.ClearSlot(index);
        }

        /// <summary>
        /// 다음에 머무를 칸을 고른다. 아래로 내려가며 다음 찬 칸을 찾고,
        /// 바닥까지 다 훑었으면(더 이상 아래에 찬 칸이 없으면) 처음부터 다시 맨 위에서 시작한다.
        /// 도중에 새로 쌓여도 이 흐름을 끊지 않는다.
        /// </summary>
        private int ChooseNext()
        {
            for (int i = _current - 1; i >= 0; i--)
            {
                if (memory.GetSlot(i) != SlotState.Free)
                    return i;
            }

            int top = FindTopOccupied();
            if (top >= 0)
                OnCycleReset?.Invoke();

            return top;
        }

        /// <summary>
        /// 가장 위에 있는 찬 칸. 가비지도 지나가며 치워야 하므로 같이 센다.
        /// 하나도 없으면 -1.
        /// </summary>
        private int FindTopOccupied()
        {
            for (int i = memory.Capacity - 1; i >= 0; i--)
            {
                if (memory.GetSlot(i) != SlotState.Free)
                    return i;
            }
            return -1;
        }
    }
}
