using System;
using Game.UI.Base;
using Hali_Framework;

namespace Game.Managers
{
    public class TipMgr : Singleton<TipMgr>
    {
        public void ShowTip(string str)
            => UIMgr.Instance.ShowPanel<TipPop>(GameConst.UIGROUP_TIP, str);
        
        public void ShowConfirm(string tipStr, Action callback)
            => UIMgr.Instance.ShowPanel<ConfirmPop>(GameConst.UIGROUP_TIP, new ConfirmParam(tipStr, callback));
    }
}