using RYU.Memory;
using UnityEngine;

namespace RYU.Skill
{
    /// <summary>
    /// 바닥에 떨어진 스킬. 플레이어가 닿으면 버튼 목록에 들어간다.
    /// 스택에 넣는 건 버튼을 눌렀을 때다.
    /// 오브젝트에 Is Trigger를 켠 Collider2D가 있어야 한다.
    /// </summary>
    public class SkillPickup : MonoBehaviour
    {
        [SerializeField] private SkillDefinition skill;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (skill == null || !IsPlayer(other))
                return;

            var inventory = FindAnyObjectByType<SkillInventory>();
            if (inventory == null)
                return;

            inventory.Add(skill);
            Destroy(gameObject);
        }

        /// <summary>
        /// 콜라이더와 MemoryController가 다른 오브젝트에 있어도 플레이어로 알아본다.
        /// </summary>
        private static bool IsPlayer(Collider2D other)
        {
            if (other.GetComponentInParent<MemoryController>() != null)
                return true;

            Rigidbody2D body = other.attachedRigidbody;
            return body != null && body.GetComponentInChildren<MemoryController>() != null;
        }
    }
}
