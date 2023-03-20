using Game.UI.Base;
using Hali_Framework;

namespace Game.Global
{
    public class BattleProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //进入战斗场景
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>(GameConst.BATTLE_SCENE, null, false);
        }
    }
}