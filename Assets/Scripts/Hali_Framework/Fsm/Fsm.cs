using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    public class Fsm<T> : FsmBase, IFsm<T> where T : class
    {
        private T _mOwner;
        private readonly Dictionary<Type, FsmState<T>> _mStates;
        private Dictionary<string, object> _mDatas;
        private FsmState<T> _mCurrentState;
        private float _mCurrentStateTime;
        private bool _mIsDestroyed;
        
        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        public Fsm()
        {
            _mOwner = null;
            _mStates = new Dictionary<Type, FsmState<T>>();
            _mDatas = null;
            _mCurrentState = null;
            _mCurrentStateTime = 0f;
            _mIsDestroyed = true;
        }

        public T Owner => _mOwner;
        
        public override Type OwnerType => typeof(T);
        
        public override int FsmStateCount => _mStates.Count;

        public override bool IsRunning => _mCurrentState != null;
        
        public override bool IsDestroyed => _mIsDestroyed;

        public FsmState<T> CurrentState => _mCurrentState;

        public override string CurrentStateName => _mCurrentState?.GetType().FullName;
        
        public override float CurrentStateTime => _mCurrentStateTime;

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(string name, T owner, params FsmState<T>[] states)
        {
            if (owner == null)
            {
                Debug.LogError("FSM owner is invalid.");
                return null;
            }

            if (states == null || states.Length < 1)
            {
                Debug.LogError("FSM owner is invalid.");
                return null;
            }
            
            Fsm<T> fsm = new Fsm<T>();
            fsm.Name = name;
            fsm._mOwner = owner;
            fsm._mIsDestroyed = false;
            foreach (var state in states)
            {
                if (state == null)
                {
                    Debug.LogError("FSM owner is invalid.");
                    return null;
                }

                Type stateType = state.GetType();
                if (fsm._mStates.ContainsKey(stateType))
                {
                    Debug.LogError($"FSM '{typeof(T).Name}' state '{stateType.FullName}' is already exist.");
                    return null;
                }
                
                fsm._mStates.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(string name, T owner, List<FsmState<T>> states)
        {
            if (owner == null)
            {
                Debug.Log("FSM owner is invalid.");
                return null;
            }

            if (states == null || states.Count < 1)
            {
                Debug.Log("FSM owner is invalid.");
                return null;
            }
            
            Fsm<T> fsm = new Fsm<T>();
            fsm.Name = name;
            fsm._mOwner = owner;
            fsm._mIsDestroyed = false;
            foreach (var state in states)
            {
                if (state == null)
                {
                    Debug.Log("FSM owner is invalid.");
                    return null;
                }

                Type stateType = state.GetType();
                if (fsm._mStates.ContainsKey(stateType))
                {
                    Debug.Log($"FSM '{name}' state '{stateType.FullName}' is already exist.");
                    return null;
                }
                
                fsm._mStates.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        public void Clear()
        {
            _mCurrentState?.OnLeave(this, true);
            foreach (var fsmState in _mStates.Values)
            {
                fsmState.OnDestroy(this);
            }

            Name = null;
            _mOwner = null;
            _mStates.Clear();
            _mDatas.Clear();
            _mCurrentState = null;
            _mCurrentStateTime = 0;
            _mIsDestroyed = true;
        }
        
        
        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : FsmState<T>
        {
            if (IsRunning)
            {
                Debug.Log("FSM is running, can not start again.");
                return;
            }

            var state = GetState<TState>();
            if (state == null)
            {
                Debug.Log($"FSM '{Name}' can not start state '{typeof(TState).FullName}' which is not exist.");
                return;
            }

            _mCurrentStateTime = 0;
            _mCurrentState = state;
            _mCurrentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FsmState<T>
        {
            return _mStates.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : FsmState<T>
        {
            FsmState<T> state = null;
            if (_mStates.TryGetValue(typeof(TState), out state))
                return (TState)state;

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public FsmState<T>[] GetAllStates()
        {
            int index = 0;
            FsmState<T>[] results = new FsmState<T>[_mStates.Count];
            foreach (var state in _mStates)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Data name is invalid.");
                return false;
            }

            if (_mDatas == null)
                return false;

            return _mDatas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Data name is invalid.");
                return default;
            }

            if (_mDatas == null)
                return default;

            if (_mDatas.TryGetValue(name, out var data))
                return (TData)data;
            return default;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Data name is invalid.");
                return;
            }

            if (_mDatas == null)
                _mDatas = new Dictionary<string, object>(StringComparer.Ordinal);

            if(!_mDatas.ContainsKey(name)) 
                _mDatas.Add(name, data);
            else
                _mDatas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Data name is invalid.");
                return false;
            }

            if (_mDatas == null)
                return false;

            return _mDatas.Remove(name);
        }
        
        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_mCurrentState == null)
                return;

            _mCurrentStateTime += elapseSeconds;
            _mCurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            _mStates?.Clear();
            _mDatas?.Clear();
        }
        
        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        internal void ChangeState<TState>() where TState : FsmState<T>
        {
            if (_mCurrentState == null)
            {
                Debug.Log("Current state is invalid.");
                return;
            }

            var state = GetState<TState>();
            if (state == null)
            {
                Debug.LogError(
                    $"FSM '{Name}' can not change state to '{typeof(TState).FullName}' which is not exist.");
                return;
            }

            _mCurrentState.OnLeave(this, false);
            _mCurrentStateTime = 0;
            _mCurrentState = state;
            _mCurrentState.OnEnter(this);
        }
    }
}