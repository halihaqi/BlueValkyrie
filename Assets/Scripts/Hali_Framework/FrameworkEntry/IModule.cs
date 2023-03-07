namespace Hali_Framework
{
    public interface IModule
    {
        /// <summary>
        /// 游戏模块优先级，优先级高的先轮询
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 模块初始化
        /// </summary>
        internal void Init();

        /// <summary>
        /// 游戏模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑时间</param>
        /// <param name="realElapseSeconds">真实时间</param>
        internal void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理游戏模块
        /// </summary>
        internal void Dispose();
    }
}