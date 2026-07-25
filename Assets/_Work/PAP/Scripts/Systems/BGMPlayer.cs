using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private SoundSo soundData;

        private void Start()
        {
            SoundManager.Instance.PlaySound(soundData.soundName);
        }
    }
}