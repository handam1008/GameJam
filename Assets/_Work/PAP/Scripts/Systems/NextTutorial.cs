using _Work.PAP.Scripts.TutorialSystem;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class NextTutorial : MonoBehaviour
    {
        public void Call()
        {
            MainTutorial.Instance.Next();
        }
    }
}