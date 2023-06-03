using Game.BattleScene.Fsms.BattleRoleFsm;
using Hali_Framework;

namespace Game.BattleScene
{
    public class BattleRunState : FsmState<BattleMaster>
    {
        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            UIMgr.Instance.RefocusPanel(fsm.Owner.BattlePanel, fsm.Owner.CurRole);
            fsm.Owner.CurRole.Fsm.ChangeState<RoleMoveState>();
        }
    }
}