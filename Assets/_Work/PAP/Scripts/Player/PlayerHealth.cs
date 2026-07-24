using System;
using System.Collections;
using _Work.PAP.Scripts.Agent;
using _Work.RYU._01.Script.Player;
using RYU.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace _Work.PAP.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour,IDamageable
    {
        AgentMovement AgentMovement;
        ShieldMode ShieldMode;
        [SerializeField] private int Health = 3;
        private int maxHealth;

        public event Action OnDeath;
        public UnityEvent OnHitEvent;
        public UnityEvent OnHitEndEvent;
        public UnityEvent OnHealEvent;

        public bool IsDead => Health <= 0;

        private void Awake()
        {
            maxHealth = Health;
            AgentMovement = GetComponent<AgentMovement>();
            ShieldMode = GetComponent<ShieldMode>();
        }

        public void TakeDamage(Vector3 dir)
        {
            // 실드 중이면 반짝하고 피해를 막는다
            if (ShieldMode != null && ShieldMode.IsShielded)
            {
                ShieldMode.Blink();
                return;
            }
            if (IsDead) return;

            Health--;

            AgentMovement.CanMove = false;
            AgentMovement.StopImmediately();
            AgentMovement.ApplyVelocity(dir);
            StartCoroutine(ForceRoutine());
            StartCoroutine(InfRoutine());
            OnHitEvent?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        private IEnumerator InfRoutine()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            yield return new WaitForSeconds(1f);
            gameObject.layer = LayerMask.NameToLayer("Agent");
            OnHitEndEvent?.Invoke();
        }

        public void TakeHeal(int amount)
        {
            if (Health >= maxHealth) return;
            OnHealEvent?.Invoke();
            Health += amount;
        }

        private IEnumerator ForceRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            AgentMovement.CanMove = true;
            AgentMovement.StopImmediately();
        }
    }
}