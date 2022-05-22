using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [DisallowMultipleComponent]
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected bool _isRunning { get; private set; }

        public static T Instance
        {
            get { return _instance ??= FindObjectOfType<T>(); }
        }

        private static T _instance;

        public virtual void Awake()
        {
            if (_instance == null || _instance == this)
            {
                if (transform.parent == null) {
                    DontDestroyOnLoad(gameObject);
                }
                _instance = (T) this;
                _isRunning = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            _isRunning = false;
        }
    }
}
