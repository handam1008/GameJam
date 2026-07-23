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

        public int UsedCount => Count(state => state != SlotState.Free);

        public int GarbageCount => Count(state => state == SlotState.Garbage);

        public int FreeCount => Capacity - UsedCount;

        public SlotState GetSlot(int index) => _slots[index];

        public AbstractStack GetItem(int index) => _items[index];

        public bool TryAllocate(int cost)
        {
            if (cost <= 0 || cost > Capacity)
                return false;

            int start = FindContiguousFree(cost);
            if (start < 0)
                return false;

            for (int i = start; i < start + cost; i++)
                _slots[i] = SlotState.Data;

            return true;
        }

        public bool TryPushBottom(AbstractStack item)
        {
            if (item == null)
                return false;

            int free = FindFirstFree();
            if (free < 0)
                return false;

            // 맨 아래에 자리를 만들기 위해 빈칸까지의 내용을 한 칸씩 위로 민다.
            for (int i = free; i > 0; i--)
            {
                _slots[i] = _slots[i - 1];
                _items[i] = _items[i - 1];
            }

            _slots[0] = SlotState.Data;
            _items[0] = item;
            return true;
        }

        public void Clear(int index)
        {
            _slots[index] = SlotState.Free;
            _items[index] = null;
        }

        public int Corrupt()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == SlotState.Garbage)
                    continue;

                _slots[i] = SlotState.Garbage;
                _items[i] = null;
                return i;
            }

            return -1;
        }

        public int CollectGarbage()
        {
            int cleared = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == SlotState.Free)
                    continue;

                _slots[i] = SlotState.Free;
                _items[i] = null;
                cleared++;
            }
            return cleared;
        }

        private int Count(Func<SlotState, bool> match)
        {
            int count = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (match(_slots[i]))
                    count++;
            }
            return count;
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
