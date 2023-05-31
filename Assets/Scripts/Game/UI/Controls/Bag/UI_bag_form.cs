using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public partial class UI_bag_form : ControlBase
    {
        private List<BagItemInfo> _items;
        private int _bagId;

        protected internal override void OnInit()
        {
            base.OnInit();
            sv_bag.itemRenderer = OnItemRenderer;
            sv_bag.onClickItem = OnItemClick;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            btn_sort.onClick.RemoveListener(OnSortClick);
        }

        public void SetData(int bagId)
        {
            btn_sort.onClick.RemoveListener(OnSortClick);
            btn_sort.onClick.AddListener(OnSortClick);
            _bagId = bagId;
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
            btn_sort.onClick.AddListener(OnSortClick);
            _items = items ?? throw new Exception("Has no bag with id:{bagId}.");
            UpdateView();
        }

        private void UpdateView()
        {
            sv_bag.numItems = _items.Count;
        }

        private void OnItemRenderer(int index, GameObject obj)
        {
            var itemObj = obj.GetComponent<UI_bag_item>();
            var bagItemInfo = _items[index];
            itemObj.SetData(bagItemInfo.id, bagItemInfo.num);
        }

        private void OnItemClick(int index, ControlBase cb)
        {
            var item = cb as UI_bag_item;
            if(item == null) return;
            EventMgr.Instance.TriggerEvent(ClientEvent.BAG_ITEM_CLICK, item.Item, item.ItemNum);
        }

        private void OnSortClick()
        {
            Vector3 newScale = img_sort_arrow.rectTransform.localScale;
            newScale.y = -newScale.y;
            img_sort_arrow.rectTransform.localScale = newScale;
            
            PlayerMgr.Instance.BagMaster.SortBagById(_bagId, newScale.y < 0);
            _items = PlayerMgr.Instance.BagMaster.GetAllVisibleItems(_bagId);
            sv_bag.RefreshList();
        }
    }
}