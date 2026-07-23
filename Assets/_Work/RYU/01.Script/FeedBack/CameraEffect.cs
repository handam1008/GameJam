using DG.Tweening;
using UnityEngine;

namespace _Work.RYU._01.Script.FeedBack
{
    /// <summary>
    /// 피격 순간 카메라를 흔든다. DOTween의 셰이크는 위치가 매끄럽게 이어져
    /// 순수 랜덤보다 훨씬 자연스럽게 잦아든다.
    /// 위치 흔들림에 살짝의 기울어짐을 얹어 얻어맞은 느낌을 낸다.
    /// </summary>
    public class CameraEffect : AbstractFeedBack
    {
        [SerializeField] private Camera targetCamera;

        [Header("Shake")]
        [Tooltip("흔들리는 시간(초).")]
        [SerializeField, Min(0.01f)] private float duration = 0.35f;

        [Tooltip("위치가 튀는 최대 거리.")]
        [SerializeField, Min(0f)] private float strength = 0.4f;

        [Tooltip("초당 흔들리는 횟수. 클수록 부들부들, 작을수록 크게 출렁인다.")]
        [SerializeField, Min(1)] private int vibrato = 14;

        [Header("Tilt")]
        [Tooltip("좌우로 기울어지는 최대 각도. 0이면 기울이지 않는다.")]
        [SerializeField, Min(0f)] private float tiltAngle = 3f;

        private Vector3 _originalPosition;
        private Vector3 _originalRotation;
        private bool _hasOriginal;

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        public override void CreateFeedBack()
        {
            if (targetCamera == null)
                return;

            Transform cam = targetCamera.transform;

            // 흔들리는 도중 또 맞으면 원래 자리 기준으로 처음부터 다시 흔든다.
            // 흔들린 위치를 기준으로 잡으면 맞을 때마다 카메라가 조금씩 밀려난다.
            StopFeedBack();

            _originalPosition = cam.localPosition;
            _originalRotation = cam.localEulerAngles;
            _hasOriginal = true;

            // fadeOut이 켜져 있어 갈수록 약해지다 정확히 원위치에서 끝난다.
            cam.DOShakePosition(duration, new Vector3(strength, strength, 0f), vibrato, fadeOut: true);

            if (tiltAngle > 0f)
                cam.DOShakeRotation(duration, new Vector3(0f, 0f, tiltAngle), vibrato, fadeOut: true);
        }

        public override void StopFeedBack()
        {
            if (targetCamera == null)
                return;

            Transform cam = targetCamera.transform;
            cam.DOKill();

            if (_hasOriginal)
            {
                cam.localPosition = _originalPosition;
                cam.localEulerAngles = _originalRotation;
            }
        }
    }
}
