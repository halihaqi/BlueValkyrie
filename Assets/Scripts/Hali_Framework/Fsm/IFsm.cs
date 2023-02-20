namespace Hali_Framework
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    public interface IFsm<T> where T : class
    {
        string Name { get;}

        T Owner { get; }
        
        /// <summary>
        /// 状态数量
        /// </summary>
        int FsmStateCount { get; }
        
        /// <summary>
        /// 是否在运行
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// 是否被销毁
        /// </summary>
        bool IsDestroyed { get; }
        
        /// <summary>
        /// 当前状态
        /// </summary>
        FsmState<T> CurrentState { get; }
        
        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        float CurrentStateTime { get; }

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        void Start<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool HasState<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 获取所有状态
        /// </summary>
        /// <returns></returns>
        FsmState<T>[] GetAllStates();
        
        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        bool HasData(string name);
        
        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        TData GetData<TData>(string name);
        
        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        void SetData<TData>(string name, TData data);
        
        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        bool RemoveData(string name);
        
        /// <summary>
        /// 改变状态机当前状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        void ChangeState<TState>() where TState : FsmState<T>;
    }
}