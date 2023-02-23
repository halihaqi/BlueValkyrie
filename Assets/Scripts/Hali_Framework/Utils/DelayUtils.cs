using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    public delegate void DelayCallback(object param);
    
    /// <summary>
    /// 全局延迟执行类，
    /// 同一方法只能存在一个延迟执行，重复添加会覆盖上一次执行,
    /// repeat 小于等于0为无限重复
    /// </summary>
    public class DelayUtils : SinletonAutoMono<DelayUtils>
    {
        private Dictionary<DelayCallback, DelayItem> _toAdd;
        private Dictionary<DelayCallback, DelayItem> _delayDic;
        private List<DelayItem> _toRemove;
        
        private Queue<DelayItem> _pool;

        private float _deltaTime;

        private DelayUtils()
        {
            _toAdd = new Dictionary<DelayCallback, DelayItem>();
            _toRemove = new List<DelayItem>();
            _delayDic = new Dictionary<DelayCallback, DelayItem>();
            _pool = new Queue<DelayItem>();
        }

        public void Delay(float delayTime, int repeat, DelayCallback callback, object param = null)
        {
            if (callback == null)
            {
                Debug.LogWarning("Delay callback is null.");
                return;
            }

            DelayItem item;
            if (_delayDic.TryGetValue(callback, out item))
            {
                item.Set(delayTime, repeat, callback, param);
                item.elapsed = 0;
                item.deleted = false;
                return;
            }

            if (_toAdd.TryGetValue(callback, out item))
            {
                item.Set(delayTime, repeat, callback, param);
                return;
            }

            item = PopFromPool();
            item.Set(delayTime, repeat, callback, param);
            _toAdd.Add(callback, item);
        }

        public void Loop(float delayTime, DelayCallback callback, object param = null)
        {
            Delay(delayTime, 0, callback, param);
        }

        public void Remove(DelayCallback callback)
        {
            DelayItem item;
            if (_toAdd.TryGetValue(callback, out item))
            {
                _toAdd.Remove(callback);
                PushToPool(item);
            }

            if (_delayDic.TryGetValue(callback, out item))
                item.deleted = true;
        }

        private void Update()
        {
            _deltaTime = Time.unscaledDeltaTime;
            Dictionary<DelayCallback, DelayItem>.Enumerator iter;

            //延时字典轮询
            if (_delayDic.Count > 0)
            {
                iter = _delayDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    DelayItem item = iter.Current.Value;
                    if (item.deleted)
                    {
                        _toRemove.Add(item);
                        continue;
                    }

                    item.elapsed += _deltaTime;
                    if(item.elapsed < item.delayTime)
                        continue;

                    if (item.elapsed is < 0 or > 0.03f)
                        item.elapsed = 0;

                    if (item.realRepeat > 0)
                    {
                        --item.realRepeat;
                        if (item.realRepeat == 0)
                        {
                            item.deleted = true;
                            _toRemove.Add(item);
                        }
                    }
                    item.callback?.Invoke(item.param);
                }
                iter.Dispose();
            }
            
            //移除toRemove列表元素
            if (_toRemove.Count > 0)
            {
                for (int i = 0; i < _toRemove.Count; i++)
                {
                    var item = _toRemove[i];
                    if (item.deleted && item.callback != null)
                    {
                        _delayDic.Remove(item.callback);
                        PushToPool(item);
                    }
                }
                _toRemove.Clear();
            }
            
            //添加toAdd元素进delay字典
            //延时的方法要到下一帧才开始计时
            if (_toAdd.Count > 0)
            {
                iter = _toAdd.GetEnumerator();
                while (iter.MoveNext())
                    _delayDic.Add(iter.Current.Key, iter.Current.Value);
                iter.Dispose();
                _toAdd.Clear();
            }
        }

        private DelayItem PopFromPool()
        {
            DelayItem item;
            if (_pool.Count > 0)
                item = _pool.Dequeue();
            else
                item = new DelayItem();

            return item;
        }

        private void PushToPool(DelayItem item)
        {
            item.Recycle();
            _pool.Enqueue(item);
        }

        class DelayItem
        {
            public float delayTime;
            public int configRepeat;
            public DelayCallback callback;
            public object param;

            public float elapsed;
            public int realRepeat;
            public bool deleted;

            public void Set(float delayTime, int repeat, DelayCallback callback, object param)
            {
                this.delayTime = delayTime;
                this.configRepeat = this.realRepeat = repeat;
                this.callback = callback;
                this.param = param;
            }

            public void Recycle()
            {
                callback = null;
                elapsed = 0;
                realRepeat = 0;
                deleted = false;
            }
        }
    }
}