using System.Collections.Generic;

namespace Hali_Framework
{
    public class ReferencePool
    {
        private readonly Queue<IReference> _poolList;
        
        public int PoolNum { get; private set; }

        public ReferencePool(IReference obj)
        {
            _poolList = new Queue<IReference>();
            Push(obj);
        }

        public IReference Pop()
        {
            --PoolNum;
            return _poolList.Dequeue();
        }

        public void Push(IReference obj)
        {
            obj.Reset();
            _poolList.Enqueue(obj);
            ++PoolNum;
        }

        public void Clear()
        {
            _poolList.Clear();
            PoolNum = 0;
        }
    }
}