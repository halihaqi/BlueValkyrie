using Game.Model;
using Hali_Framework;

namespace Game.BattleScene
{
    public class BattleStartState : FsmState<BattleMaster>
    {
        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            //todo 开场动画
            fsm.Owner.WarStart(RoleType.Student);
            ChangeState<BattleChessState>(fsm);
        }
    }
}