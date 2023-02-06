using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    public class PoolCollectionEntity : MonoBehaviour
    {
        [System.Serializable]
        private class PoolInfo
        {
            public string poolName;
            public int cacheNum;

            public PoolInfo(string name, int num) => (poolName, cacheNum) = (name, num);
        }
        
        private Dictionary<string, ObjectPool> _poolDic = new Dictionary<string, ObjectPool>();

        [SerializeField] private List<PoolInfo> poolInfos = new List<PoolInfo>();

        private void Start()
        {
            DontDestroyOnLoad(this);
            poolInfos.Clear();
            foreach (var pool in _poolDic)
                poolInfos.Add(new PoolInfo(pool.Key, pool.Value.CacheNum));
            EventMgr.Instance.AddListener(ClientEvent.POOL_CHANGED, UpdateInfo);
        }

        private void UpdateInfo()
        {
            poolInfos.Clear();
            foreach (var pool in _poolDic)
                poolInfos.Add(new PoolInfo(pool.Key, pool.Value.CacheNum));
        }

        internal void SetPoolCollection(Dictionary<string, ObjectPool> poolCollection)
            => _poolDic = poolCollection;
    }
}