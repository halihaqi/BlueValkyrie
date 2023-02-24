using System;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class ConfirmPop : PopBase
    {
        private Action _callback;
        private Text _txtTip;
        
        protected internal override void OnInit(object userData)
        {
            isModal = true;
            interactable = false;
            base.OnInit(userData);
            _txtTip = GetControl<Text>("txt_tip");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is ConfirmParam p)
            {
                _callback = p.callback;
                _txtTip.text = p.tipStr;
            }
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            HideMe();
            if (btnName.Equals("btn_sure"))
                _callback?.Invoke();
        }
    }

    public class ConfirmParam
    {
        public Action callback;
        public string tipStr;

        public ConfirmParam(string tipStr, Action callback)
        {
            this.callback = callback;
            this.tipStr = tipStr;
        }
    }
}