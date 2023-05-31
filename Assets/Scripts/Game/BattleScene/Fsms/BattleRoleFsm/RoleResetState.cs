using Game.BattleScene.BattleRole;
using Hali_Framework;

namespace Game.BattleScene.Fsms.BattleRoleFsm
{
    public class RoleResetState : FsmState<IBattleRole>
    {
        protected internal override void OnEnter(IFsm<IBattleRole> fsm)
        {
            base.OnEnter(fsm);
            //禁止操控
            fsm.Owner.IsControl = false;
        }
    }
}