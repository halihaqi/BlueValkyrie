using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Begin;
using Game.UI.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Begin
{
    [ExecuteInEditMode]
    public class BeginPanel : PanelBase
    {
        private RingLayoutGroup _ringLayoutGroup;

        private Button[] _btns;
        private Button _pointBtn;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            //获取组件
            _ringLayoutGroup = GetComponentInChildren<RingLayoutGroup>();

            var img = GetControl<Image>("group_btn");
            _btns = GetControls<Button>().ToArray();
            
            //添加动效
            AddCustomListener(img, EventTriggerType.PointerEnter,
                OnPointEnterBtnGroup);
            AddCustomListener(img, EventTriggerType.PointerExit,
                OnPointExitBtnGroup);
            AddCustomListeners(_btns, EventTriggerType.PointerEnter,
                OnPointEnterBtn);
            AddCustomListeners(_btns, EventTriggerType.PointerExit,
                OnPointExitBtn);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _ringLayoutGroup.transform.localScale = Vector3.one * 0.8f;
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            switch (btnName)
            {
                case "btn_new":
                    UIMgr.Instance.HidePanel(this);
                    BeginCamera.Instance.Move(() =>
                    {
                        UIMgr.Instance.ShowPanel<ChooseRolePanel>();
                    });
                    break;
                case "btn_load":
                    break;
                case "btn_option":
                    break;
                case "btn_about":
                    break;
                case "btn_exit":
                    Application.Quit();
                    break;
            }
        }


        #region 自定义控件事件

        private void OnPointEnterBtnGroup(BaseEventData data)
        {
            _ringLayoutGroup.transform.DOScale(Vector3.one * 1.2f, 0.5f);
        }

        private void OnPointExitBtnGroup(BaseEventData data)
        {
            _ringLayoutGroup.transform.DOScale(Vector3.one * 0.8f, 0.5f);
        }

        private void OnPointEnterBtn(BaseEventData data)
        {
            PointerEventData point = data as PointerEventData;
            _pointBtn = point?.pointerCurrentRaycast.gameObject?.GetComponent<Button>();
            if(_pointBtn == null) return;
            _pointBtn.transform.DOScale(Vector3.one * 1.2f, 0.5f);
        }
        
        private void OnPointExitBtn(BaseEventData data)
        {
            if(_pointBtn == null) return;
            _pointBtn.transform.DOScale(Vector3.one, 0.5f);
            _pointBtn = null;
        }

        #endregion
    }
}