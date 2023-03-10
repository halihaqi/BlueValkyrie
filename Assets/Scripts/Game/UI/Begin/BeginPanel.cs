using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.BeginScene;
using Game.Managers;
using Game.Model;
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

        private List<Button> _btns;
        private Button _pointBtn;
        private Button _btnLoad;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            //获取组件
            _btns = new List<Button>();
            _ringLayoutGroup = GetComponentInChildren<RingLayoutGroup>();

            var img = GetControl<Image>("group_btn");
            _btns = GetControls<Button>();
            _btnLoad = _btns.Find(b => b.name == "btn_load");
            
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
            
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            if (playerData == null || playerData.dataDic.Count <= 0)
                //隐藏Load按钮
                _btnLoad.gameObject.SetActive(false);
            else
                _btnLoad.gameObject.SetActive(true);
            
            _ringLayoutGroup.ArcBtnGroup();
            _ringLayoutGroup.transform.localScale = Vector3.one * 0.8f;
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            switch (btnName)
            {
                case "btn_new":
                    HideMe();
                    BeginCamera.Instance.Move(() =>
                    {
                        UIMgr.Instance.ShowPanel<ChooseRolePanel>();
                    });
                    break;
                case "btn_load":
                    UIMgr.Instance.ShowPanel<SaveLoadPop>(GameConst.UIGROUP_POP,
                        userData: new SaveLoadParam(false));
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