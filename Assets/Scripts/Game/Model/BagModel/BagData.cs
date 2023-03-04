using System;
using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Hali_Framework;
using UnityEngine;

namespace Game.Model.BagModel
{
    public enum ItemType
    {
        Common,
        Equip,
        EquipPiece,
    }
    
    [Serializable]
    public class BagData
    {
        public int ownerId;
        public Dictionary<int, BagInfo> dataDic;

        public BagData(int ownerId)
        {
            this.ownerId = ownerId;
            dataDic = new Dictionary<int, BagInfo>();
        }

        public int BagCount => dataDic.Count;

        public bool HasBag(int bagId) => dataDic.ContainsKey(bagId);

        public void AddBag(int bagId)
        {
            if (HasBag(bagId))
                throw new Exception($"Can not has two bag with id:{bagId}.");
            dataDic.Add(bagId, new BagInfo(bagId, ownerId));
        }

        public bool RemoveBag(int bagId) => dataDic.Remove(bagId);

        public int[] GetAllBagIds()
        {
            int[] arr = new int[dataDic.Count];
            int index = 0;
            foreach (var bagId in dataDic.Keys)
                arr[index++] = bagId;

            return arr;
        }

        //仅检测一个背包
        public bool HasItem(int bagId, int itemId) => HasBag(bagId) && dataDic[bagId].HasItem(itemId);
        
        //检测所有背包
        public bool HasItem(int itemId)
        {
            bool hasItem = false;
            foreach (var bag in dataDic.Values.Where(bag => bag.HasItem(itemId)))
                hasItem = true;

            return hasItem;
        }

        public int AddItem(int bagId, int itemId, int num)
        {
            if(!HasBag(bagId))
                return -1;
            return dataDic[bagId].Add(itemId, num);
        }
        
        public int AddItem(int bagId, ItemInfo itemInfo, int num)
        {
            if(!HasBag(bagId))
                return -1;
            return dataDic[bagId].Add(itemInfo, num);
        }
        
        public int AddItem(int bagId, BagItemInfo itemInfo, int num)
        {
            if(!HasBag(bagId))
                return -1;
            return dataDic[bagId].Add(itemInfo, num);
        }

        //仅获得单个背包中的道具
        public BagItemInfo GetItem(int bagId, int itemId)
        {
            if(!HasBag(bagId))
                return null;
            return dataDic[bagId].GetItem(itemId);
        }
        
        //获得所有背包中指定id道具
        public List<BagItemInfo> GetItems(int itemId)
        {
            List<BagItemInfo> results = new List<BagItemInfo>();
            foreach (var bag in dataDic.Values)
            {
                if(bag.HasItem(itemId))
                    results.Add(bag.GetItem(itemId));
            }

            return results;
        }
        
        //获得单个背包所有道具
        public List<BagItemInfo> GetAllItems(int bagId)
        {
            if(!HasBag(bagId))
                return null;
            var bag = dataDic[bagId];
            List<BagItemInfo> results = new List<BagItemInfo>(bag.items.Count);
            foreach (var item in bag.items)
            {
                results.Add(new BagItemInfo(item));
            }
            
            return results;
        }

        public void SortBagById(int bagId, bool isAsc)
        {
            if (!HasBag(bagId)) return;
            
            if(isAsc)
                dataDic[bagId].Sort((a, b) => a.id.CompareTo(b.id));
            else
                dataDic[bagId].Sort((a, b) => b.id.CompareTo(a.id));
        }

        public void SortBagByType(int bagId, bool isAsc)
        {
            if (!HasBag(bagId)) return;
            
            if(isAsc)
                dataDic[bagId].Sort((a, b) => a.type.CompareTo(b.type));
            else
                dataDic[bagId].Sort((a, b) => b.type.CompareTo(a.type));
        }
        
        public void SortBagByNum(int bagId, bool isAsc)
        {
            if (!HasBag(bagId)) return;
            
            if(isAsc)
                dataDic[bagId].Sort((a, b) => a.num.CompareTo(b.num));
            else
                dataDic[bagId].Sort((a, b) => b.num.CompareTo(a.num));
        }

        public void ClearBag(int bagId)
        {
            if(HasBag(bagId))
                dataDic[bagId].Clear();
        }

        public void ClearAll() => dataDic.Clear();
    }

    [Serializable]
    public class BagInfo
    {
        public int id;
        public int ownerId;
        public List<BagItemInfo> items;

        public BagInfo(int id, int ownerId)
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
            return new BagItemInfo(item);
        }

        public BagItemInfo GetItem(ItemInfo info) => GetItem(info.id);

        public List<BagItemInfo> GetTypeItems(int type)
            => items.FindAll(i => i.type == type);
        
        public void Sort(Comparison<BagItemInfo> comparison) => items.Sort(comparison);

        public void Clear() => items.Clear();
    }

    [Serializable]
    public class BagItemInfo
    {
        public int id;
        public string name;
        public int num;
        public int type;
        public int visible;
        public string resName;

        public BagItemInfo(ItemInfo info)
        {
            id = info.id;
            name = info.fullName;
            type = info.type;
            visible = info.visible;
            resName = info.resName;
            num = 0;
        }
        
        
        /// <summary>
        /// 拷贝使用
        /// </summary>
        /// <param name="info"></param>
        public BagItemInfo(BagItemInfo info)
        {
            id = info.id;
            name = info.name;
            type = info.type;
            visible = info.visible;
            resName = info.resName;
            num = info.num;
        }

        public int Add(int addNum = 1) => num = Mathf.Clamp(num + addNum, 0, 999999);
    }
}