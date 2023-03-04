using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Hali_Framework
{
    public class ResMgr : Singleton<ResMgr>, IModule
    {
        //资源容器
        //避免重复加载，需要在合适时机释放
        private readonly Dictionary<string, Dictionary<string, object>> _resDic;
        private readonly Dictionary<string, Queue<Action<object>>> _loadingDic;
        private readonly List<string> _toReleaseList;

        public int Priority => 0;
        
        public ResMgr()
        {
            _resDic = new Dictionary<string, Dictionary<string, object>>();
            _loadingDic = new Dictionary<string, Queue<Action<object>>>();
            _toReleaseList = new List<string>();
        }
        
        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            ClearAllRes();
        }

        /// <summary>
        /// 加载资源(同步，不缓存)
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string path) where T : Object
        {
            T res = Resources.Load<T>(path);
            //如果是GameObject，先实例化再返回
            return res is GameObject ? Object.Instantiate(res) : res;
        }

        /// <summary>
        /// 加载资源(同步，缓存)
        /// </summary>
        /// <param name="part"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string part, string path) where T : Object
        {
            T res;
            if(!_resDic.ContainsKey(part))
                _resDic.Add(part, new Dictionary<string, object>());
            if (_resDic[part].ContainsKey(path))
                res = _resDic[part][path] as T;
            else
            {
                res = Resources.Load<T>(path);
                
                if (res == null)
                    throw new Exception($"<Load Error> No path: {path}.");
                _resDic[part].Add(path, res);
            }

            return res is GameObject ? Object.Instantiate(res) : res;
        }

        
        /// <summary>
        /// 加载资源(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : Object
        {
            MonoMgr.Instance.StartCoroutine(AsyncLoadCoroutine(path, callback));
        }

        /// <summary>
        /// 加载资源(异步，缓存)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadAsync<T>(string part, string path, UnityAction<T> callback = null) where T : Object
        {
            MonoMgr.Instance.StartCoroutine(AsyncLoadCoroutine(part, path, callback));
        }

        public void ClearPartRes(string part)
        {
            if(_resDic != null && _resDic.ContainsKey(part))
                _resDic[part].Clear();
        }
        
        public void ClearAllRes() => _resDic.Clear();

        
        
        #region 异步协程

        private IEnumerator AsyncLoadCoroutine<T>(string path, UnityAction<T> callback) where T : Object
        {
            if (_loadingDic.ContainsKey(path))
            {
                _loadingDic[path].Enqueue(obj => { callback?.Invoke(obj as T); });
                yield break;
            }
            _loadingDic.Add(path, new Queue<Action<object>>());
            _loadingDic[path].Enqueue(obj =>
            {
                callback?.Invoke(obj as T);
            });
            
            var rr = Resources.LoadAsync<T>(path);
            while(!rr.isDone)
                yield return rr.progress;
            var res = rr.asset as T;

            if (res == null)
                throw new Exception($"<Load Error> No path: {path}.");

            while (_loadingDic[path].Count > 0)
            {
                _loadingDic[path].Dequeue()?.Invoke(res is GameObject
                    ? Object.Instantiate(res)
                    : res);
            }
            _loadingDic.Remove(path);
        }

        private IEnumerator AsyncLoadCoroutine<T>(string part, string path, UnityAction<T> callback) where T : Object
        {
            if (_loadingDic.ContainsKey(path))
            {
                _loadingDic[path].Enqueue(obj => { callback?.Invoke(obj as T); });
                yield break;
            }
            //开始加载，加入加载list
            _loadingDic.Add(path, new Queue<Action<object>>());
            _loadingDic[path].Enqueue(obj => { callback?.Invoke(obj as T); });
            
            T res = null;
            if(!_resDic.ContainsKey(part))
                _resDic.Add(part, new Dictionary<string, object>());
            if (_resDic[part].ContainsKey(path))
                res = _resDic[part][path] as T;
            else
            {
                _resDic[part].Add(path, null);
                var rr = Resources.LoadAsync<T>(path);
                while(!rr.isDone)
                    yield return rr.progress;
                res = rr.asset as T;
                _resDic[part][path] = res;
            }

            while (_loadingDic[path].Count > 0)
            {
                _loadingDic[path].Dequeue()?.Invoke(res is GameObject
                    ? Object.Instantiate(res)
                    : res);
            }
            _loadingDic.Remove(path);
        }

        #endregion
    }
}
