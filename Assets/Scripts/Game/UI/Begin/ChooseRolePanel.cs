using System;
using System.Collections.Generic;
using Game.BeginScene;
using Game.Managers;
using Game.UI.Base;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Begin
{
    public class ChooseRolePanel : PanelBase
    {
        private int _selectIndex = -1;
        private List<int> _roleIdList;

        private Button _btnLeft;
        private Button _btnRight;
        private Button _btnSure;
        
        private Action<string> _sureCallback;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            //获取控件
            _btnLeft = GetControl<Button>("btn_left");
            _btnRight = GetControl<Button>("btn_right");
            _btnSure = GetControl<Button>("btn_sure");

            _sureCallback = SureCallback;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            //初始化数据
            var dic = BinaryDataMgr.Instance.GetTable<ChooseRoleInfoContainer>().dataDic;
            _roleIdList = new List<int>(dic.Count);
            foreach (var roleInfo in dic.Values)
                _roleIdList.Add(roleInfo.roleId);
            
            _roleIdList.Sort();
            _selectIndex = 0;
            
            EventMgr.Instance.AddListener(ClientEvent.ROLE_CHANGE_BEGIN, OnRoleChange);
            EventMgr.Instance.AddListener(ClientEvent.ROLE_SHOW_INFO, OnRoleShowInfo);
            EventMgr.Instance.AddListener(ClientEvent.ROLE_CHANGE_COMPLETE, OnRoleChangeComplete);
            BeginSceneMonoMgr.Instance.ChangeRole(_roleIdList[_selectIndex]);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener(ClientEvent.ROLE_CHANGE_BEGIN, OnRoleChange);
            EventMgr.Instance.RemoveListener(ClientEvent.ROLE_SHOW_INFO, OnRoleShowInfo);
            EventMgr.Instance.RemoveListener(ClientEvent.ROLE_CHANGE_COMPLETE, OnRoleChangeComplete);
            EventMgr.Instance.TriggerEvent(ClientEvent.ROLE_CHANGE_BEGIN);
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            switch (btnName)
            {
                case "btn_left":
                    if(BeginSceneMonoMgr.Instance.IsChanging) return;
                    _selectIndex = _selectIndex - 1 < 0 ? _roleIdList.Count - 1 : _selectIndex - 1;
                    BeginSceneMonoMgr.Instance.ChangeRole(_roleIdList[_selectIndex]);       
                    break;
                case "btn_right":
                    if(BeginSceneMonoMgr.Instance.IsChanging) return;
                    _selectIndex = _selectIndex + 1 > _roleIdList.Count - 1 ? 0 : _selectIndex + 1;
                    BeginSceneMonoMgr.Instance.ChangeRole(_roleIdList[_selectIndex]);  
                    break;
                case "btn_back":
                    BeginSceneMonoMgr.Instance.ResetRole();
                    UIMgr.Instance.HidePanel(this);
                    BeginCamera.Instance.Move(() =>
                    {
                        UIMgr.Instance.ShowPanel<BeginPanel>();
                    });
                    break;
                case "btn_sure":
                    UIMgr.Instance.ShowPanel<InputPop>(GameConst.UIGROUP_POP, userData: _sureCallback);
                    break;
            }
        }

        private void OnRoleChange()
        {
            _btnLeft.interactable = false;
            _btnRight.interactable = false;
            _btnSure.interactable = false;
        }

        private void OnRoleShowInfo()
        {
            UIMgr.Instance.ShowPanel<RoleInfoPop>(GameConst.UIGROUP_POP, userData: _roleIdList[_selectIndex]);
        }

        private void OnRoleChangeComplete()
        {
            _btnLeft.interactable = true;
            _btnRight.interactable = true;
            _btnSure.interactable = true;
        }

        private void SureCallback(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                TipHelper.ShowTip("请输入用户名!");
                return;
            }
            if (ExistUser(str))
            {
                TipHelper.ShowTip("用户已存在!");
                return;
            }

            UIMgr.Instance.ShowPanel<SaveLoadPop>(GameConst.UIGROUP_POP,
                userData: new SaveLoadParam(true, str, _roleIdList[_selectIndex], true));
        }

        private bool ExistUser(string userName)
        {
            return PlayerMgr.Instance.LoadUser(userName) != null;
        }
    }
}