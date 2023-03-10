using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;

namespace Game.Model
{
    [System.Serializable]
    public class ShopItem : IBagRole
    {
        public int playerId;
        public Inventory Inventory { get; set; }

        public ShopItem(){}
        
        public ShopItem(int playerId)
        {
            this.playerId = playerId;
            Inventory = new Inventory(playerId);
            var shopType = ShopMgr.Instance.Shops;
            var shopItems = ShopMgr.Instance.ShopItems;

            //添加所有商店
            foreach (var shop in shopType.Values)
            {
                Inventory.AddBag(shop.shopBagId);
                //给商店添加贩售道具
                for (int i = 0; i < shop.itemList.Length; i++)
                {
                    Inventory.AddItem
                        (shop.shopBagId, shopItems[shop.itemList[i]].itemId, shop.itemInventory[i]);
                }
            }
        }
    }
}