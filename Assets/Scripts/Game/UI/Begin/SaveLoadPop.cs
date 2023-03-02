using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Model;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Begin
{
    public class SaveLoadPop : PopBase
    {
        private const string ITEM_PATH = "UI/Controls/btn_sl_item";

        private bool _isSave;
        private string _nowUserName;
        private int _secretaryId;
        
        private HList _list;

        private Dictionary<int, PlayerInfo> _userDic;

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);

            _list = GetControl<HList>("sv_sl_data");
            _list.IsVirtual = false;
            _list.itemRenderer = OnItemRender;
            _list.onClickItem = OnItemClick;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            if (userData is SaveLoadParam p)
            {
                _isSave = p.isSave;
                _nowUserName = p.userName;
                _secretaryId = p.secretaryId;
            }
            else
                throw new Exception($"{Name} param is invalid.");
            
            _userDic = PlayerMgr.Instance.LoadUserDic();
            _list.numItems = GameConst.FILE_NUM;
        }

        private void OnItemRender(int index, GameObject go)
        {
            var item = go.GetComponent<UI_btn_sl_item>();
            
            _userDic.TryGetValue(index, out var info);
            item.SetData(_isSave, index, info, _nowUserName, _secretaryId);
        }

        private void OnItemClick(int index, ControlBase cb)
        {
            var item = cb as UI_btn_sl_item;
            if(_isSave)
                item.OnSaveClick();
            else
                item.OnLoadClick();
        }
    }

    public class SaveLoadParam
    {
        public bool isSave;
        public string userName;
        public int secretaryId;

        public SaveLoadParam(bool isSave, string userName, int secretaryId)
            => (this.isSave, this.userName, this.secretaryId) = (isSave, userName, secretaryId);

        public SaveLoadParam(bool isSave) => this.isSave = isSave;
    }
}