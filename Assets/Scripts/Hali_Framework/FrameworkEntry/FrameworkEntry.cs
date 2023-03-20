using System.Collections.Generic;
using Game.Managers;

namespace Hali_Framework
{
    public static class FrameworkEntry
    {
        private static readonly LinkedList<IModule> _modules = new LinkedList<IModule>();
        private static LinkedListNode<IModule> _cachedNode = null;

        public static void Init()
        {
            RegisterModule(MonoMgr.Instance);
            RegisterModule(AudioMgr.Instance);
            RegisterModule(BinaryDataMgr.Instance);
            RegisterModule(EventMgr.Instance);
            RegisterModule(FsmMgr.Instance);
            RegisterModule(InputMgr.Instance);
            RegisterModule(ObjectPoolMgr.Instance);
            RegisterModule(ReferencePoolMgr.Instance);
            RegisterModule(ProcedureMgr.Instance);
            RegisterModule(ResMgr.Instance);
            RegisterModule(SceneMgr.Instance);
            RegisterModule(UIMgr.Instance);
            //GameModule
            RegisterModule(EquipMgr.Instance);
            RegisterModule(ItemMgr.Instance);
            RegisterModule(PlayerMgr.Instance);
            RegisterModule(RoleMgr.Instance);
            RegisterModule(ShopMgr.Instance);
            RegisterModule(BattleMgr.Instance);
            RegisterModule(StudentMgr.Instance);
            RegisterModule(EpisodeMgr.Instance);
            
            InitModules();
            MonoMgr.Instance.AddUpdateListener(Update);

            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_WORLD, -1);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_PANEL, 0);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_POP, 1);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_TIP, 2);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_SYS, 3);
        }

        /// <summary>
        /// 游戏模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑时间</param>
        /// <param name="realElapseSeconds">实际时间</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var module in _modules)
                module.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理所有模块
        /// </summary>
        public static void Dispose()
        {
            foreach (var module in _modules)
                module.Dispose();
            _modules.Clear();
        }

        public static T GetModule<T>() where T : Singleton<T>, IModule, new()
        {
            foreach (var module in _modules)
            {
                if (module is T)
                    return module as T;
            }

            return null;
        }

        public static void InitModules()
        {
            _cachedNode = _modules.First;
            while (_cachedNode != null)
            {
                _cachedNode.Value.Init();
                _cachedNode = _cachedNode.Next;
            }
        }

        public static void DisposeModules()
        {
            _cachedNode = _modules.First;
            while (_cachedNode != null)
            {
                _cachedNode.Value.Dispose();
                _cachedNode = _cachedNode.Next;
            }
        }

        public static void RegisterModule(IModule module)
        {
            if(_modules.Contains(module)) return;

            if (_modules.Last == null || module.Priority >= _modules.Last.Value.Priority)
                _modules.AddLast(module);
            else
            {
                //倒序遍历
                _cachedNode = _modules.Last;
                while (_cachedNode != null)
                {
                    if (module.Priority < _cachedNode.Value.Priority)
                        _modules.AddBefore(_cachedNode, module);
                    _cachedNode = _cachedNode.Previous;
                }
            }
        }
        
    }
}