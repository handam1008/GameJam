using System;
using _Work.PAP.Scripts.Agent;
using UnityEngine;

namespace _Work.PAP.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        
        public AgentMovement AgentMovement { get; private set; }
        public DashMovement DashMovement { get; private set; }

        private void Awake()
        {
            AgentMovement = GetComponentInChildren<AgentMovement>();
            DashMovement = GetComponentInChildren<DashMovement>();
            playerInput.OnDashKeyPressed += HandleOnDash;
        }

        private void HandleOnDash()
        {
            DashMovement.UseDash();
        }

        private void Update()
        {
            AgentMovement.SetMovementDir(playerInput.MovementInput);
        }
    }
}