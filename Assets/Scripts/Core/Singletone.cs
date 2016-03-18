using System;
using System.Reflection;
using UnityEngine;

namespace MCN
{
    public abstract class Singletone<T> where T : class
    {
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance();
                }
                return _instance;
            }
        }

        private static void CreateInstance()
        {
            if (_instance == null)
            {
                Type t = typeof(T);

                ConstructorInfo[] ctors = t.GetConstructors();

                if (ctors.Length > 0)
                {
                    throw new InvalidOperationException(String.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
                }

                _instance = (T)Activator.CreateInstance(t, true);
            }
        }
    }

    public abstract class MonoSingletone<T> : MonoBehaviour where T : MonoSingletone<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                }

                if (_instance == null)
                {
                    GameObject obj = new GameObject("");
                    _instance = obj.AddComponent(typeof(T)) as T;
                    _instance.name = _instance.CreatedObjectName();
                }

                return _instance;
            }
        }

        protected abstract string CreatedObjectName();
    }
}