using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;

namespace Game.Model
{
    [System.Serializable]
    public class ShopInfo : IBagRole
    {
        public int playerId;
        public BagData BagData { get; set; }

        public ShopInfo(){}
        
        public ShopInfo(int playerId)
        {
            this.playerId = playerId;
            BagData = new BagData(playerId);
            var shopType = BinaryDataMgr.Instance.GetTable<ShopTypeInfoContainer>().dataDic;
            var shopItems = BinaryDataMgr.Instance.GetTable<ShopItemInfoContainer>().dataDic;

            //添加所有商店
            foreach (var shop in shopType.Values)
            {
                BagData.AddBag(shop.shopBagId);
                //给商店添加贩售道具
                for (int i = 0; i < shop.itemList.Length; i++)
                {
                    BagData.AddItem
                        (shop.shopBagId, shopItems[shop.itemList[i]].itemId, shop.itemInventory[i]);
                }
            }
        }
    }
}