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
        private int _bagId;

        private HList _list;
        private Button _btnSift;
        private Button _btnSort;
        private Image _imgSortArrow;

        protected internal override void OnInit()
        {
            base.OnInit();
            _list = GetControl<HList>("sv_bag");
            _btnSift = GetControl<Button>("btn_sift");
            _btnSort = GetControl<Button>("btn_sort");
            _imgSortArrow = GetControl<Image>("img_sort_arrow");

            _list.itemRenderer = OnItemRenderer;
            _list.onClickItem = OnItemClick;
            _btnSort.onClick.AddListener(OnSortClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _btnSort.onClick.RemoveListener(OnSortClick);
        }

        public void SetData(int bagId)
        {
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

        private void OnSortClick()
        {
            Vector3 newScale = _imgSortArrow.rectTransform.localScale;
            newScale.y = -newScale.y;
            _imgSortArrow.rectTransform.localScale = newScale;
            
            PlayerMgr.Instance.BagMaster.SortBagById(_bagId, newScale.y < 0);
            _items = PlayerMgr.Instance.BagMaster.GetAllVisibleItems(_bagId);
            _list.RefreshList();
        }
    }
}