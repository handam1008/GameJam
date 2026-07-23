using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 적에게 맞아 생긴 가비지 한 칸. 다른 스택과 똑같이 맨 아래에 쌓이지만,
    /// Garbage 칸이라 스캐너가 지나가도 실행되지 않고 GC로만 치울 수 있다.
    /// </summary>
    public class HitStack : AbstractStack
    {
        public override string DisplayName => "피격";
        public override Sprite Icon
        {
            get;
            set;
        }
        public override SlotState SlotState => SlotState.Data;

        public HitStack(Sprite icon)
        {
            Icon = icon;
        }

        public override void Execute()
        {
            // 가비지 칸이라 스캐너는 Execute를 부르지 않고 바로 비운다. 여기 올 일은 없다.
        }
    }
}
