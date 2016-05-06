using System;
using System.Reflection;
using UnityEngine;

namespace FZ
{
    /**
    *@brief 싱글톤 추상 클래스
    *@details 템플릿 T에 해당하는 클래스가 이 추상 클래스를 상속받을 시 싱글톤 클래스로 구현된다.
    *반드시 생성자는 private로 지정해 주어야 한다.
    *@author Delight
    */
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

    /**
    *@brief 모노 싱글톤 추상 클래스
    *@details 템플릿 T에 해당하는 클래스가 이 추상 클래스를 상속받을 시 모노 싱글톤 클래스로 구현된다.
    * MonoBehaviour를 상속받아야 하는 클래스가 싱글톤으로 구현되어야 할 시 이 추상 클래스를 상속받는다.
    *@author Delight
    */
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
        /**
        *@brief 모노 싱글톤의 오브젝트 이름
        *@details 싱글톤 객체가 생성될 때 생성되는 GameObject의 객체 이름을 리턴한다.
        */
        protected abstract string CreatedObjectName();
    }
}