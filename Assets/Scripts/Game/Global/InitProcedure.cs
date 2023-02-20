using Hali_Framework;

namespace Game.Global
{
    public class InitProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //todo 初始化

            ChangeState<GameProcedure>();
        }
    }
}