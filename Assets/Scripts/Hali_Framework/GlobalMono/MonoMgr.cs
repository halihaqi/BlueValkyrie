using System;
using System.Collections;
using UnityEngine;

namespace Hali_Framework
{
    public class MonoMgr : Singleton<MonoMgr>, IModule
    {
        private GlobalMono _globalMono;

        //公共Mono最先启动
        public int Priority => 0;

        void IModule.Init()
        {
            GameObject obj = new GameObject("GlobalMono");
            //GlobalMono的Start()中有过场景不销毁
            _globalMono = obj.AddComponent<GlobalMono>();
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _globalMono.Dispose();
        }

        #region Update相关公共方法
        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        public void AddUpdateListener(Action action)
        {
            _globalMono.AddUpdateListener(action);
        }
        
        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        public void AddUpdateListener(Action<float, float> action)
        {
            _globalMono.AddUpdateListener(action);
        }

        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        public void RemoveUpdateListener(Action action)
        {
            _globalMono.RemoveUpdateListener(action);
        }
        
        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        public void RemoveUpdateListener(Action<float, float> action)
        {
            _globalMono.RemoveUpdateListener(action);
        }

        /// <summary>
        /// 清空Update事件监听
        /// </summary>
        public void Clear()
        {
            _globalMono.Clear();
        }
        #endregion

        #region Coroutine相关公共方法
        //开启协程
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _globalMono.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return _globalMono.StartCoroutine(methodName);
        }

        //停止协程
        public void StopCoroutine(IEnumerator routine)
        {
            _globalMono.StopCoroutine(routine);
        }

        public void StopCoroutine(string methodName)
        {
            _globalMono.StopCoroutine(methodName);
        }

        //停止所有协程
        public void StopAllCoroutines()
        {
            _globalMono.StopAllCoroutines();
        }
        #endregion
        
    }
}
