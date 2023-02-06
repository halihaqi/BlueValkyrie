using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Hali_Framework
{
    public class EventMgr : Singleton<EventMgr>, IModule
    {
        //事件容器，不支持同种名称不同类型的事件监听
        private readonly Dictionary<ClientEvent, Delegate> _eventDic;
        //用于单次监听除重
        private readonly Dictionary<ClientEvent, Delegate> _onceEventDic;
        private static int ALL_EVENT_COUNT = 0;
        
        int IModule.Priority => 0;

        public EventMgr()
        {
            _eventDic = new Dictionary<ClientEvent, Delegate>(25);
            _onceEventDic = new Dictionary<ClientEvent, Delegate>(10);
        }
        
        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _eventDic.Clear();
            _onceEventDic.Clear();
            ALL_EVENT_COUNT = 0;
        }

        #region 单次监听(触发一次自动移除)

        public void OnceListener(ClientEvent name, Action call)
        {
            var oriCall = call;
            if (!_onceEventDic.TryGetValue(name, out var act))
                _onceEventDic.Add(name, oriCall);
            else if(act == (Delegate) oriCall)
                return;
            call += () =>
            {
                RemoveListener(name, oriCall);
                _onceEventDic.Remove(name);
            };
            AddListener(name, call);
        }
    
        public void OnceListener<T>(ClientEvent name, Action<T> call)
        {
            var oriCall = call;
            if (!_onceEventDic.TryGetValue(name, out var act))
                _onceEventDic.Add(name, oriCall);
            else if(act == (Delegate) oriCall)
                return;
            call += t =>
            {
                RemoveListener(name, oriCall);
                _onceEventDic.Remove(name);
            };
            AddListener(name, call);
        }
    
        public void OnceListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            var oriCall = call;
            if (!_onceEventDic.TryGetValue(name, out var act))
                _onceEventDic.Add(name, oriCall);
            else if(act == (Delegate) oriCall)
                return;
            call += (t, u) =>
            {
                RemoveListener(name, oriCall);
                _onceEventDic.Remove(name);
            };
            AddListener(name, call);
        }

        #endregion

        #region 添加监听

        public void AddListener(ClientEvent name, Action call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
    
        public void AddListener<T>(ClientEvent name, Action<T> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
    
        public void AddListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T, U> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }

        #endregion

        #region 移除监听

        public void RemoveListener(ClientEvent name, Action call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
        }
    
        public void RemoveListener<T>(ClientEvent name, Action<T> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
        }
    
        public void RemoveListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T, U> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
        }

        #endregion

        #region 触发事件

        public void TriggerEvent(ClientEvent name)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action act)) continue;
                try
                {
                    act.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
    
        public void TriggerEvent<T>(ClientEvent name, T param1)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T> act)) continue;
                try
                {
                    act.Invoke(param1);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
    
        public void TriggerEvent<T, U>(ClientEvent name, T param1, U param2)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T, U> act)) continue;
                try
                {
                    act.Invoke(param1, param2);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }

        #endregion
    

        /// <summary>
        /// 移除事件所有监听，慎用
        /// </summary>
        /// <param name="name"></param>
        public void RemoveEvent(ClientEvent name)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                ALL_EVENT_COUNT -= fun.GetInvocationList().Length;
                _eventDic.Remove(name);
            }
        }
    
        public void Clear()
        {
            _eventDic.Clear();
        }
    }
}
