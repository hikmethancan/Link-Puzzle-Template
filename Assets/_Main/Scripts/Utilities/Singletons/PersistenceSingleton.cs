using UnityEngine;

namespace _Main.Scripts.Utilities.Singletons
{
    public class PersistenceSingleton<T> : SingletonBase where T : Component
    {
        public static T Instance { get; private set; } = null;
        
        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}