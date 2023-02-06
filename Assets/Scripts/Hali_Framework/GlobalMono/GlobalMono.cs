using System;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 全局Mono脚本，外部不调用，通过MonoMgr使用
    /// </summary>
    public class GlobalMono : MonoBehaviour
    {
        private event Action UpdateEvent;
        private event Action<float, float> UpdateEventWithTime;

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
            UpdateEventWithTime?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        public void AddUpdateListener(Action action)
        {
            UpdateEvent -= action;
            UpdateEvent += action;
        }
        
        public void AddUpdateListener(Action<float, float> action)
        {
            UpdateEventWithTime -= action;
            UpdateEventWithTime += action;
        }
        
        public void RemoveUpdateListener(Action action)
        {
            UpdateEvent -= action;
        }
        
        public void RemoveUpdateListener(Action<float, float> action)
        {
            UpdateEventWithTime -= action;
        }
        
        public void Clear()
        {
            UpdateEvent = null;
            UpdateEventWithTime = null;
        }

        internal void Dispose()
        {
            Destroy(this);
        }
    }
}
