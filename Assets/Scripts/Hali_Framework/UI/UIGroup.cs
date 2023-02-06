using System;
using System.Collections.Generic;

namespace Hali_Framework
{
    public class UIGroup
    {
        private string _name;
        private int _depth;
        private bool _pause;

        private readonly LinkedList<PanelInfo> _panelInfos;
        private LinkedListNode<PanelInfo> _cachedNode;

        public UIGroup(string name, int depth)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("UI group name is invalid.");

            _name = name;
            _pause = false;
            _panelInfos = new LinkedList<PanelInfo>();
            _cachedNode = null;
            _depth = depth;
        }

        public string Name => _name;

        public int Depth
        {
            get => _depth;
            set
            {
                if(_depth.Equals(value)) return;
                
                _depth = value;
                SetDepth(_depth);
                Refresh();
            }
        }

        public bool Pause
        {
            get => _pause;
            set
            {
                if(_pause == value) return;

                _pause = value;
                Refresh();
            }
        }

        public int PanelCount => _panelInfos.Count;

        public PanelEntity CurPanelEntity => _panelInfos.First?.Value.PanelEntity;

        public UIGroupEntity UIGroupEntity { get; set; }

        public bool HasPanel(int serialId)
        {
            foreach (var info in _panelInfos)
            {
                if (info.PanelEntity.SerialId == serialId)
                    return true;
            }

            return false;
        }

        public PanelEntity GetPanel(int serialId)
        {
            foreach (var info in _panelInfos)
            {
                if (info.PanelEntity.SerialId == serialId)
                    return info.PanelEntity;
            }

            return null;
        }

        private PanelInfo GetPanelInfo(PanelEntity panelEntity)
        {
            foreach (var info in _panelInfos)
            {
                if (info.PanelEntity == panelEntity)
                    return info;
            }

            return null;
        }

        public List<PanelEntity> GetAllPanels()
        {
            List<PanelEntity> copyList = new List<PanelEntity>(_panelInfos.Count);
            foreach (var info in _panelInfos)
                copyList.Add(info.PanelEntity);
            
            return copyList;
        }

        public void SetDepth(int depth)
        {
            
        }

        
        /// <summary>
        /// 添加界面
        /// </summary>
        /// <param name="panelEntity"></param>
        internal void AddPanel(PanelEntity panelEntity) => _panelInfos.AddFirst(PanelInfo.Create(panelEntity));
        
        /// <summary>
        /// 界面组轮询
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        internal void Update(float elapseSeconds, float realElapseSeconds)
        {
            _cachedNode = _panelInfos.First;
            while (_cachedNode != null)
            {
                if(!_cachedNode.Value.Paused)
                    _cachedNode.Value.PanelEntity.OnUpdate(elapseSeconds, realElapseSeconds);   
                _cachedNode = _cachedNode.Next;
            }

            _cachedNode = null;
        }

        /// <summary>
        /// 移除界面
        /// </summary>
        /// <param name="panelEntity"></param>
        /// <exception cref="Exception"></exception>
        internal void RemovePanel(PanelEntity panelEntity)
        {
            var info = GetPanelInfo(panelEntity);
            if (info == null)
                throw new Exception($"Can not remove UI form from UIGroup:{_name} for serial id:{panelEntity.SerialId}");

            if (!info.Covered)
            {
                info.Covered = true;
                panelEntity.OnCover();
            }

            if (!info.Paused)
            {
                info.Paused = true;
                panelEntity.OnPause();
            }

            _panelInfos.Remove(info);
            ReferencePoolMgr.Instance.PushRef(info);
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panelEntity"></param>
        /// <param name="userData"></param>
        /// <exception cref="Exception"></exception>
        internal void RefocusPanel(PanelEntity panelEntity, object userData)
        {
            var info = GetPanelInfo(panelEntity);
            if (info == null)
                throw new Exception($"Can not refocus UI form from UIGroup:{_name} for serial id:{panelEntity.SerialId}");
            _panelInfos.Remove(info);
            _panelInfos.AddFirst(info);
        }

        /// <summary>
        /// 刷新界面组
        /// </summary>
        public void Refresh()
        {
            _cachedNode = _panelInfos.First;

            int panelCount = PanelCount;
            bool isFirst = true;
            
            while (_cachedNode != null)
            {
                if (_cachedNode.Value == null)
                {
                    _cachedNode = _cachedNode.Next;
                    continue;
                }
                
                _cachedNode.Value.PanelEntity.OnDepthChanged(Depth, panelCount--);

                //如果组暂停
                //将所有界面都Pause && Cover
                if (_pause)
                {
                    if (!_cachedNode.Value.Covered)
                    {
                        _cachedNode.Value.Covered = true;
                        _cachedNode.Value.PanelEntity.OnCover();
                    }
                    if (!_cachedNode.Value.Paused)
                    {
                        _cachedNode.Value.Paused = true;
                        _cachedNode.Value.PanelEntity.OnPause();
                    }
                }
                //如果组恢复暂停
                //将所有界面解除Pause，第一个不Cover，其余都Cover
                else
                {
                    if (_cachedNode.Value.Paused)
                    {
                        _cachedNode.Value.Paused = false;
                        _cachedNode.Value.PanelEntity.OnResume();
                    }

                    if (isFirst)
                    {
                        if (_cachedNode.Value.Covered)
                        {
                            _cachedNode.Value.Covered = false;
                            _cachedNode.Value.PanelEntity.OnReveal();
                        }
                        
                        isFirst = false;
                    }
                    else
                    {
                        if (!_cachedNode.Value.Covered)
                        {
                            _cachedNode.Value.Covered = true;
                            _cachedNode.Value.PanelEntity.OnCover();
                        }
                    }
                }
                
                _cachedNode = _cachedNode.Next;
            }
        }
    }
}