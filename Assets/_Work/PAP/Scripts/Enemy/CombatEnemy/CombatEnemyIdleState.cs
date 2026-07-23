using _Work.PAP.Scripts.Enemy.Enemies;
using _Work.PAP.Scripts.Enemy.FSM;
using Enemies.FSM;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy.CombatEnemy
{
    public class CombatEnemyIdleState : EnemyState
    {
        private float currentPatrolTime;
        public CombatEnemyIdleState(AbstractEnemy enemy, EnemyStateMachine stateMachine, int animationHash) : base(enemy, stateMachine, animationHash)
        {
        }

        public override void Enter()
        {
            _enemy.AgentMovement.SetMoveDirection(Vector2Int.zero);
            currentPatrolTime = Time.time + Random.Range(0,_enemy.patrolInterval);
        }

        public override void UpdateState()
        {
            if (_enemy.CanSeePlayer())
            {
                _stateMachine.ChangeState((int)CombatEnemyState.Chase);
                return;
            }

            if (!_enemy.patrolWhenIdle)
            {
                _enemy.AgentMovement.SetMoveDirection(Vector2Int.zero);
                return;
            }

            if (Time.time >= currentPatrolTime)
            {
                _enemy.Patrol();
                currentPatrolTime = Time.time + _enemy.patrolInterval;
            }
        }
        
    }
}