namespace RYU.Memory
{
    /// <summary>
    /// 메모리 칸에 들어가는 것들의 공통 뼈대.
    /// 스캐너가 칸에 도착하면 Execute가 불린다.
    /// 무엇을 하는지는 상속받은 쪽이 정한다.
    /// </summary>
    public abstract class AbstractStack
    {
        /// <summary>UI와 로그에 쓰는 이름.</summary>
        public abstract string DisplayName { get; }

        /// <summary>스캐너가 이 칸에 도착했을 때 처리할 내용.</summary>
        public abstract void Execute();
    }
}
