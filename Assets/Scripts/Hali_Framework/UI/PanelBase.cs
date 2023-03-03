using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    [RequireComponent(typeof(PanelEntity))]
    public abstract class PanelBase : MonoBehaviour
    {
        private const char KEY = '_';
        private bool _available = false;
        private bool _visible = false;
        private PanelEntity _panelEntity = null;
        private Transform _cachedTransform = null;
        private int _originalLayer = 0;

        private Dictionary<string, List<UIBehaviour>> _controlDic;
        private Dictionary<Type, List<ControlBase>> _addControlDic;

        public PanelEntity PanelEntity => _panelEntity;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public bool Available => _available;

        public bool Visible
        {
            get => _available && _visible;
            set
            {
                if (!_available)
                {
                    Debug.LogWarning($"Panel '{Name}' is not available.");
                    return;
                }
                
                if(_visible == value) return;

                _visible = value;
                InternalSetVisible(value);
            }
        }
        
        public bool IsFullScreen { get; protected set; }

        public Transform CachedTransform => _cachedTransform;

        
        #region 生命周期

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">自定义数据</param>
        protected internal virtual void OnInit(object userData)
        {
            _cachedTransform ??= transform;
            _controlDic = new Dictionary<string, List<UIBehaviour>>();
            _addControlDic = new Dictionary<Type, List<ControlBase>>();
            _panelEntity = GetComponent<PanelEntity>();
            _originalLayer = gameObject.layer;
            
            //搜索所有子物体的UI组件添加到容器中
            FindChildrenControls(this.transform);
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">自定义数据</param>
        protected internal virtual void OnShow(object userData)
        {
            _available = true;
            Visible = true;
            if (IsFullScreen)
            {
                InputMgr.Instance.Enabled = false;
                //如果是全屏，隐藏上方所有面板
                UIMgr.Instance.HideUpperUIGroupPanels(_panelEntity.UIGroup);
            }
        }
        
        /// <summary>
        /// 界面打开完成
        /// </summary>
        protected internal virtual void OnShowComplete()
        {
        }
        
        /// <summary>
        /// 界面轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑时间</param>
        /// <param name="realElapseSeconds">真实时间</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds){}

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="isShutdown"></param>
        /// <param name="userData"></param>
        protected internal virtual void OnHide(bool isShutdown, object userData)
        {
        }
        
        /// <summary>
        /// 界面关闭完成
        /// </summary>
        protected internal virtual void OnHideComplete()
        {
            Visible = false;
            _available = false;
            if (IsFullScreen)
                InputMgr.Instance.Enabled = true;
        }

        /// <summary>
        /// 界面回收
        /// </summary>
        protected internal virtual void OnRecycle()
        {
            RecycleAddCustomControls();
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        protected internal virtual void OnPause()
        {
        }
        
        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        protected internal virtual void OnResume()
        {
        }
        
        /// <summary>
        /// 界面遮挡
        /// </summary>
        protected internal virtual void OnCover(){}
        
        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        protected internal virtual void OnReveal(){}
        
        /// <summary>
        /// 界面激活
        /// </summary>
        protected internal virtual void OnRefocus(object userData){}
        
        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        protected internal virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup){}

        protected void HideMe(object userData, bool isShutdown) 
            => UIMgr.Instance.HidePanel(PanelEntity.SerialId, userData, isShutdown);

        protected void HideMe() => HideMe(null, false);

        #endregion

        
        #region UI Event

        protected virtual void OnClick(string btnName){}

        protected virtual void OnToggleValueChanged(string togName, bool isToggle){}

        protected virtual void OnSliderValueChanged(string sldName, float val){}

        protected virtual void OnInputFieldValueChanged(string inputName, string val){}

        #endregion


        #region Register Controls

        /// <summary>
        /// 获得物体挂载的UI组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="controlName">物体名</param>
        /// <returns></returns>
        public T GetControl<T>(string controlName) where T : UIBehaviour
        {
            //每个物体只会挂载一个同种类的组件，所以不会重复
            if (_controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < _controlDic[controlName].Count; i++)
                {
                    if (_controlDic[controlName][i] is T control)
                        return control;
                }
            }
            Debug.Log($"{Name} no UIControl named:{controlName}");
            return null;
        }

        /// <summary>
        /// 获得所有该种类UI控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetControls<T>() where T : UIBehaviour
        {
            List<T> list = new List<T>();
            foreach (var control in _controlDic.Values)
            {
                T item = control.Find(o => o is T) as T;
                if(item != null)
                    list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 搜索所有子物体的UI组件并添加进字典容器中，
        /// 如果是ControlBase下的子物体将不会添加，由ControlBase管理
        /// </summary>
        private void FindChildrenControls(Transform parent)
        {
            if(parent.childCount <=0) return;

            //搜索所有子物体的组件
            Transform child;
            bool stopRecursion = false;
            for (int i = 0; i < parent.childCount; i++)
            {
                stopRecursion = false;
                child = parent.GetChild(i);
                //不加下划线的控件不加入
                if (!child.name.Contains(KEY))
                {
                    FindChildrenControls(child);
                    continue;
                }
                var controls = child.GetComponents<UIBehaviour>();
                foreach (var control in controls)
                {
                    if (_controlDic.ContainsKey(control.name))
                        _controlDic[control.name].Add(control);
                    else
                        _controlDic.Add(control.name, new List<UIBehaviour> { control });
                        
                    //如果是ControlBase，中断搜索，由ControlBase管理子控件
                    if (control is ControlBase cb)
                    {
                        cb.OnInit();
                        stopRecursion = true;
                        continue;
                    }

                    #region 组件添加事件监听

                    if(control is Button btn)
                    {
                        btn.onClick.AddListener(() =>
                        {
                            OnClick(control.name);
                        });
                    }
                    if (control is Toggle tog)
                    {
                        tog.onValueChanged.AddListener((isToggle) =>
                        {
                            OnToggleValueChanged(control.name, isToggle);
                        });
                    }
                    if (control is Slider sld)
                    {
                        sld.onValueChanged.AddListener((val) =>
                        {
                            OnSliderValueChanged(control.name, val);
                        });
                    }
                    if (control is InputField ifd)
                    {
                        ifd.onValueChanged.AddListener((val) =>
                        {
                            OnInputFieldValueChanged(control.name, val);
                        });
                    }

                    #endregion
                }

                //递归搜索所有子物体
                if(!stopRecursion)
                    FindChildrenControls(child);
            }
        }

        #endregion
        
        
        #region Custom Control

        public List<T> GetAddControls<T>() where T : ControlBase
        {
            if (_addControlDic.ContainsKey(typeof(T)))
                return _addControlDic[typeof(T)] as List<T>;

            return null;
        }
        
        /// <summary>
        /// 实例化自定义控件并加入控件字典，自动触发控件的OnInit方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void InitCustomControl<T>(string path, Action<T> callback) where T : ControlBase
        {
            ResMgr.Instance.LoadAsync<GameObject>(path, go =>
            {
                var control = go.GetComponent<T>();
                if (control == null)
                    throw new Exception($"{control.GetType().Name} has no {typeof(T)}.");
                AddCustomControl(control);
                
                callback?.Invoke(control);
            });
        }

        public void AddCustomControl(ControlBase cb)
        {
            Type type = cb.GetType();
            if (_addControlDic.ContainsKey(type))
            {
                _addControlDic[type] ??= new List<ControlBase>();
                _addControlDic[type].Add(cb);
            }
            else
                _addControlDic.Add(type, new List<ControlBase> { cb });
            cb.OnInit();
        }

        public bool RemoveCustomControl<T>() where T : ControlBase
            => RemoveCustomControl(typeof(T));

        public bool RemoveCustomControl(Type cbType)
        {
            if (_addControlDic.ContainsKey(cbType))
            {
                var removeCbs = _addControlDic[cbType];
                _addControlDic.Remove(cbType);
                foreach (var cb in removeCbs)
                    cb.OnRecycle();
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 回收添加的自定义控件进池，OnRecycle调用
        /// </summary>
        private void RecycleAddCustomControls()
        {
            foreach (var cbList in _addControlDic.Values)
            {
                foreach (var control in cbList)
                    control.OnRecycle();
            }
            _addControlDic.Clear();
        }

        #endregion
        
        /// <summary>
        /// 设置界面的可见性
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}