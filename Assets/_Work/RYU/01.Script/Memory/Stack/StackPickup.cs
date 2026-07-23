using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 바닥에 떨어져 있다가 플레이어가 닿으면 스택 맨 왼쪽으로 들어가는 것들의 공통 뼈대.
    /// 무엇이 들어갈지는 상속받은 쪽이 CreateStack에서 정한다.
    /// </summary>
    /// 오브젝트에 Is Trigger를 켠 Collider2D가 있어야 한다.
    public abstract class StackPickup : MonoBehaviour
    {
        /// <summary>이 아이템이 스택에 넣을 내용.</summary>
        protected abstract AbstractStack CreateStack();

        private void OnTriggerEnter2D(Collider2D other)
        {
            var memory = other.GetComponentInParent<MemoryController>();
            if (memory == null)
                return;

            // 스택이 꽉 찼으면 줍지 않고 바닥에 그대로 둔다.
            if (!memory.TryPickUp(CreateStack()))
                return;

            Destroy(gameObject);
        }
    }
}
