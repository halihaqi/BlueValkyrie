using System.Collections.Generic;
using UnityEngine;
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
            _panelEntity = GetComponent<PanelEntity>();
            _originalLayer = gameObject.layer;
            
            //搜索常用UI组件添加到容器中
            FindChildrenControls<Button>();
            FindChildrenControls<Image>();
            FindChildrenControls<Text>();
            FindChildrenControls<Toggle>();
            FindChildrenControls<Slider>();
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
        protected internal virtual void OnRecycle(){}

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

        #endregion

        #region UI事件

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
        /// <param name="name">物体名</param>
        /// <returns></returns>
        public T GetControl<T>(string name) where T : UIBehaviour
        {
            //每个物体只会挂载一个同种类的组件，所以不会重复
            if (_controlDic.ContainsKey(name))
            {
                for (int i = 0; i < _controlDic[name].Count; i++)
                {
                    if (_controlDic[name][i] is T)
                        return _controlDic[name][i] as T;
                }
            }
            Debug.Log($"{Name} no UIControl named:{name}");
            return null;
        }
        
        /// <summary>
        /// 搜索所有子物体的特定UI组件并添加进字典容器中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildrenControls<T>() where T : UIBehaviour
        {
            //搜索所有子物体的组件
            T[] controls = this.GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; i++)
            {
                //添加组件进字典
                string objName = controls[i].gameObject.name;

                if (_controlDic.ContainsKey(objName))
                    _controlDic[objName].Add(controls[i]);
                else
                    _controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            
                //组件添加事件监听
                if(controls[i] is Button btn)
                {
                    btn.onClick.AddListener(() =>
                    {
                        OnClick(objName);
                    });
                }
                if (controls[i] is Toggle tog)
                {
                    tog.onValueChanged.AddListener((isToggle) =>
                    {
                        OnToggleValueChanged(objName, isToggle);
                    });
                }
                if (controls[i] is Slider sld)
                {
                    sld.onValueChanged.AddListener((val) =>
                    {
                        OnSliderValueChanged(objName, val);
                    });
                }
                if (controls[i] is InputField ifd)
                {
                    ifd.onValueChanged.AddListener((val) =>
                    {
                        OnInputFieldValueChanged(objName, val);
                    });
                }
            }
        }
    }
}