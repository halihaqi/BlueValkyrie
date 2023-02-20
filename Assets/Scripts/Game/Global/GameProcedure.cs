using Game.UI.Begin;
using Hali_Framework;

namespace Game.Global
{
    public class GameProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            UIMgr.Instance.ShowPanel<BeginPanel>();
        }
    }
}