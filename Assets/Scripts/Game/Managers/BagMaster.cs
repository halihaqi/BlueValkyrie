using System.Collections.Generic;
using Game.Model;
using Game.Model.BagModel;

namespace Game.Managers
{
    public class BagMaster
    {
        private PlayerInfo _owner;
        public BagMaster(PlayerInfo owner) => _owner = owner;

        /// <summary>
        /// 背包是否存在
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public bool HasBag(int bagId) => _owner.bagData.HasBag(bagId);
        
        /// <summary>
        /// 添加背包
        /// </summary>
        /// <param name="bagId"></param>
        public void AddBag(int bagId) => _owner.bagData.AddBag(bagId);
        
        /// <summary>
        /// 移除背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public bool RemoveBag(int bagId) => _owner.bagData.RemoveBag(bagId);
        
        /// <summary>
        /// 单个背包中道具是否存在
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int bagId, int itemId) => _owner.bagData.HasItem(bagId, itemId);
        
        /// <summary>
        /// 所有物品中道具是否存在
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int itemId) => _owner.bagData.HasItem(itemId);

        /// <summary>
        /// 添加道具到单个背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public int AddItem(int bagId, int itemId, int num = 1) => _owner.bagData.AddItem(bagId, itemId, num);
        
        /// <summary>
        /// 添加道具到单个背包
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="itemInfo"></param>
        /// <param name="num"></param>
        public int AddItem(int bagId, ItemInfo itemInfo, int num = 1) => _owner.bagData.AddItem(bagId, itemInfo, num);

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
        public BagItemInfo GetItem(int bagId, int itemId) => _owner.bagData.GetItem(bagId, itemId);
        
        /// <summary>
        /// 获得所有背包中指定itemId的道具列表
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<BagItemInfo> GetItems(int itemId) => _owner.bagData.GetItems(itemId);

        /// <summary>
        /// 获得指定背包中所有道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public List<BagItemInfo> GetAllItems(int bagId) => _owner.bagData.GetAllItems(bagId);

        /// <summary>
        /// 获得指定背包中可显示的道具
        /// </summary>
        /// <returns></returns>
        public List<BagItemInfo> GetAllVisibleItems(int bagId)
        {
            return _owner.bagData.GetAllItems(bagId).FindAll(i => i.visible == 1);
        }

        /// <summary>
        /// 根据id排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagById(int bagId, bool isAsc) => _owner.bagData.SortBagById(bagId, isAsc);
        
        /// <summary>
        /// 根据type排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagByType(int bagId, bool isAsc) => _owner.bagData.SortBagByType(bagId, isAsc);
        
        /// <summary>
        /// 根据num排序道具
        /// </summary>
        /// <param name="bagId"></param>
        /// <param name="isAsc"></param>
        public void SortBagByNum(int bagId, bool isAsc) => _owner.bagData.SortBagByNum(bagId, isAsc);

        /// <summary>
        /// 清空一个背包
        /// </summary>
        /// <param name="bagId"></param>
        public void ClearBag(int bagId) => _owner.bagData.ClearBag(bagId);

        /// <summary>
        /// 清空所有物品
        /// </summary>
        public void ClearAll() => _owner.bagData.ClearAll();
    }
}