using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FZ
{
    /**
    *@brief 싱글톤 추상 클래스
    *@details 템플릿 T에 해당하는 클래스가 이 추상 클래스를 상속받을 시 싱글톤 클래스로 구현된다.
    *반드시 생성자는 private로 지정해 주어야 한다.
    *@author Delight
    */
    public abstract class Singleton<T> where T : class
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
    *@brief 제네럴 싱글톤 추상 클래스
    *@details 하나의 클래스가 여러 가짓수의 클래스를 관리할 때 사용한다. T가 총괄하는 클래스, T2가 관리할 클래스들의 상위 클래스
    *반드시 생성자는 private로 지정해 주어야 한다.
    *@author Delight
    */
    public abstract class GeneralSingleton<T, T2> where T2 : class
    {
        private static T _instance;
        private static Dictionary<Type, T2> _handlers;

        public static T3 Get<T3>() where T3 : T2
        {
            if (_handlers == null)
            {
                _handlers = new Dictionary<Type, T2>();

                CreateInstance();
            }
            return (T3)_handlers[typeof(T3)];
        }

        protected static void RegisterHandler(T2 handler)
        {
            if (_handlers == null)
            {
                _handlers = new Dictionary<Type, T2>();

                CreateInstance();
            }
            GeneralSingleton<T, T2>._handlers.Add(handler.GetType(), handler);
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
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
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

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}