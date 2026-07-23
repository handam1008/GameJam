using RYU.Combat;
using UnityEngine;

namespace RYU.Dev
{
    /// <summary>
    /// 광역 피해 판정 확인용 허수아비. 적 코드가 나오면 지운다.
    /// </summary>
    public class DamageDummy : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHp = 100f;

        private float _hp;

        private void Awake()
        {
            _hp = maxHp;
        }

        public void TakeDamage(Vector3 amount)
        {
            // _hp -= amount;
            // Debug.Log($"[Dummy] {name} 피해 {amount} → 남은 체력 {_hp}", this);
            //
            // if (_hp <= 0f)
            //     gameObject.SetActive(false);
        }
    }
}
