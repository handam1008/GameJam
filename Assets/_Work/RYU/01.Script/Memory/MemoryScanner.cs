using System;
using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 메모리 칸을 왼쪽에서 오른쪽으로 훑는 프로그램 카운터.
    /// 칸의 오른쪽 끝에 닿으면 그 칸이 실행된다.
    /// 한 회차가 끝나면 맨 앞으로 돌아가고, 다음 회차는 한 칸 더 나아간다.
    /// (1번 → 1,2번 → 1,2,3번 …)
    /// 빈 칸에 닿으면 거기서 멈추고 처음부터 다시 시작한다.
    ///
    /// 실행된 칸을 어떻게 처리할지는 아직 정하지 않았다.
    /// 지금은 어느 칸이 실행됐는지 알리기만 한다.
    /// </summary>
    public class MemoryScanner : MonoBehaviour
    {
        [SerializeField] private MemoryController memory;

        [Tooltip("한 칸을 지나가는 데 걸리는 시간(초). 작을수록 빠르다.")]
        [SerializeField, Min(0.01f)] private float secondsPerSlot = 0.5f;

        /// <summary>맨 왼쪽이 0. 1.5면 1번 칸의 한가운데. UI가 이 값으로 스캔선을 그린다.</summary>
        private float _position;

        /// <summary>이번 회차에 실행할 칸 수. 1부터 시작해 회차마다 하나씩 는다.</summary>
        private int _cycleLength = 1;

        /// <summary>다음에 오른쪽 끝이 닿을 칸.</summary>
        private int _nextSlot;

        public float Position => _position;

        /// <summary>스캐너가 지금 지나고 있는 칸. 아직 시작 전이면 0.</summary>
        public int CurrentIndex => Mathf.FloorToInt(_position);

        /// <summary>칸이 실행됐을 때 그 칸의 인덱스를 발행한다.</summary>
        public event Action<int> OnSlotExecuted;

        /// <summary>한 회차가 끝나 맨 앞으로 돌아갈 때 발행한다.</summary>
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

        private void Update()
        {
            if (memory == null)
                return;

            _position += Time.deltaTime / secondsPerSlot;

            // 한 프레임에 여러 칸을 지나칠 수 있으므로 닿은 경계를 모두 처리한다.
            while (_position >= _nextSlot + 1)
            {
                if (!TryExecute(_nextSlot))
                    break;

                _nextSlot++;

                if (_nextSlot >= _cycleLength)
                {
                    EndCycle();
                    break;
                }
            }
        }

        /// <summary>칸을 실행한다. 비어 있어서 처음으로 돌아갔으면 false.</summary>
        private bool TryExecute(int index)
        {
            if (index >= memory.Capacity || memory.GetSlot(index) == SlotState.Free)
            {
                RestartFromBeginning();
                return false;
            }

            // 칸에 담긴 게 있으면 그 내용을 처리한다. 디버그 키로 채운 칸은 내용이 없어 지나간다.
            memory.GetItem(index)?.Execute();

            OnSlotExecuted?.Invoke(index);
            return true;
        }

        /// <summary>이번 회차를 끝내고 한 칸 더 긴 회차를 준비한다.</summary>
        private void EndCycle()
        {
            _cycleLength++;

            if (_cycleLength > memory.Capacity)
            {
                RestartFromBeginning();
                return;
            }

            _position = 0f;
            _nextSlot = 0;
            OnCycleReset?.Invoke();
        }

        private void RestartFromBeginning()
        {
            _position = 0f;
            _nextSlot = 0;
            _cycleLength = 1;
            OnCycleReset?.Invoke();
        }
    }
}
