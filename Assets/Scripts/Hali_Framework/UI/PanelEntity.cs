using System;
using System.Collections;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 挂载继承PanelBase的类后自动添加，
    /// 将UI业务层和架构层分开
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class PanelEntity : MonoBehaviour
    {
        private int _serialId;
        private int _depth;
        private string _assetName;
        private UIGroup _uiGroup;
        private PanelBase _panelLogic;
        
        private CanvasGroup _canvasGroup;
        private const float FADE_SPEED = 0.01f;
        private Action<PanelEntity> _showCompleteEvent;
        private Action<PanelEntity> _hideCompleteEvent;

        #region 属性

        /// <summary>
        /// 界面序列号
        /// </summary>
        public int SerialId => _serialId;

        /// <summary>
        /// 界面资源名
        /// </summary>
        public string AssetName => _assetName;
        
        /// <summary>
        /// 界面在组内深度
        /// </summary>
        public int Depth => _depth;

        /// <summary>
        /// 界面所属界面组
        /// </summary>
        public UIGroup UIGroup => _uiGroup;

        /// <summary>
        /// 界面逻辑
        /// </summary>
        public PanelBase Logic => _panelLogic;

        #endregion


        #region 生命周期

        public void OnInit(int serialId, string assetName, UIGroup uiGroup,
            bool isNewInstance, object userdata)
        {
            _serialId = serialId;
            _assetName = assetName;
            _uiGroup = uiGroup;
            _depth = 0;
            
            if(!isNewInstance) return;

            _panelLogic = GetComponent<PanelBase>();
            if (_panelLogic == null)
                throw new Exception($"Panel:{assetName} can not get panel logic.");
            _canvasGroup = GetComponent<CanvasGroup>();

            _panelLogic.OnInit(userdata);
        }

        public void OnShow(object userData)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInOut(true));
            _panelLogic.OnShow(userData);
        }

        private void OnShowComplete()
        {
            _panelLogic.OnShowComplete();
            _showCompleteEvent?.Invoke(this);
            EventMgr.Instance.TriggerEvent(ClientEvent.SHOW_PANEL_COMPLETE, this);
        }

        public void AddShowCompleteListener(Action<PanelEntity> callback)
        {
            _showCompleteEvent -= callback;
            _showCompleteEvent += callback;
        }

        public void RemoveShowCompleteListener(Action<PanelEntity> callback)
        {
            _showCompleteEvent -= callback;
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            _panelLogic.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public void OnHide(bool isShutdown, object userData)
        {
            _panelLogic.OnHide(isShutdown, userData);
            if (!isShutdown)
            {
                StopAllCoroutines();
                StartCoroutine(FadeInOut(false));
            }
            else
                OnHideComplete();
        }

        private void OnHideComplete()
        {
            _panelLogic.OnHideComplete();
            _hideCompleteEvent?.Invoke(this);
            EventMgr.Instance.TriggerEvent(ClientEvent.HIDE_PANEL_COMPLETE, this);
        }

        public void AddHideCompleteListener(Action<PanelEntity> callback)
        {
            _hideCompleteEvent -= callback;
            _hideCompleteEvent += callback;
        }

        public void RemoveHideCompleteListener(Action<PanelEntity> callback)
        {
            _hideCompleteEvent -= callback;
        }

        public void OnRecycle()
        {
            _panelLogic.OnRecycle();

            _serialId = 0;
            _depth = 0;
        }

        public void OnPause()
        {
            _panelLogic.OnPause();
        }

        public void OnResume()
        {
            _panelLogic.OnResume();
        }

        public void OnCover()
        {
            _panelLogic.OnCover();
        }

        public void OnReveal()
        {
            _panelLogic.OnReveal();
        }

        public void OnRefocus(object userData)
        {
            _panelLogic.OnRefocus(userData);
        }

        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            _depth = depthInUIGroup;
            _panelLogic.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }
        
        /// <summary>
        /// 面板渐变显示隐藏协程
        /// </summary>
        /// <param name="isIn">是否显示</param>
        /// <returns></returns>
        private IEnumerator FadeInOut(bool isIn)
        {
            if (_canvasGroup == null)
                throw new Exception($"{AssetName} has no CanvasGroup.");
            if (isIn)
            {
                _canvasGroup.alpha = 0;
                while(_canvasGroup.alpha < 1)
                {
                    _canvasGroup.alpha += FADE_SPEED;
                    yield return null;
                }

                OnShowComplete();
            }
            else
            {
                _canvasGroup.alpha = 1;
                while (_canvasGroup.alpha > 0)
                {
                    _canvasGroup.alpha -= FADE_SPEED;
                    yield return null;
                }

                OnHideComplete();
            }
        }

        #endregion
    }
}