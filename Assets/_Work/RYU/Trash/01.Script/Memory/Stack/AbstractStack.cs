using UnityEngine;

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

        /// <summary>메모리 칸 UI에 표시할 아이콘. 종류마다 다르며, 없으면 null.</summary>
        public abstract Sprite Icon { get; set; }

        /// <summary>
        /// 이 스택이 칸에 심어질 때의 칸 상태. 기본은 실행 대상인 Data.
        /// 피격처럼 실행되지 않고 GC로만 치워야 하는 것은 Garbage로 재정의한다.
        /// </summary>
        public virtual SlotState SlotState => SlotState.Data;

        /// <summary>
        /// 같은 능력이 겹쳐 쌓인 개수. 기본은 1이며, 병합될 때마다 늘어난다.
        /// 칸 하나에 아이콘 하나만 그대로 남고(스프라이트가 깨지지 않고), 개수만 UI 텍스트로 늘어난다.
        /// </summary>
        public int StackCount { get; private set; } = 1;

        /// <summary>스캐너가 이 칸에 도착했을 때 처리할 내용.</summary>
        public abstract void Execute();

        /// <summary>이 스택과 저 스택이 같은 능력이라 하나로 합칠 수 있는지. 기본은 합치지 않는다.</summary>
        public virtual bool CanMergeWith(AbstractStack other) => false;

        /// <summary>같은 능력을 하나 더 겹친다. 칸은 늘어나지 않고 개수만 늘어난다.</summary>
        public void Merge(AbstractStack other)
        {
            StackCount += other.StackCount;
        }

        /// <summary>
        /// 스캐너가 이 칸을 지나칠 때마다 호출된다.
        /// 쌓인 개수만큼 지나가야 실제로 실행되며, 마지막 통과에서만 Execute를 부르고 true를 돌려준다.
        /// false를 돌려주면 아직 남았다는 뜻이라 칸을 비우지 않는다.
        /// </summary>
        public bool Pass()
        {
            StackCount--;
            if (StackCount > 0)
                return false;

            Execute();
            return true;
        }
    }
}
