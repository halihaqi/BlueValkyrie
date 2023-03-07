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
    
    public class EquipMgr : Singleton<EquipMgr>, IModule
    {
        private Dictionary<int, EquipInfo> _equipDic;

        private static readonly Dictionary<EquipType, string> ATTRIBUTE_NAMES = new Dictionary<EquipType, string>
        {
            {EquipType.Hat, "DEF"},
            {EquipType.Gloves, "ATK"},
            {EquipType.Bag, "HP"},
            {EquipType.Shoes, "AP"},
        };

        public int Priority => 2;

        void IModule.Init()
        {
            _equipDic = BinaryDataMgr.Instance.GetTable<EquipInfoContainer>().dataDic;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _equipDic.Clear();
            _equipDic = null;
        }

        public EquipInfo GetEquip(int id)
        {
            return _equipDic.TryGetValue(id, out var equip) ? equip : null;
        }
        
        public EquipInfo FindEquip(int itemId)
        {
            return _equipDic.Values.FirstOrDefault(equipInfo => equipInfo.itemId == itemId);
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