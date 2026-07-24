using CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private SupplyPlane itemPlane;

        public void CallPlane()
        {
            itemPlane.CallPlane();
        }
    }
}