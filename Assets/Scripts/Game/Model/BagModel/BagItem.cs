using System;
using UnityEngine;

namespace Game.Model.BagModel
{
    public enum ItemType
    {
        Common,
        Equip,
        EquipPiece,
        StudentPiece,
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