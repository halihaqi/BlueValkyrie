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

        private HList _list;
        private Button _btnSift;
        private Button _btnSort;

        protected internal override void OnInit()
        {
            base.OnInit();
            _list = GetControl<HList>("sv_bag");
            _btnSift = GetControl<Button>("btn_sift");
            _btnSort = GetControl<Button>("btn_sort");

            _list.IsVirtual = true;
            _list.itemRenderer = OnItemRenderer;
            _list.onClickItem = OnItemClick;
        }

        public void SetData(int bagId)
        {
            _items = PlayerMgr.Instance.BagMaster.GetAllVisibleItems(bagId);
            if (_items == null)
                throw new Exception("Has no bag with id:{bagId}.");

            UpdateView();
            
            if(_items.Count <= 0) return;
            EventMgr.Instance.TriggerEvent(ClientEvent.BAG_ITEM_CLICK, ItemMgr.Instance.GetItem(_items[0].id),
                _items[0].num);
        }

        public void SetData(List<BagItemInfo> items)
        {
            _items = items ?? throw new Exception("Has no bag with id:{bagId}.");
            UpdateView();
        }

        private void UpdateView()
        {
            _list.numItems = _items.Count;
        }

        private void OnItemRenderer(int index, GameObject obj)
        {
            var itemObj = obj.GetComponent<UI_btn_bag_item>();
            var bagItemInfo = _items[index];
            itemObj.SetData(bagItemInfo.id, bagItemInfo.num);
        }

        private void OnItemClick(int index, ControlBase cb)
        {
            var item = cb as UI_btn_bag_item;
            if(item == null) return;
            EventMgr.Instance.TriggerEvent(ClientEvent.BAG_ITEM_CLICK, item.Item, item.ItemNum);
        }
    }
}