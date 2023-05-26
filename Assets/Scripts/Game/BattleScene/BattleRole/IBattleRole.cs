using Game.Model;
using UnityEngine;

namespace Game.BattleScene.BattleRole
{
    public interface IBattleRole
    {
        #region 可变信息

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
        
        /// <summary>
        /// 是否可控制
        /// </summary>
        public bool IsControl { get; set; }
        
        /// <summary>
        /// 攻击目标
        /// </summary>
        public IBattleRole AtkTarget { get; set; }
        
        /// <summary>
        /// 角色可变信息，可上buff和debuff
        /// </summary>
        public BattleRoleState RoleState { get; }

        #endregion

        #region 固定信息
        
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; }

        /// <summary>
        /// 攻击类型
        /// </summary>
        public AtkType AtkType { get; }

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