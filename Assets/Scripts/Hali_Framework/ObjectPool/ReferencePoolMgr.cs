using System.Collections.Generic;

namespace Hali_Framework
{
    public class ReferencePoolMgr : Singleton<ReferencePoolMgr>, IModule
    {
        private readonly Dictionary<string, ReferencePool> _referencePoolCollection;

        public int Priority => 0;
        
        public ReferencePoolMgr() => _referencePoolCollection = new Dictionary<string, ReferencePool>();
        
        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            ClearReferencePool();
        }
        
        /// <summary>
        /// 从引用池取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopRef<T>() where T : class, IReference, new()
        {
            string poolName = typeof(T).Name;
            if (_referencePoolCollection.ContainsKey(poolName) && _referencePoolCollection[poolName].PoolNum > 0)
                return (T) _referencePoolCollection[poolName].Pop();
            else
                return new T();
        }

        /// <summary>
        /// 压入引用池
        /// </summary>
        /// <param name="obj">引用实例</param>
        /// <typeparam name="T"></typeparam>
        public void PushRef<T>(T obj) where T : class, IReference, new()
        {
            string poolName = typeof(T).Name;
            if(_referencePoolCollection.ContainsKey(poolName))
                _referencePoolCollection[poolName].Push(obj);
            else
                _referencePoolCollection.Add(poolName, new ReferencePool(obj));
        }

        /// <summary>
        /// 清空引用池
        /// </summary>
        public void ClearReferencePool()
        {
            _referencePoolCollection.Clear();
        }
    }
}