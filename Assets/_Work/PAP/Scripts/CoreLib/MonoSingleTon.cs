using System;
using UnityEngine;

namespace CoreLib
{
    public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                }// 먼저 같은 타입에 싱글톤이 있는지 확인한다.

                if (_instance is null) //없으면 만들어서 준다.
                {
                    string objectName = typeof(T).ToString();
                    GameObject instanceGo = new GameObject(objectName);
                    _instance = instanceGo.AddComponent<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);
            if (managers.Length > 1)
            {
                Destroy(gameObject); //이미 해당 씬에 다른 매니저가 있다면 나는 파괴.
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) //내가 싱클톤이었다면 싱글톤을 리셋한다.
            {
                _instance = null;
            }
        }
        
    }
}