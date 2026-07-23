using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace CoreLib
{
    public class CameraEffectManager : MonoSingleTon<CameraEffectManager>
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;

        public async UniTask ZoomAsync(float value,float duration = 0.2f,Ease ease = Ease.InBack)
        {
            await DOTween.To(() => cinemachineCamera.Lens.OrthographicSize,x => cinemachineCamera.Lens.OrthographicSize = x,value,duration).SetEase(ease).AsyncWaitForCompletion();
        }
    }
}