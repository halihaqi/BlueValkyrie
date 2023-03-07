using System.Collections.Generic;
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
    }
}