using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class ItemMgr : Singleton<ItemMgr>, IModule
    {
        private Dictionary<int, ItemInfo> _itemDic;

        public int Priority => 2;

        void IModule.Init()
        {
            _itemDic = BinaryDataMgr.Instance.GetTable<ItemInfoContainer>().dataDic;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _itemDic.Clear();
            _itemDic = null;
        }
        
        public ItemInfo GetItem(int itemId)
        {
            if (_itemDic.ContainsKey(itemId))
                return _itemDic[itemId];

            return null;
        }

        public List<ItemInfo> GetItems()
        {
            return _itemDic.Values.ToList();
        }

        public List<int> GetItemIds()
        {
            return _itemDic.Keys.ToList();
        }
    }
}