using System;
using CSILib.SoundManager.RunTime;
using DG.Tweening;
using UnityEngine;

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
        MaterialPropertyBlock _mpb;
        private readonly int TIMESCALE_HASH = Shader.PropertyToID("_TimeScale");

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _mpb = new MaterialPropertyBlock();
            noiseMat.GetPropertyBlock(_mpb);
            
        }

        public void AddReach(int count)
        {
            currentReach += count;
            float reach = _curve.Evaluate((float)currentReach / (float)reachToClear);
            _mpb.SetFloat(TIMESCALE_HASH, reach * targetTimeScale);
            noiseMat.SetPropertyBlock(_mpb);
            _sr.DOColor(new Color(0, 0, 0, 1 - reach),1f);
        }
    }
}