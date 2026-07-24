using Combat.Effects;
using Systems;
using UnityEngine;

namespace _Work.PAP.Scripts.FeedbackSystem
{
    public class PlayEffectFeedback : AbstractFeedBack
    {
        [SerializeField] protected PoolItemSO effect;
        public override void CreateFeedBack()
        {
            EffectPlayer effectPlayer = PoolManager.Instance.Pop(effect.ItemName) as EffectPlayer;
            effectPlayer.SetPositionAndPlay(transform.position);
        }

        public override void StopFeedBack()
        {
        }
    }
}