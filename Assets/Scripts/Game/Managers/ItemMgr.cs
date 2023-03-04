using System.Collections.Generic;
using Hali_Framework;

namespace Game.Managers
{
    public class ItemMgr : Singleton<ItemMgr>
    {
        private Dictionary<int, ItemInfo> _itemDic;

        public ItemMgr()
        {
            _itemDic = BinaryDataMgr.Instance.GetTable<ItemInfoContainer>().dataDic;
        }
        
        public ItemInfo GetItem(int itemId)
        {
            if (_itemDic.ContainsKey(itemId))
                return _itemDic[itemId];

            return null;
        }
    }
}