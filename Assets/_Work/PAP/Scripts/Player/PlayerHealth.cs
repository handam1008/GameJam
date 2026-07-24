using System;
using System.Collections;
using _Work.PAP.Scripts.Agent;
using _Work.RYU._01.Script.Player;
using RYU.Combat;
using UnityEngine;

namespace _Work.PAP.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour,IDamageable
    {
        AgentMovement AgentMovement;
        ShieldMode ShieldMode;
        [SerializeField] private int Health = 3;

        public event Action OnDeath;

        public bool IsDead => Health <= 0;

        private void Awake()
        {
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

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
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