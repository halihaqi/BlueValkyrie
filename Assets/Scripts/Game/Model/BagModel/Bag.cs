using System;
using System.Collections.Generic;
using Game.Managers;

namespace Game.Model.BagModel
{
    [Serializable]
    public class Bag
    {
        public int id;
        public int ownerId;
        public List<BagItemInfo> items;

        public Bag(int id, int ownerId)
        {
            this.id = id;
            this.ownerId = ownerId;
            items = new List<BagItemInfo>();
        }
        
        public bool HasItem(int itemId) => items.Find(i => i.id == itemId) != null;
        
        public bool HasItem(ItemInfo item) => items.Find(i => i.id == item.id) != null;

        public int Add(ItemInfo item, int num) => Add(item.id, num);

        public int Add(BagItemInfo item, int num) => Add(item.id, num);

        public int Add(int itemId, int num)
        {
            var targetItem = items.Find(i => i.id == itemId);
            if (targetItem == null)
            {
                if (num <= 0) return 0;
                var itemInfo = ItemMgr.Instance.GetItem(itemId);
                if (itemInfo == null)
                    throw new Exception($"Item id:{itemId} has no item.");
                targetItem = new BagItemInfo(itemInfo);
                targetItem.Add(num);
                items.Add(targetItem);
            }
            else
                targetItem.Add(num);

            //如果道具数为0，自动移除
            if (targetItem.num <= 0)
                items.Remove(targetItem);

            return targetItem.num;
        }

        public BagItemInfo GetItem(int itemId)
        {
            var item = items.Find(i => i.id == itemId);
            return item != null ? new BagItemInfo(item) : null;
        }

        public BagItemInfo GetItem(ItemInfo info) => GetItem(info.id);

        public List<BagItemInfo> GetTypeItems(int type)
            => items.FindAll(i => i.type == type);
        
        public void Sort(Comparison<BagItemInfo> comparison) => items.Sort(comparison);

        public void Clear() => items.Clear();
    }
}