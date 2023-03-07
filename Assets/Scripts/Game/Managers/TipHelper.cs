using System;
using Game.UI.Base;
using Hali_Framework;

namespace Game.Managers
{
    public static class TipHelper
    {
        public static void ShowTip(string str)
            => UIMgr.Instance.ShowPanel<TipPop>(GameConst.UIGROUP_TIP, str);
        
        public static void ShowConfirm(string tipStr, Action callback)
            => UIMgr.Instance.ShowPanel<ConfirmPop>(GameConst.UIGROUP_TIP, new ConfirmParam(tipStr, callback));
    }
}