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
            var itemData = BinaryDataMgr.Instance.GetTable<ShopItemInfoContainer>().dataDic;
            foreach (var shopItemInfo in itemData.Values)
            {
                if(!BagData.HasBag(shopItemInfo.currencyId))
                    BagData.AddBag(shopItemInfo.currencyId);

                BagData.AddItem(shopItemInfo.currencyId, shopItemInfo.id, shopItemInfo.num);
            }
        }
    }
}