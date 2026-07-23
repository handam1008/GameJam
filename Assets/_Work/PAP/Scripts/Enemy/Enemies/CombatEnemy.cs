using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Enemy.CombatEnemy;
using Enemies.FSM;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy.Enemies
{
    public enum CombatEnemyState : int
    {
        Idle = 0,
        Chase = 1,
        Attack = 2
    }
    public class CombatEnemy : AbstractEnemy
    {
        private EnemyStateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EnemyStateMachine();
            _stateMachine.Initialize(this);
            _stateMachine.AddState((int)CombatEnemyState.Idle,
                new CombatEnemyIdleState(this, _stateMachine, 0));
            _stateMachine.AddState((int)CombatEnemyState.Chase,
                new CombatEnemyChaseState(this, _stateMachine, 0));
        }

        private void Start()
        {
            _stateMachine.ChangeState((int)CombatEnemyState.Idle);
        }

        private void Update()
        {
            _stateMachine.UpdateCurrentState();
        }

        private void OnDestroy()
        {
            // 상태가 GameManager 이벤트를 구독한 채 파괴되지 않도록 정리한다 (예: ChaseState.OnMoveEvent)
            _stateMachine.CurrentState?.Exit();
        }
    }
}