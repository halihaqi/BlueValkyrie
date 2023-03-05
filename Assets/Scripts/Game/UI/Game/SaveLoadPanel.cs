using System.Collections.Generic;
using Game.Managers;
using Game.Model;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class SaveLoadPanel : PanelBase
    {
        private UI_hub_form _hubForm;
        private HList _svSlData;
        private Button _btnSave;
        private Button _btnLoad;
        
        private Dictionary<int, PlayerInfo> _userDic;
        private bool _isSave;
        private int _saveBtnIndex;
        private int _loadBtnIndex;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _hubForm = GetControl<UI_hub_form>("hub_form");
            _svSlData = GetControl<HList>("sv_sl_data");
            _btnSave = GetControl<Button>("btn_save");
            _btnLoad = GetControl<Button>("btn_load");
            
            _svSlData.itemRenderer = OnItemRender;
            _svSlData.onClickItem = OnItemClick;
            _loadBtnIndex = _btnLoad.transform.GetSiblingIndex();
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _hubForm.SetData(HideMe);
            _userDic = PlayerMgr.Instance.LoadUserDic();
            OnSlClick(true);
            _svSlData.numItems = GameConst.FILE_NUM;
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            if (btnName == "btn_save") OnSlClick(true);

            if (btnName == "btn_load") OnSlClick(false);
        }

        private void OnSlClick(bool isSave)
        {
            if(isSave == _isSave) return;
            _isSave = isSave;
            if (isSave)
                _btnSave.transform.SetSiblingIndex(_loadBtnIndex);
            else
                _btnLoad.transform.SetSiblingIndex(_loadBtnIndex);
            _svSlData.RefreshList();
        }

        private void OnItemRender(int index, GameObject go)
        {
            var item = go.GetComponent<UI_btn_sl_item>();
            
            _userDic.TryGetValue(index, out var info);
            if(_isSave)
                item.SetSave(index, info);
            else
                item.SetLoad(index, info);
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
}