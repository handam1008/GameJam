using CoreLib;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.PAP.Scripts.FeedbackSystem
{
    public class ZoomInOutFeedback : AbstractFeedBack
    {
        public override void CreateFeedBack()
        {
            ZoomInOutAsync().Forget();
        }

        private async UniTaskVoid ZoomInOutAsync()
        {
            await CameraEffectManager.Instance.ZoomAsync(10.5f);
            CameraEffectManager.Instance.ZoomAsync(10f).Forget();
        }

        public override void StopFeedBack()
        {
        }
    }
}