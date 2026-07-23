using System;

namespace RYU.Memory
{
    public class MemorySpace : IMemorySpace
    {
        private readonly SlotState[] _slots;

        /// <summary>칸에 실제로 담긴 것. 디버그 키로 채운 칸은 내용 없이 자리만 차지해 null이다.</summary>
        private readonly AbstractStack[] _items;

        public MemorySpace(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _slots = new SlotState[capacity];
            _items = new AbstractStack[capacity];
        }

        public int Capacity => _slots.Length;

        public int GarbageCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _slots.Length; i++)
                {
                    if (_slots[i] == SlotState.Garbage)
                        count++;
                }
                return count;
            }
        }

        public int FreeCount => Capacity - GarbageCount;

        public SlotState GetSlot(int index) => _slots[index];

        public AbstractStack GetItem(int index) => _items[index];

        public bool TryPushLeft(AbstractStack item)
        {
            if (item == null)
                return false;

            int free = FindFirstFree();
            if (free < 0)
                return false;

            // 왼쪽에 자리를 만들기 위해 빈칸까지의 내용을 한 칸씩 오른쪽으로 민다.
            for (int i = free; i > 0; i--)
            {
                _slots[i] = _slots[i - 1];
                _items[i] = _items[i - 1];
            }

            _slots[0] = SlotState.Garbage;
            _items[0] = item;
            return true;
        }

        private int FindFirstFree()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == SlotState.Free)
                    return i;
            }
            return -1;
        }

        public bool TryAllocate(int cost)
        {
            if (cost <= 0 || cost > Capacity)
                return false;

            int start = FindContiguousFree(cost);
            if (start < 0)
                return false;

            for (int i = start; i < start + cost; i++)
                _slots[i] = SlotState.Garbage;

            return true;
        }

        public int CollectGarbage()
        {
            int cleared = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != SlotState.Garbage)
                    continue;

                _slots[i] = SlotState.Free;
                _items[i] = null;
                cleared++;
            }
            return cleared;
        }

        private int FindContiguousFree(int length)
        {
            int run = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == SlotState.Free)
                {
                    run++;
                    if (run == length)
                        return i - length + 1;
                }
                else
                {
                    run = 0;
                }
            }
            return -1;
        }
    }
}
