using UnityEditor;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy.Editor
{
    [CustomEditor(typeof(AttackPatternSO))]
    public class AttackPatternSOEditor : UnityEditor.Editor
    {
        private const float CellSize = 22f;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty damageProp = serializedObject.FindProperty("damage");
            SerializedProperty range1Prop = serializedObject.FindProperty("range");
            SerializedProperty rangeProp = serializedObject.FindProperty("patternSize");
            SerializedProperty primaryProp = serializedObject.FindProperty("primary");
            SerializedProperty animationProp = serializedObject.FindProperty("AnimationName");
            SerializedProperty hashProp = serializedObject.FindProperty("AnimationHash");
            SerializedProperty durationProp = serializedObject.FindProperty("duration");
            SerializedProperty postDelayProp = serializedObject.FindProperty("postDelay");
            SerializedProperty patternProp = serializedObject.FindProperty("_pattern");

            EditorGUILayout.PropertyField(damageProp);
            EditorGUILayout.PropertyField(range1Prop);
            EditorGUILayout.PropertyField(rangeProp);
            EditorGUILayout.PropertyField(primaryProp);
            EditorGUILayout.PropertyField(animationProp);
            EditorGUILayout.PropertyField(hashProp);
            EditorGUILayout.PropertyField(durationProp);
            EditorGUILayout.PropertyField(postDelayProp);

            int newRange = Mathf.Max(1, rangeProp.intValue);
            rangeProp.intValue = newRange;

            int oldRange = Mathf.RoundToInt(Mathf.Sqrt(patternProp.arraySize));
            if (oldRange != newRange) ResizePattern(patternProp, oldRange, newRange);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Attack Pattern", EditorStyles.boldLabel);

            for (int y = newRange - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                for (int x = 0; x < newRange; x++)
                {
                    SerializedProperty cell = patternProp.GetArrayElementAtIndex(y * newRange + x);
                    cell.boolValue = GUILayout.Toggle(cell.boolValue, GUIContent.none, GUI.skin.button,
                        GUILayout.Width(CellSize), GUILayout.Height(CellSize));
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>range가 바뀌어도 기존 칸의 (x, y) 값을 그대로 유지하며 배열 크기를 다시 맞춘다.</summary>
        private static void ResizePattern(SerializedProperty patternProp, int oldRange, int newRange)
        {
            bool[] oldValues = new bool[patternProp.arraySize];
            for (int i = 0; i < oldValues.Length; i++) oldValues[i] = patternProp.GetArrayElementAtIndex(i).boolValue;

            patternProp.arraySize = newRange * newRange;

            for (int y = 0; y < newRange; y++)
            {
                for (int x = 0; x < newRange; x++)
                {
                    bool value = x < oldRange && y < oldRange && oldValues.Length > 0
                        ? oldValues[y * oldRange + x]
                        : false;
                    patternProp.GetArrayElementAtIndex(y * newRange + x).boolValue = value;
                }
            }
        }
    }
}
