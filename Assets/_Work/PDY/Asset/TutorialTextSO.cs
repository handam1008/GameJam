using UnityEngine;

namespace _Work.PDY.Asset
{
    [CreateAssetMenu(fileName = "Tutorial Text", menuName = "TextSO", order = 0)]
    public class TutorialTextSO : ScriptableObject
    {
        public string text;
    }
}