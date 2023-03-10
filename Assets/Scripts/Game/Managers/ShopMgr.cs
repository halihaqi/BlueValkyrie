﻿using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class ShopMgr : Singleton<ShopMgr>, IModule
    {
        private Dictionary<int, ShopItemInfo> _shopItems;
        private Dictionary<int, ShopTypeInfo> _shops;
        private ShopTypeInfo _curShop;

        public ShopTypeInfo CurShop
        {
            get => _curShop;
            set => _curShop = value;
        }

        public Dictionary<int, ShopItemInfo> ShopItems => _shopItems;
        public Dictionary<int, ShopTypeInfo> Shops => _shops;

        public int Priority => 2;

        void IModule.Init()
        {
            _shopItems = BinaryDataMgr.Instance.GetTable<ShopItemInfoContainer>().dataDic;
            _shops = BinaryDataMgr.Instance.GetTable<ShopTypeInfoContainer>().dataDic;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _shopItems.Clear();
            _shops.Clear();
            _shops = null;
            _shopItems = null;
            _curShop = null;
        }
        
        public bool Buy(ShopTypeInfo shop, int itemId, int num)
        {
            var shopMaster = PlayerMgr.Instance.ShopMaster;
            var bagMaster = PlayerMgr.Instance.BagMaster;
            var inventoryItem = shopMaster.GetItem(shop.shopBagId, itemId);
            var shopItemInfo = _shopItems.Values.FirstOrDefault(i => i.itemId == inventoryItem.id);
            //判断商品是否够
            if (inventoryItem == null || shopItemInfo == null || inventoryItem.num < num) return false;
            
            //判断货币是否够
            var costItem = PlayerMgr.Instance.BagMaster.GetItem(0, shopItemInfo.currencyId);
            if (costItem == null || costItem.num < shopItemInfo.price) return false;
            
            //购买
            //商店减 人物加 货币扣
            shopMaster.AddItem(shop.shopBagId, inventoryItem, -num);
            bagMaster.AddItem(0, costItem, -shopItemInfo.price);
            bagMaster.AddItem(0, inventoryItem, num);
            EventMgr.Instance.TriggerEvent(ClientEvent.BUY_COMPLETE);
            return true;
        }

        public bool Buy(int shopItemIndex, int num)
        {
            if (_curShop == null) return false;
            return Buy(_curShop, shopItemIndex, num);
        }

        public int GetShopItemRealId(int shopItemId)
        {
            if (_shopItems.ContainsKey(shopItemId))
                return _shopItems[shopItemId].itemId;
            else
                return -1;
        }

        public ShopItemInfo GetShopItem(int shopItemId)
        {
            return _shopItems.TryGetValue(shopItemId, out var shopItem) ? shopItem : null;
        }
    }
}