﻿using Game.Model.BagModel;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_normal_item : ControlBase
    {
        private ItemInfo _info;
        private int _num;
        private Image _imgItem;
        private Text _txtNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            _imgItem = GetControl<Image>("img_item");
            _txtNum = GetControl<Text>("txt_num");
        }

        public void SetData(int itemId)
        {
            _info = BinaryDataMgr.Instance.GetInfo<ItemInfoContainer, int, ItemInfo>(itemId);
            SetNull(_info == null);
            if(_info == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_info), img =>
            {
                _imgItem.sprite = img;
            });
            _txtNum.text = "";
        }

        public void SetData(ItemInfo info)
        {
            _info = info;
            SetNull(_info == null);
            if(_info == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_info), img =>
            {
                _imgItem.sprite = img;
            });
            _txtNum.text = "";
        }

        public void SetData(BagItemInfo bagItemInfo)
        {
            if (bagItemInfo == null)
            {
                SetNull(true);
                return;
            }
            _info = BinaryDataMgr.Instance.GetInfo<ItemInfoContainer, int, ItemInfo>(bagItemInfo.id);
            SetNull(_info == null);
            if(_info == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_info), img =>
            {
                _imgItem.sprite = img;
            });
            _txtNum.text = bagItemInfo.num.ToXNum();
        }

        private void SetNull(bool isNull)
        {
            _imgItem.gameObject.SetActive(!isNull);
            _txtNum.gameObject.SetActive(!isNull);
        }
    }
}