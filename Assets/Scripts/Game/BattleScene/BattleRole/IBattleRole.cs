using Game.Model;
using UnityEngine;

namespace Game.BattleScene.BattleRole
{
    public interface IBattleRole
    {
        #region 角色状态

        /// <summary>
        /// 是否可控制
        /// </summary>
        public bool IsControl { get; set; }
        
        /// <summary>
        /// 当前生命
        /// </summary>
        public int CurHp { get; }
        
        /// <summary>
        /// 当前行动力
        /// </summary>
        public float CurAp { get; }
        
        /// <summary>
        /// 当前弹药量
        /// </summary>
        public int CurAmmo { get; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; }
        
        public IBattleRole AtkTarget { get; set; }

        #endregion

        #region 角色信息
        
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; }
        
        /// <summary>
        /// 最大生命
        /// </summary>
        public int MaxHp { get; }

        /// <summary>
        /// 最大行动力
        /// </summary>
        public float MaxAp { get; }

        /// <summary>
        /// 最大弹药量
        /// </summary>
        public int MaxAmmo { get; }

        /// <summary>
        /// 攻击类型
        /// </summary>
        public AtkType AtkType { get; }
        
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AtkRange { get; }
        
        /// <summary>
        /// 角色类型
        /// </summary>
        public RoleType RoleType { get; }
        
        public GameObject Go { get; }

        #endregion

        public void InitMe(object info);

        public void SubAp(float ap);

        public void SubHp(int hp);

        public void SubAmmo(int num);

        public void ResetAmmo();
    }
}