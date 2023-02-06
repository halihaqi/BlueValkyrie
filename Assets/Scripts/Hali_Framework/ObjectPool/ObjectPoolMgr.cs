using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hali_Framework
{
    public class ObjectPoolMgr : Singleton<ObjectPoolMgr>, IModule
    {
        private readonly Dictionary<string, ObjectPool> _objectPoolCollection;
        private GameObject _poolCollectionObj;

        public int Priority => 0;
        
        public ObjectPoolMgr()
            => _objectPoolCollection = new Dictionary<string, ObjectPool>();
        
        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            Clear();
        }

        /// <summary>
        /// 从对象池取出
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="callback"></param>
        public void PopObj(string path, Action<GameObject> callback)
        {
            if (_objectPoolCollection.ContainsKey(path) && _objectPoolCollection[path].CacheNum > 0)
            {
                GameObject obj = _objectPoolCollection[path].Pop();
                callback?.Invoke(obj);
            }
            else
            {
                ResMgr.Instance.LoadAsync<GameObject>(path, (obj) =>
                {
                    obj.name = path;
                    callback?.Invoke(obj);
                });
            }
        }


        /// <summary>
        /// 压入对象池
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="obj">物体实例</param>
        public void PushObj(string path, GameObject obj)
        {
            Debug.Log($"Push {obj}");
            if (_objectPoolCollection.ContainsKey(path))
            {
                _objectPoolCollection[path].Push(obj);
            }
            else
            {
                if (_poolCollectionObj == null)
                {
                    _poolCollectionObj = new GameObject("Pool");
                    _poolCollectionObj.AddComponent<PoolCollectionEntity>().
                        SetPoolCollection(_objectPoolCollection);
                }
                _objectPoolCollection.Add(path, new ObjectPool(path, obj, _poolCollectionObj));
            }
        }

        /// <summary>
        /// 获得对象池缓存的对象数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetPoolCacheNum(string path)
        {
            if (_objectPoolCollection.ContainsKey(path))
                return _objectPoolCollection[path].CacheNum;
            else
                return 0;
        }

        /// <summary>
        /// 清空单个对象池
        /// </summary>
        /// <param name="path"></param>
        public void ClearPool(string path)
        {
            if(!_objectPoolCollection.ContainsKey(path)) return;

            _objectPoolCollection[path].Clear();
        }
        
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void Clear()
        {
            _objectPoolCollection.Clear();
            Object.Destroy(_poolCollectionObj);
            _poolCollectionObj = null;
        }
    }
}