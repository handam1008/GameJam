using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Work.PAP.Scripts.Agent
{
    public class AfterImageEffect : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 0.25f;
        private SpriteRenderer _renderer;
        
        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void StartTrail()
        {
            _tokenSource = new CancellationTokenSource();
            TrailAsync().Forget();
        }

        public void StopTrail()
        {
            CancelToken();
        }

        private void OnDestroy()
        {
            CancelToken();
        }

        private void CancelToken()
        {
            if (_tokenSource != null)
            {
                _tokenSource?.Cancel();
                _tokenSource?.Dispose();
                _tokenSource = null;
            }
        }

        private async UniTaskVoid TrailAsync()
        {
            while (true)
            {
                SpawnGhost();
                await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval),cancellationToken: _tokenSource.Token);
            }
        }

        public void SpawnGhost()
        {
            GameObject go = new GameObject("Trail");
            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _renderer.sprite;
            spriteRenderer.color = new Color(1,1,1,0.6f);
            spriteRenderer.sortingLayerName = "Trail";
            go.transform.SetPositionAndRotation(transform.position,transform.rotation);
            go.transform.localScale = transform.localScale;
            spriteRenderer.DOFade(0f, 0.5f).OnComplete(() => Destroy(go));
        }
    }
}