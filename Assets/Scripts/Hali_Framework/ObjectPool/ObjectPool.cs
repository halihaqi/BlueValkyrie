using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hali_Framework
{
        public class ObjectPool
        {
            private GameObject _parentObj;
            private readonly Queue<GameObject> _poolList;
            
            /// <summary>
            /// 池中对象缓存数
            /// </summary>
            public int CacheNum { get; private set; }
            
            /// <summary>
            /// 对象池名
            /// </summary>
            public string PoolName { get; private set; }

            /// <summary>
            /// 对象池构造函数
            /// </summary>
            /// <param name="poolName">对象池名</param>
            /// <param name="firstObj">第一个对象</param>
            /// <param name="poolObj">对象池实例</param>
            public ObjectPool(string poolName, GameObject firstObj, GameObject poolObj)
            {
                this._parentObj = new GameObject(poolName);
                PoolName = poolName;
                _parentObj.transform.parent = poolObj.transform;
                _poolList = new Queue<GameObject>();
                Push(firstObj);
            }

            public GameObject Pop()
            {
                GameObject obj = _poolList.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null, false);
                --CacheNum;
                EventMgr.Instance.TriggerEvent(ClientEvent.POOL_CHANGED);
                return obj;
            }
            
            public void Push(GameObject obj)
            {
                if (_poolList.Contains(obj))
                    throw new Exception($"Can not push {obj} twice in pool:{PoolName}.");
                obj.SetActive(false);
                obj.transform.SetParent(_parentObj.transform, false);
                _poolList.Enqueue(obj);
                ++CacheNum;
                EventMgr.Instance.TriggerEvent(ClientEvent.POOL_CHANGED);
            }
            
            public void Clear()
            {
                Object.Destroy(_parentObj);
                _parentObj = null;
                _poolList.Clear();
                CacheNum = 0;
                EventMgr.Instance.TriggerEvent(ClientEvent.POOL_CHANGED);
            }
        }
}