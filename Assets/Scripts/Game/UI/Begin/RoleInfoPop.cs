using System;
using Game.Managers;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Begin
{
    public class RoleInfoPop : PopBase
    {
        private Text _txtName;
        private Text _txtNameNick;
        private Text _txtAge;
        private Text _txtBelong;
        private Text _txtGrade;
        private Text _txtTaste;
        private Text _txtMoe;

        private Image _imgBadge;
        private Image _imgHead;

        private RoleInfo _info;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _txtName = GetControl<Text>("txt_name");
            _txtNameNick = GetControl<Text>("txt_name_nick");
            _txtAge = GetControl<Text>("txt_age");
            _txtTaste = GetControl<Text>("txt_taste");
            _txtBelong = GetControl<Text>("txt_belong");
            _txtGrade = GetControl<Text>("txt_grade");
            _txtMoe = GetControl<Text>("txt_moe");

            _imgBadge = GetControl<Image>("img_badge");
            _imgHead = GetControl<Image>("img_head");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is int id)
                _info = RoleMgr.Instance.GetRole(id);

            if (_info == null)
                throw new Exception($"{Name} find no RoleInfo.");

            _txtName.text = _info.fullName;
            _txtNameNick.text = _info.nickName;
            _txtAge.text = $"{_info.age}岁";
            _txtTaste.text = _info.taste;
            _txtBelong.text = _info.belong;
            _txtGrade.text = _info.grade;
            _txtMoe.text = _info.moe;
            
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetSchoolBadgeIcon(_info), img =>
            {
                _imgBadge.sprite = img;
            });
            
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_info), img =>
            {
                _imgHead.sprite = img;
            });
            
            EventMgr.Instance.AddListener(ClientEvent.ROLE_CHANGE_BEGIN, OnRoleChangeBegin);
        }
        
        protected internal override void OnShowComplete()
        {
            base.OnShowComplete();
            EventMgr.Instance.TriggerEvent(ClientEvent.ROLE_CHANGE_COMPLETE);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener(ClientEvent.ROLE_CHANGE_BEGIN, OnRoleChangeBegin);
        }

        private void OnRoleChangeBegin() => HideMe();
    }
}