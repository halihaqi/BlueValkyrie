using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class ShopMgr : Singleton<ShopMgr>
    {
        private List<ShopItemInfo> _shopItems;
        private BagMaster _playerBag;
        private BagMaster _shopBag;
        private ShopTypeInfo _curShop;

        public ShopTypeInfo CurShop
        {
            get => _curShop;
            set => _curShop = value;
        }

        public ShopMgr()
        {
            _shopItems = BinaryDataMgr.Instance.GetTable<ShopItemInfoContainer>().dataDic.Values.ToList();
            _playerBag = PlayerMgr.Instance.BagMaster;
            _shopBag = PlayerMgr.Instance.ShopMaster;
        }
        
        public bool Buy(ShopTypeInfo shop, int itemId, int num)
        {
            var inventoryItem = _shopBag.GetItem(shop.shopBagId, itemId);
            var shopItemInfo = _shopItems.Find(i => i.itemId == inventoryItem.id);
            //判断商品是否够
            if (inventoryItem == null || shopItemInfo == null || inventoryItem.num < num) return false;
            
            //判断货币是否够
            var costItem = _playerBag.GetItem(0, shopItemInfo.currencyId);
            if (costItem == null || costItem.num < shopItemInfo.price) return false;
            
            //购买
            //商店减 人物加 货币扣
            _shopBag.AddItem(shop.shopBagId, inventoryItem, -num);
            _playerBag.AddItem(0, costItem, -shopItemInfo.price);
            _playerBag.AddItem(0, inventoryItem, num);
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
            var dic = BinaryDataMgr.Instance.GetTable<ShopItemInfoContainer>().dataDic;
            if (dic.ContainsKey(shopItemId))
                return dic[shopItemId].itemId;
            else
                return -1;
        }
    }
}