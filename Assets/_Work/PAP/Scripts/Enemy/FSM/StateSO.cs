using UnityEngine;

namespace Enemies.FSM
{
    [CreateAssetMenu(fileName = "State data", menuName = "Agent/StateData", order = 0)]
    public class StateSO : ScriptableObject
    {
        public string StateName;
    }
}