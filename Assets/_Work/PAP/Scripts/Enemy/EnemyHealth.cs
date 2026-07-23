using System;
using System.Collections;
using RYU.Combat;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.Events;

namespace _Work.PAP.Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour,IDamageable
    {
        [SerializeField] private int settingHealth;
        private int currentHealth;
        public UnityEvent OnDamaged;
        private Rigidbody2D rigid;

        private void Awake()
        {
            currentHealth = settingHealth;
            rigid = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(Vector3 dir)
        {
            if (--currentHealth > 0)
            {
                OnDamaged?.Invoke();
                rigid.AddForce(dir, ForceMode2D.Impulse);
                StartCoroutine(ForceRoutine());
            }
            else
            {
                //쥬금
                GetComponent<EnemyAI>().enabled = false;
                rigid.linearVelocity = Vector3.zero;
                transform.gameObject.layer = LayerMask.NameToLayer("Dead");
            }
        }

        private IEnumerator ForceRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            rigid.linearVelocity = Vector3.zero;
        }
    }
}