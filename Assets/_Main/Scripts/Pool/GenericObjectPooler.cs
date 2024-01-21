using System.Collections.Generic;
using UnityEngine;

namespace ObjPooler.Scripts
{
    public abstract class GenericObjectPooler<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private T prefab;
        public static GenericObjectPooler<T> Instance { get; private set; }

        private Queue<T> _objects = new Queue<T>();

        private void Awake()
        {
            Instance = this;
        }

        public T GetPrefab()
        {
            return prefab;
        }

        public T Get()
        {
            if (_objects.Count == 0)
                AddObjects(1);
            return _objects.Dequeue();
        }

        public void ReturnToPool(T objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            _objects.Enqueue(objectToReturn);
        }

        private void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = GameObject.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                _objects.Enqueue(newObject);    
            }
            
        }
    }
    
}
