using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    public class UIMgr : Singleton<UIMgr>, IModule
    {
        private readonly Dictionary<string, UIGroup> _uiGroups;
        private readonly Dictionary<int, string> _loadingPanels;
        private readonly HashSet<int> _loadingPanelsToRelease;
        private readonly Queue<PanelEntity> _recycleQueue;
        private int _cachedSerialId;
        private GameObject _canvas;
        private GameObject _eventSystem;

        private const string PANEL_PATH = "UI/";

        /// <summary>
        /// 界面组数量
        /// </summary>
        public int UIGroupCount => _uiGroups.Count;

        public int Priority => 0;

        public UIMgr()
        {
            _uiGroups = new Dictionary<string, UIGroup>();
            _loadingPanels = new Dictionary<int, string>();
            _loadingPanelsToRelease = new HashSet<int>();
            _recycleQueue = new Queue<PanelEntity>();
            _cachedSerialId = 0;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
            while (_recycleQueue.Count > 0)
            {
                PanelEntity panelEntity = _recycleQueue.Dequeue();
                panelEntity.OnRecycle();
                ObjectPoolMgr.Instance.PushObj(PANEL_PATH + panelEntity.AssetName, panelEntity.gameObject);
            }

            foreach (var group in _uiGroups.Values)
            {
                group.Update(elapseSeconds, realElapseSeconds);
            }
        }

        void IModule.Dispose()
        {
            _uiGroups.Clear();
            _loadingPanels.Clear();
            _loadingPanelsToRelease.Clear();
            _recycleQueue.Clear();
            _cachedSerialId = 0;
        }

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public bool HasUIGroup(string groupName) => _uiGroups.ContainsKey(groupName);

        /// <summary>
        /// 获得界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public UIGroup GetUIGroup(string groupName)
        {
            return _uiGroups.TryGetValue(groupName, out var group) ? group : null;
        }

        /// <summary>
        /// 获得所有界面组
        /// </summary>
        /// <returns></returns>
        public UIGroup[] GetAllUIGroups()
        {
            int index = 0;
            UIGroup[] allGroups = new UIGroup[_uiGroups.Count];
            foreach (var group in _uiGroups.Values)
                allGroups[index++] = group;
            return allGroups;
        }

        /// <summary>
        /// 添加界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="groupDepth">深度</param>
        /// <returns></returns>
        public bool AddUIGroup(string groupName, int groupDepth)
        {
            if (HasUIGroup(groupName))
                return false;

            _uiGroups.Add(groupName, new UIGroup(groupName, groupDepth));
            InitCanvas();
            GameObject obj = new GameObject(groupName);
            obj.AddComponent<UIGroupEntity>().BindUIGroup(_uiGroups[groupName]);
            obj.transform.SetParent(_canvas.transform, false);
            return true;
        }

        /// <summary>
        /// 是否拥有界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool HasPanel(int serialId)
        {
            foreach (var group in _uiGroups.Values)
            {
                if (group.HasPanel(serialId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获得界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public PanelEntity GetPanel(int serialId)
        {
            foreach (var group in _uiGroups.Values)
            {
                var panel = group.GetPanel(serialId);
                if (panel != null)
                    return panel;
            }

            return null;
        }

        /// <summary>
        /// 获得所有已加载界面
        /// </summary>
        /// <returns></returns>
        public List<PanelEntity> GetAllLoadedPanels()
        {
            List<PanelEntity> panelList = new List<PanelEntity>(_uiGroups.Count);
            foreach (var group in _uiGroups.Values)
                panelList.AddRange(group.GetAllPanels());

            return panelList;
        }

        /// <summary>
        /// 界面是否正在加载
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool IsPanelLoading(int serialId) => _loadingPanels.ContainsKey(serialId);

        /// <summary>
        /// 获得所有正在加载的界面
        /// </summary>
        /// <returns></returns>
        public int[] GetAllLoadingPanelIds()
        {
            int num = 0;
            int[] loadingIds = new int[_loadingPanels.Count];
            foreach (var id in _loadingPanels.Keys)
                loadingIds[num++] = id;

            return loadingIds;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">面板类名必须和资源名一致</typeparam>
        /// <returns>界面id</returns>
        public int ShowPanel<T>(string uiGroupName = GameConst.UIGROUP_Panel,
            object userData = null, Action<PanelBase> callback = null) where T : PanelBase =>
            ShowPanel(typeof(T).Name, uiGroupName, userData, callback);

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="assetName">UI资源名</param>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <returns>界面id</returns>
        /// <exception cref="Exception"></exception>
        public int ShowPanel(string assetName, string uiGroupName = GameConst.UIGROUP_Panel, object userData = null,
            Action<PanelBase> callback = null)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new Exception("Show panel failed. Asset name is invalid.");

            UIGroup group = GetUIGroup(uiGroupName);
            if (group == null)
                throw new Exception($"Show panel failed. dont contains {uiGroupName} UIGroup.");

            int id = ++_cachedSerialId;

            bool isNew = false;
            //如果需要加载，就加入加载列表
            if (ObjectPoolMgr.Instance.GetPoolCacheNum(PANEL_PATH + assetName) <= 0)
            {
                _loadingPanels.Add(id, assetName);
                isNew = true;
            }

            ObjectPoolMgr.Instance.PopObj(PANEL_PATH + assetName,
                obj =>
                {
                    //如果在加载途中被Shutdown，加载完直接回收
                    if (_loadingPanelsToRelease.Contains(id))
                    {
                        ObjectPoolMgr.Instance.PushObj(PANEL_PATH + assetName, obj);
                        _loadingPanelsToRelease.Remove(id);
                        return;
                    }

                    InternalShowPanel(id, assetName, group, obj, isNew, userData, callback);
                });
            return id;
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(int serialId, object userData = null, bool isShutdown = false)
        {
            //如果正在加载，加载完再隐藏
            if (IsPanelLoading(serialId))
            {
                _loadingPanelsToRelease.Add(serialId);
                _loadingPanels.Remove(serialId);
                return;
            }
            
            HidePanel(GetPanel(serialId), userData, isShutdown);
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(PanelBase panel, object userData = null, bool isShutdown = false)
            => HidePanel(panel.PanelEntity, userData, isShutdown);

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        /// <exception cref="Exception"></exception>
        public void HidePanel(PanelEntity panel, object userData = null, bool isShutdown = false)
        {
            if (panel == null)
                Debug.Log("Hide panel is null.");
            UIGroup group = panel.UIGroup;
            if (group == null)
                throw new Exception("UI group is invalid.");

            group.RemovePanel(panel);
            panel.OnHide(isShutdown, userData);
            
            if (isShutdown)
            {
                group.Refresh();
                _recycleQueue.Enqueue(panel);
            }
            else
            {
                panel.AddHideCompleteListener(OnPanelHideComplete);
            }
        }

        private void OnPanelHideComplete(PanelEntity panelEntity)
        {
            panelEntity.UIGroup.Refresh();
            _recycleQueue.Enqueue(panelEntity);
            panelEntity.RemoveHideCompleteListener(OnPanelHideComplete);
        }

        /// <summary>
        /// 隐藏所有已加载界面
        /// </summary>
        /// <param name="userData"></param>
        public void HideAllLoadedPanels(object userData)
        {
            foreach (var panel in GetAllLoadedPanels())
            {
                if(HasPanel(panel.SerialId))
                    HidePanel(panel, userData);
            }
        }

        /// <summary>
        /// 隐藏所有正在加载界面
        /// </summary>
        public void HideAllLoadingPanels()
        {
            foreach (var id in _loadingPanels.Keys)
                _loadingPanelsToRelease.Add(id);
            _loadingPanels.Clear();
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(int serialId, object userData = null)
            => RefocusPanel(GetPanel(serialId), userData);
        
        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(PanelBase panel, object userData = null)
            => RefocusPanel(panel.PanelEntity, userData);

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <exception cref="Exception"></exception>
        public void RefocusPanel(PanelEntity panel, object userData = null)
        {
            UIGroup group = panel != null ? panel.UIGroup : null;
            if (group == null)
                throw new Exception("UI group is invalid.");
            
            group.RefocusPanel(panel, userData);
            group.Refresh();
            panel.OnRefocus(userData);
        }


        private void InternalShowPanel(int serialId, string assetName, UIGroup uiGroup, GameObject obj, bool isNew,
            object userData, Action<PanelBase> callback)
        {
            //移除加载列表
            if (isNew)
                _loadingPanels.Remove(serialId);

            obj.name = assetName;
            obj.transform.SetParent(uiGroup.UIGroupEntity.transform, false);
            if (!obj.TryGetComponent(out PanelEntity panel))
            {
                Debug.LogError("panel has no panel entity.");
                return;
            }

            panel.OnInit(serialId, assetName, uiGroup, isNew, userData);
            uiGroup.AddPanel(panel);
            panel.OnShow(userData);
            uiGroup.Refresh();
            callback?.Invoke(panel.Logic);
        }

        private void InitCanvas()
        {
            if (_canvas == null)
            {
                _canvas = new GameObject("Canvas");
                _canvas.AddComponent<CanvasEntity>();
            }

            if (_eventSystem == null)
            {
                _eventSystem = new GameObject("EventSystem");
                _eventSystem.AddComponent<EventSystemEntity>();
            }
        }
    }
}