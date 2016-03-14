using System;
using System.Reflection;

namespace MCN
{
    public class Singletone<T> where T : class
    {
        private static T _instance = null;

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
}