using System;
using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Hali_Framework;
using UnityEngine;

namespace Game.Model.BagModel
{
    [Serializable]
    public class Inventory
    {
        public int ownerId;
        public Dictionary<int, Bag> dataDic;

        public Inventory(int ownerId)
        {
            this.ownerId = ownerId;
            dataDic = new Dictionary<int, Bag>();
        }

        public int BagCount => dataDic.Count;

        public bool HasBag(int bagId) => dataDic.ContainsKey(bagId);

        public void AddBag(int bagId)
        {
            if (HasBag(bagId))
                throw new Exception($"Can not has two bag with id:{bagId}.");
            dataDic.Add(bagId, new Bag(bagId, ownerId));
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
}