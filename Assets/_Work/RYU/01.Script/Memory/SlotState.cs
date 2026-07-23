namespace RYU.Memory
{
    public enum SlotState
    {
        /// <summary>비어 있어 새로 넣을 수 있다.</summary>
        Free,

        /// <summary>아직 실행하지 않은 데이터가 들어 있다.</summary>
        Data,

        /// <summary>실행이 끝났거나 피격으로 망가졌다. GC로만 치운다.</summary>
        Garbage
    }
}
