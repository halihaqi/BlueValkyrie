using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Game
{
    public class ShopPanel : PanelBase
    {
        private UI_hub_form _hubForm;
        private HList _svShopList;
        private HList _svShop;

        private BagMaster _bm;

        private int[] _shopBagIds;
        private List<ShopTypeInfo> _shops;
        private int _curShopIndex = -1;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _hubForm = GetControl<UI_hub_form>("hub_form");
            _svShopList = GetControl<HList>("sv_shop_list");
            _svShop = GetControl<HList>("sv_shop");

            _svShopList.itemRenderer = OnShopListRenderer;
            _svShop.itemRenderer = OnShopRenderer;
            _svShopList.onClickItem = OnShopListItemClick;

            _bm = PlayerMgr.Instance.ShopMaster;
            _shops = BinaryDataMgr.Instance.GetTable<ShopTypeInfoContainer>().dataDic.Values.ToList();
            _shopBagIds = _bm.GetAllBagIds();
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _hubForm.SetData(HideMe);
            _svShopList.numItems = _shopBagIds.Length;
            SetShop(0);
            EventMgr.Instance.AddListener(ClientEvent.BUY_COMPLETE, OnBuyComplete);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            ShopMgr.Instance.CurShop = null;
            EventMgr.Instance.RemoveListener(ClientEvent.BUY_COMPLETE, OnBuyComplete);
        }

        private void OnBuyComplete()
        {
            _hubForm.UpdateView();
            _svShop.RefreshList();
        }

        private void OnShopListRenderer(int index, GameObject obj)
        {
            var item = obj.GetComponent<UI_btn_normal>();
            var shop = _shops.Find(s => s.shopBagId == _shopBagIds[index]);
            if(shop != null)
                item.SetData(shop.name);
        }

        private void OnShopRenderer(int index, GameObject obj)
        {
            var item = obj.GetComponent<UI_shop_item>();
            var shopItemId = ShopMgr.Instance.CurShop.itemList[index];
            var shopItemNum = ShopMgr.Instance.CurShop.itemInventory[index];
            item.SetData(shopItemId, shopItemNum);
        }

        private void OnShopListItemClick(int index, ControlBase cb)
        {
            if(_curShopIndex == _shopBagIds[index]) return;
            SetShop(index);
        }

        private void SetShop(int shopIndex)
        {
            _curShopIndex = _shopBagIds[shopIndex];
            ShopMgr.Instance.CurShop = _shops.Find(s => s.shopBagId == _curShopIndex);
            _svShop.numItems = ShopMgr.Instance.CurShop.itemList.Length;
        }
    }
}