using System.Collections.Generic;
using Game.BattleScene;
using Game.Managers;
using Game.UI.Base;
using Hali_Framework;
using UnityEngine;

namespace Game.Global
{
    public class BattleProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //进入战斗场景
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>(GameConst.BATTLE_SCENE, OnEnterScene);
        }
        
        private void OnEnterScene()
        {
            var formation = new List<BattleRoleInfo>();
            formation.Add(RoleMgr.Instance.GetBattleRole(1002));
            formation.Add(RoleMgr.Instance.GetBattleRole(1003));
            formation.Add(RoleMgr.Instance.GetBattleRole(1005));
            formation.Add(RoleMgr.Instance.GetBattleRole(1011));
            formation.Add(RoleMgr.Instance.GetBattleRole(1013));
            BattleSceneMonoMgr.Instance.InitFormationEntity(formation, Camera.main);
        }
    }
}