using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _Work.PAP.Scripts.FeedbackSystem
{
    public class PlaySoundFeedback : AbstractFeedBack
    {
        [SerializeField] private SoundSo soundData;
        public override void CreateFeedBack()
        {
            SoundManager.Instance.PlaySound(soundData.soundName);
        }

        public override void StopFeedBack()
        {
        }
    }
}