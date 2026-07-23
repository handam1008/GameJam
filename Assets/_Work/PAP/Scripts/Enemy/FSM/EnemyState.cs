using Enemies.FSM;

namespace _Work.PAP.Scripts.Enemy.FSM
{
    public abstract class EnemyState
    {
        protected AbstractEnemy _enemy;
        protected EnemyStateMachine _stateMachine;

        protected int _animationHash;
        protected bool _endTriggerCall;

        public EnemyState(AbstractEnemy enemy, EnemyStateMachine stateMachine, int animationHash)
        {
            _enemy = enemy;
            _stateMachine = stateMachine;
            _animationHash = animationHash;
        }
        //진입
        public virtual void Enter()
        {
            // _enemy.AnimatorCompo.SetBoolean(_animationHash, true);
        }
        //실행
        public virtual void UpdateState() {}
        //퇴장
        public virtual void Exit()
        {
            // _enemy.AnimatorCompo.SetBoolean(_animationHash, false);
        }
        
        public void AnimationEndTrigger() => _endTriggerCall = true;
        
    }
}