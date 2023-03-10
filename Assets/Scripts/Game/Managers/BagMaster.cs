using System.Collections.Generic;
using System.Linq;
using Game.Model;
using Game.Model.BagModel;

namespace Game.Managers
{
    public class BagMaster
    {
        private readonly IBagRole _owner;
        public BagMaster(IBagRole owner) => _owner = owner;

        public int BagCount => _owner.Inventory.BagCount;

        /// <summary>
        /// 背包是否存在
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public bool HasBag(int bagId) => _owner.Inventory.HasBag(bagId);
        
        /// <summary>
        /// 添加背包
        /// </summary>
        /// <param name="bagId"></param>
        public void AddBag(int bagId) => _owner.Inventory.AddBag(bagId);
        
        /// <summary>
        /// 移除背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public bool RemoveBag(int bagId) => _owner.Inventory.RemoveBag(bagId);

        public int[] GetAllBagIds() => _owner.Inventory.GetAllBagIds();
        
        /// <summary>
        /// 单个背包中道具是否存在
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int bagId, int itemId) => _owner.Inventory.HasItem(bagId, itemId);
        
        /// <summary>
        /// 所有物品中道具是否存在
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int itemId) => _owner.Inventory.HasItem(itemId);

        /// <summary>
        /// 添加道具到单个背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public int AddItem(int bagId, int itemId, int num = 1) => _owner.Inventory.AddItem(bagId, itemId, num);
        
        /// <summary>
        /// 添加道具到单个背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemInfo"></param>
        /// <param name="num"></param>
        public int AddItem(int bagId, ItemInfo itemInfo, int num = 1) => _owner.Inventory.AddItem(bagId, itemInfo, num);
        
        public int AddItem(int bagId, BagItemInfo itemInfo, int num = 1) => _owner.Inventory.AddItem(bagId, itemInfo, num);

        public bool TryGetItem(int bagId, int itemId, out BagItemInfo item)
        {
            if (HasItem(bagId, itemId))
            {
                item = GetItem(bagId, itemId);
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// 获得单个背包中的道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public BagItemInfo GetItem(int bagId, int itemId) => _owner.Inventory.GetItem(bagId, itemId);

        /// <summary>
        /// 获得所有背包中指定itemId的道具列表
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<BagItemInfo> GetItems(int itemId) => _owner.Inventory.GetItems(itemId);
        
        public List<BagItemInfo> GetItemsByType(int bagId, int itemType)
        {
            var items = _owner.Inventory.GetAllItems(bagId);
            return items.FindAll(i => i.type == itemType);
        }

        public List<BagItemInfo> GetItemsByType(int bagId, ItemType itemType)
            => GetItemsByType(bagId, (int)itemType);

        /// <summary>
        /// 获得指定背包中所有道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public List<BagItemInfo> GetAllItems(int bagId) => _owner.Inventory.GetAllItems(bagId);

        /// <summary>
        /// 获得指定背包中可显示的道具
        /// </summary>
        /// <returns></returns>
        public List<BagItemInfo> GetAllVisibleItems(int bagId)
        {
            return _owner.Inventory.GetAllItems(bagId).FindAll(i => i.visible == 1);
        }

        /// <summary>
        /// 根据id排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagById(int bagId, bool isAsc) => _owner.Inventory.SortBagById(bagId, isAsc);
        
        /// <summary>
        /// 根据type排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagByType(int bagId, bool isAsc) => _owner.Inventory.SortBagByType(bagId, isAsc);
        
        /// <summary>
        /// 根据num排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagByNum(int bagId, bool isAsc) => _owner.Inventory.SortBagByNum(bagId, isAsc);

        /// <summary>
        /// 清空一个背包
        /// </summary>
        /// <param name="bagId"></param>
        public void ClearBag(int bagId) => _owner.Inventory.ClearBag(bagId);

        /// <summary>
        /// 清空所有物品
        /// </summary>
        public void ClearAll() => _owner.Inventory.ClearAll();
    }
}