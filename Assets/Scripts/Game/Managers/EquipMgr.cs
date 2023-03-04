using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public enum EquipType
    {
        Hat,
        Gloves,
        Bag,
        Shoes
    }
    
    public class EquipMgr : Singleton<EquipMgr>
    {
        private static readonly Dictionary<EquipType, string> ATTRIBUTE_NAMES = new Dictionary<EquipType, string>
        {
            {EquipType.Hat, "DEF"},
            {EquipType.Gloves, "ATK"},
            {EquipType.Bag, "HP"},
            {EquipType.Shoes, "MP"},
        };

        public EquipInfo FindEquip(int itemId)
        {
            var equipDic = BinaryDataMgr.Instance.GetTable<EquipInfoContainer>().dataDic;
            return equipDic.Values.FirstOrDefault(equipInfo => equipInfo.itemId == itemId);
        }
        
        
        public string GetAttributeName(EquipInfo equip)
        {
            return ATTRIBUTE_NAMES[(EquipType)equip.type];
        }

        public string GetAttributeName(int equipType)
        {
            return ATTRIBUTE_NAMES[(EquipType)equipType];
        }
        
        public string GetAttributeName(EquipType equipType)
        {
            return ATTRIBUTE_NAMES[equipType];
        }
    }
}