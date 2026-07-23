namespace RYU.Memory
{
    /// <summary>
    /// 메모리 칸의 상태를 관리하는 규칙. MonoBehaviour에 의존하지 않는다.
    /// </summary>
    public interface IMemorySpace
    {
        int Capacity { get; }

        /// <summary>비어 있지 않은 칸 수. 데이터와 가비지를 모두 센다.</summary>
        int UsedCount { get; }

        /// <summary>가비지가 된 칸 수.</summary>
        int GarbageCount { get; }

        int FreeCount { get; }

        SlotState GetSlot(int index);

        /// <summary>칸에 담긴 것. 비었거나 가비지면 null.</summary>
        AbstractStack GetItem(int index);

        /// <summary>연속된 빈칸 cost개를 데이터로 채운다. 공간이 없으면 false.</summary>
        bool TryAllocate(int cost);

        /// <summary>맨 아래에 밀어넣고 나머지를 위로 민다. 빈칸이 없으면 false.</summary>
        bool TryPushBottom(AbstractStack item);

        /// <summary>칸을 비운다. 스캐너가 지나가며 쓴 것과 가비지를 치울 때 쓴다.</summary>
        void Clear(int index);

        /// <summary>
        /// 아래에서부터 훑어 가비지가 아닌 첫 칸을 가비지로 만든다.
        /// 망가뜨린 칸의 인덱스를 돌려주고, 이미 전부 가비지면 -1.
        /// </summary>
        int Corrupt();

        /// <summary>모든 칸을 비우고 비운 칸 수를 반환한다.</summary>
        int CollectGarbage();
    }
}
