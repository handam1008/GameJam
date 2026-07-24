using System.Collections;
using UnityEngine;

namespace _Work.PAP.Scripts.FeedbackSystem
{
    public class BlinkFeedback : AbstractFeedBack
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float blinkDruation = 0.15f;
        [SerializeField] private int count = 1;

        private MaterialPropertyBlock _mpb;
        
        private readonly int BLINK_HASH = Shader.PropertyToID("_BlinkValue");
        private WaitForSeconds _waitForSeconds;
        private Coroutine _blinkCoroutine = null;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(_mpb); //해당 렌더러에 붙어있는 메테리얼을 기반으로 mpb를 채워준다.
            _waitForSeconds = new WaitForSeconds(blinkDruation);
        }

        public override void CreateFeedBack()
        {
            _blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }

        private IEnumerator BlinkCoroutine()
        {
            for (int i = 0; i < count; i++)
            {
                _mpb.Clear();
                _mpb.SetFloat(BLINK_HASH, 0.4f);
                targetRenderer.SetPropertyBlock(_mpb);
                yield return _waitForSeconds;
                _mpb.SetFloat(BLINK_HASH, 0f);
                targetRenderer.SetPropertyBlock(_mpb);
                yield return _waitForSeconds;
            }
        }

        public override void StopFeedBack()
        {
            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);
        }
    }
}