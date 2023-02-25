using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_bag_form : ControlBase
    {
        private List<BagItemInfo> _items;

        private RectTransform _content;
        private Button _btnSift;
        private Button _btnSort;

        protected internal override void OnInit()
        {
            base.OnInit();
            _content = GetControl<ScrollRect>("sv_bag_cotent").content;
            _btnSift = GetControl<Button>("btn_sift");
            _btnSort = GetControl<Button>("btn_sort");
        }

        public void SetData(int bagId)
        {
            _items = PlayerMgr.Instance.BagMgr.GetAllItems(bagId);
            if (_items == null)
                throw new Exception("Has no bag with id:{bagId}.");

            UpdateView();
        }

        private void UpdateView()
        {
            //todo
            foreach (var item in _items)
            {
                AddCustomControl<UI_btn_bag_item>($"{UIMgr.CONTROL_PATH}btn_bag_item", bagItem =>
                {
                    bagItem.SetData(item.id, item.num);
                });
            }
        }
    }
}