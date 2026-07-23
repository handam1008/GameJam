using System;

namespace RYU.Memory
{
    public class MemorySpace : IMemorySpace
    {
        private readonly SlotState[] _slots;

        public MemorySpace(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _slots = new SlotState[capacity];
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
