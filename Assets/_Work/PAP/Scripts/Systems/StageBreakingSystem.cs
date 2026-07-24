using System;
using CoreLib;
using CSILib.SoundManager.RunTime;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Work.PAP.Scripts.Systems
{
    public class StageBreakingSystem : MonoSingleton<StageBreakingSystem>
    {
        [SerializeField] private int reachToClear = 50;
        private int currentReach = 0;
        private SpriteRenderer _sr;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private SpriteRenderer noiseMat;
        [SerializeField] private float targetTimeScale = 2f;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private int targetScene;
        MaterialPropertyBlock _mpb;
        private readonly int TIMESCALE_HASH = Shader.PropertyToID("_TimeScale");

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _mpb = new MaterialPropertyBlock();
            noiseMat.GetPropertyBlock(_mpb);
            
        }

        private void Start()
        {
            Time.timeScale = 0f;
            CameraEffectManager.Instance.ZoomAsync(16f,1f).ContinueWith(() => CameraEffectManager.Instance.ZoomAsync(10f,0.5f));
            canvasGroup.alpha = 1f;
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(1f);
            seq.Append(canvasGroup.DOFade(0f, 0.5f));
            seq.AppendCallback(() =>
            {
                Time.timeScale = 1f;
            });
            seq.SetUpdate(true);
            seq.Play();
            
        }

        public void AddReach(int count)
        {
            currentReach += count;
            float reach = _curve.Evaluate((float)currentReach / (float)reachToClear);
            _mpb.SetFloat(TIMESCALE_HASH, reach * targetTimeScale);
            noiseMat.SetPropertyBlock(_mpb);
            _sr.DOColor(new Color(0, 0, 0, 1 - reach),1f);
            if (currentReach >= reachToClear)
            {
                Time.timeScale = 0;
                canvasGroup.DOFade(1f, 1f).SetUpdate(true).OnComplete(() =>
                {
                    DOTween.KillAll();
                    SceneManager.LoadScene(targetScene);
                });
                CameraEffectManager.Instance.ZoomAsync(16f,1f).Forget();
            }
        }
    }
}