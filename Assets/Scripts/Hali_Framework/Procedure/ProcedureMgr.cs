using System;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 游戏流程管理类
    /// </summary>
    public class ProcedureMgr : Singleton<ProcedureMgr>, IModule
    {
        private IFsm<ProcedureMgr> _procedureFsm;

        public int Priority => 0;

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _procedureFsm = null;
        }

        public ProcedureBase CurProcedure
        {
            get
            {
                if (_procedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return null;
                }

                return (ProcedureBase)_procedureFsm.CurrentState;
            }
        }
        
        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (_procedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return -1;
                }

                return _procedureFsm.CurrentStateTime;
            }
        }
        
        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        /// <param name="procedures">流程管理器包含的流程。</param>
        public void Initialize(params ProcedureBase[] procedures)
        {
            _procedureFsm = FsmMgr.Instance.CreateFsm("ProcedureFsm", this, procedures);
        }
        
        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return;
            }

            _procedureFsm.Start<T>();
        }
        
        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return false;
            }

            return _procedureFsm.HasState<T>();
        }
        
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return null;
            }

            return _procedureFsm.GetState<T>();
        }

        /// <summary>
        /// 设置流程数据。
        /// </summary>
        /// <typeparam name="TData">数据的类型。</typeparam>
        /// <param name="name">数据名称。</param>
        /// <param name="data">要获取的数据。</param>
        public void SetData<TData>(string name, TData data)
            => _procedureFsm.SetData(name, data);

        /// <summary>
        /// 移除流程数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>是否移除数据成功。</returns>
        public bool RemoveData(string name)
            => _procedureFsm.RemoveData(name);

        /// <summary>
        /// 获取流程数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>要获取的数据。</returns>
        public TData GetData<TData>(string name)
            => _procedureFsm.GetData<TData>(name);

        /// <summary>
        /// 改变当前流程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>() where T : ProcedureBase
            => _procedureFsm.ChangeState<T>();
    }
}