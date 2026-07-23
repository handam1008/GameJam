using System;
using System.Threading;
using System.Threading.Tasks;
using _Work.RYU._01.Script.FeedBack;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.PAP.Scripts.Agent
{
    public class DashMovement : MonoBehaviour
    {
        [SerializeField] private AfterImageEffect effect;
        [SerializeField] private float dashPower = 10f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float cooldown = 0f;

        [SerializeField] private FeedBackPlayer player;
        // [SerializeField] private CinemachineImpulseSource impulser;

        public AgentMovement Movement { get; private set; }
        // public NotifyValue<bool> DashState = new NotifyValue<bool>();
        
        private CancellationTokenSource _tokenSource;
        // private FeedbackPlayer _feedbackPlayer;
        private bool _active = true;
        

        private void Awake()
        {
            Movement = GetComponentInParent<AgentMovement>();
            // _feedbackPlayer = GetComponentInChildren<FeedbackPlayer>();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public void UseDash()
        {
            if (!_active) return;
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            DashAsync().Forget();
        }

        private async UniTaskVoid DashAsync()
        {
            try
            {
                _active = false;
                CoolDownAsync().Forget();
                // _feedbackPlayer.PlayAllFeedback();
                player.PlayAllFeedBack();
                Vector3 direction = new Vector3(Movement.MoveDirection.x,Movement.MoveDirection.y);
                direction.Normalize();
                if (direction == Vector3.zero)
                    direction = transform.forward;
                effect.StartTrail();
                Movement.CanMove = false;
                Movement.StopImmediately();
                Movement.ApplyVelocity(direction * dashPower);
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                //Canceled
            }
            finally
            {
                Movement.StopImmediately();
                Movement.CanMove = true;
                effect.StopTrail();
            }
        }

        private async UniTaskVoid CoolDownAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration+cooldown), cancellationToken: _tokenSource.Token);
            _active = true;
        }
    }
}