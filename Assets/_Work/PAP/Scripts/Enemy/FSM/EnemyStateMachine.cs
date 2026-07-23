using System.Collections.Generic;
using _Work.PAP.Scripts.Enemy;
using _Work.PAP.Scripts.Enemy.FSM;
using Mono.Cecil;
using UnityEngine;

namespace Enemies.FSM
{
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }
        public Dictionary<int, EnemyState> StateDictionary;

        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy enemy)
        {
            _owner = enemy;
            StateDictionary = new Dictionary<int, EnemyState>(); //이거 빼먹지마.
        }

        public void ChangeState(int stateIndex, bool forceMode = false)
        {
            if (!_owner.CanStateChangeable && !forceMode) return; //상태 변경 불가능이고, 강제도 아니라면
            if (_owner.IsDead) return; //이미 사망했다면

            CurrentState?.Exit();
            CurrentState = StateDictionary[stateIndex];
            CurrentState.Enter();
        }

        public void AddState(int stateIndex, EnemyState state)
        {
            StateDictionary.Add(stateIndex, state);
        }

        public void UpdateCurrentState()
        {
            CurrentState?.UpdateState();
        }
    }
}