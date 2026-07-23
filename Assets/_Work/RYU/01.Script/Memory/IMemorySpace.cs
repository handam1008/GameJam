namespace RYU.Memory
{
    /// <summary>
    /// 메모리 칸의 상태를 관리하는 규칙. MonoBehaviour에 의존하지 않는다.
    /// </summary>
    public interface IMemorySpace
    {
        int Capacity { get; }
        int GarbageCount { get; }
        int FreeCount { get; }

        SlotState GetSlot(int index);

        /// <summary>연속된 빈칸 cost개를 가비지로 채운다. 공간이 없으면 false.</summary>
        bool TryAllocate(int cost);

        /// <summary>모든 가비지 칸을 비우고 비운 칸 수를 반환한다.</summary>
        int CollectGarbage();
    }
}
