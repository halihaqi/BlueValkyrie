using Game.BattleScene.BattleRole;
using Hali_Framework;

namespace Game.BattleScene.Fsms.BattleRoleFsm
{
    public class RoleAimState : FsmState<IBattleRole>
    {
        protected internal override void OnEnter(IFsm<IBattleRole> fsm)
        {
            base.OnEnter(fsm);
            
        }
    }
}