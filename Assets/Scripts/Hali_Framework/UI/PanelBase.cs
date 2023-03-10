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
        private bool _available = false;
        private bool _visible = false;
        private PanelEntity _panelEntity = null;
        private Transform _cachedTransform = null;
        private int _originalLayer = 0;

        private Dictionary<string, List<UIBehaviour>> _controlDic;
        private Dictionary<string, List<ControlBase>> _addControlDic;

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
            _addControlDic = new Dictionary<string, List<ControlBase>>();
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
        }

        /// <summary>
        /// 界面回收
        /// </summary>
        protected internal virtual void OnRecycle()
        {
            RecycleCustomControls();
            _addControlDic.Clear();
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

        protected void HideMe(object userData = null, bool isShutdown = false) 
            => UIMgr.Instance.HidePanel(PanelEntity.SerialId, userData, isShutdown);

        #endregion

        #region UI事件

        /// <summary>
        /// 添加自定义UI事件，同类型事件只能添加一个
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="callback">事件</param>
        protected void AddCustomListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callback)
            => UIMgr.Instance.AddCustomListener(control, type, callback);

        protected void AddCustomListeners(UIBehaviour[] controls, EventTriggerType type,
            UnityAction<BaseEventData> callback)
            => UIMgr.Instance.AddCustomListeners(controls, type, callback);
        
        protected void AddCustomListeners<T>(List<T> controls, EventTriggerType type,
            UnityAction<BaseEventData> callback) where T : UIBehaviour
        => UIMgr.Instance.AddCustomListeners(controls, type, callback);

        protected void RemoveAllCustomListeners(UIBehaviour control)
            => UIMgr.Instance.RemoveAllCustomListeners(control);

        
        protected virtual void OnClick(string btnName){}

        protected virtual void OnToggleValueChanged(string togName, bool isToggle){}

        protected virtual void OnSliderValueChanged(string sldName, float val){}

        protected virtual void OnInputFieldValueChanged(string inputName, string val){}

        #endregion
        
        
        /// <summary>
        /// 设置界面的可见性
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        
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
        /// 添加自定义控件，会自动触发控件的OnInit方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void AddCustomControl<T>(string path, Action<T> callback) where T : ControlBase
        {
            ObjectPoolMgr.Instance.PopObj(path, go =>
            {
                var control = go.GetComponent<T>();
                if (control == null)
                    throw new Exception($"{go.name} has no {typeof(T)}.");
                if (_addControlDic.ContainsKey(path))
                {
                    _addControlDic[path] ??= new List<ControlBase>();
                    _addControlDic[path].Add(control);
                }
                else
                    _addControlDic.Add(path, new List<ControlBase> { control });
                
                control.OnInit();
                callback?.Invoke(control);
            });
        }

        /// <summary>
        /// 回收添加的自定义控件进池，默认OnRecycle调用
        /// </summary>
        protected void RecycleCustomControls()
        {
            foreach (var kv in _addControlDic)
            {
                foreach (var control in kv.Value)
                {
                    control.OnRecycle();
                    ObjectPoolMgr.Instance.PushObj(kv.Key, control.gameObject);
                }
            }
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
    }
}