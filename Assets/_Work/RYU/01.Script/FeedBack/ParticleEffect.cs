using UnityEngine;

namespace _Work.RYU._01.Script.FeedBack
{
    public class ParticleEffect : AbstractFeedBack
    {
        [SerializeField] private GameObject _particleEffectPrefab;
        [SerializeField] private Transform _spawnPoint;

        private GameObject _lastSpawned;

        public override void CreateFeedBack()
        {
            
            GameObject go = Instantiate(_particleEffectPrefab, _spawnPoint.position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            ps.Play();
            
        }

        public override void StopFeedBack()
        {
            if (_lastSpawned != null)
                Destroy(_lastSpawned);
        }
    }
}
