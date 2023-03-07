using Game.UI.Begin;
using Hali_Framework;

namespace Game.Global
{
    public class BeginProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            UIMgr.Instance.ShowPanel<BeginPanel>();
        }

        protected internal override void OnLeave(IFsm<ProcedureMgr> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            //隐藏所有界面
            UIMgr.Instance.HideAllLoadingPanels();
            UIMgr.Instance.HideAllLoadedPanels();
        }
    }
}