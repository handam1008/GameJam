using System;
using System.Collections;
using _Work.PAP.Scripts.Agent;
using RYU.Combat;
using UnityEngine;

namespace _Work.PAP.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour,IDamageable
    {
        AgentMovement AgentMovement;

        private void Awake()
        {
            AgentMovement = GetComponent<AgentMovement>();
        }

        public void TakeDamage(Vector3 dir)
        {
            AgentMovement.CanMove = false;
            AgentMovement.StopImmediately();
            AgentMovement.ApplyVelocity(dir);
            StartCoroutine(ForceRoutine());
        }

        private IEnumerator ForceRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            AgentMovement.CanMove = true;
            AgentMovement.StopImmediately();
        }
    }
}