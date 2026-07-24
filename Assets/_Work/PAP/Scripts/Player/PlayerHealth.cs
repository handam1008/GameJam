using System;
using System.Collections;
using _Work.PAP.Scripts.Agent;
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
        
        public event Action OnDeath;
        public UnityEvent OnHitEvent;
        public UnityEvent OnHitEndEvent;

        public bool IsDead => Health <= 0;

        private void Awake()
        {
            AgentMovement = GetComponent<AgentMovement>();
            ShieldMode = GetComponent<ShieldMode>();
        }

        public void TakeDamage(Vector3 dir)
        {
            if (ShieldMode != null && ShieldMode.IsShielded) return;
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
            if (Health >= 3) return;
            
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