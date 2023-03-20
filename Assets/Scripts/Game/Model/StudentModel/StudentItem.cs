using System;
using Game.Managers;
using Hali_Framework;

namespace Game.Model
{
    public enum AtkType
    {
        Null,
        SMG,
        MG,
        SG,
        SR,
        RL
    }
    
    [Serializable]
    public class StudentItem
    {
        //base
        public int roleId;

        //base attribute
        public int baseHp;
        public int baseAp;
        public int baseAtk;
        public int baseDef;
        public int baseAmmo;
        public int atkType;
        public int atkRange;
        
        public int lv;
        public int star;
        public int exp;
        //0 hat, 1 gloves, 2 bag, 3 shoes
        public readonly EquipInfo[] equips;

        public int Hp => baseHp + equips[0]?.attribute ?? baseHp;
        
        public int Atk => baseAtk + equips[1]?.attribute ?? baseAtk;
        
        public int Def => baseDef + equips[2]?.attribute ?? baseDef;
        
        public int Ap => baseAp + equips[3]?.attribute ?? baseAp;

        public int Ammo => baseAmmo;

        public AtkType AtkType => (AtkType)atkType;
        
        public StudentItem(){}

        public StudentItem(BattleRoleInfo info)
        {
            roleId = info.id;
            baseHp = info.baseHp;
            baseAp = info.baseAp;
            baseAtk = info.baseAtk;
            baseDef = info.baseDef;
            baseAmmo = info.baseAmmo;
            atkType = info.atkType;
            atkRange = info.atkRange;

            lv = 1;
            star = 1;
            exp = 0;
            equips = new EquipInfo[4];
        }

        public StudentItem(int roleId)
        {
            var info = BinaryDataMgr.Instance.GetInfo<BattleRoleInfoContainer, int, BattleRoleInfo>(roleId);
            if (info == null)
                throw new Exception("Has no battle role in excel.");

            this.roleId = info.id;
            baseHp = info.baseHp;
            baseAp = info.baseAp;
            baseAtk = info.baseAtk;
            baseDef = info.baseDef;
            baseAmmo = info.baseAmmo;
            atkType = info.atkType;
            atkRange = info.atkRange;

            lv = 1;
            star = 1;
            exp = 0;
            equips = new EquipInfo[4];
        }

        /// <summary>
        /// 穿装备，如果传空则为脱
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public void WearEquip(int type, EquipInfo info)
        {
            equips[type] = info;
        }

        public void AddExp(int exps)
        {
            var upExp = StudentMgr.Instance.GetLvlUpExp(lv);
            exp += exps;

            //如果获得经验超出升级所需经验，就升级
            while (exp >= upExp)
            {
                //如果是最大等级
                if (lv >= StudentMgr.Instance.MaxLvl)
                {
                    exp = Math.Min(exp, upExp);
                    break;
                }
                LvlUp();
                exp -= upExp;
                upExp = StudentMgr.Instance.GetLvlUpExp(lv);
            }
        }

        public void StarUp()
        {
            if(star >= StudentMgr.Instance.MaxStar) return;
            var info = StudentMgr.Instance.GetStarUpInfo(star);
            ++star;
            baseHp = info.addHp;
            baseAp = info.addAp;
            baseAtk = info.addAtk;
            baseDef = info.addDef;
        }

        private void LvlUp()
        {
            var upInfo = StudentMgr.Instance.GetLvlUpInfo(lv);
            ++lv;
            baseHp += upInfo.addHp;
            baseAp += upInfo.addAp;
            baseAtk += upInfo.addAtk;
            baseDef += upInfo.addDef;
        }
    }
}